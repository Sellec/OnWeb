using OnUtils;
using OnUtils.Application;
using OnUtils.Application.Journaling;
using OnUtils.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.Users
{
    using ExecutionResultEntities = ExecutionResult<IEnumerable<UserEntity>>;

    class EntitiesManager : CoreComponentBase, IEntitiesManager, IUnitOfWorkAccessor<UnitOfWork<DB.UserEntity>>
    {
        private const string SessionEntityKey = "UserEntity_";

        private Lazy<List<Type>> _entityTypesCache = null;
        private ConcurrentDictionary<string, DateTime> _cachedErrors = new ConcurrentDictionary<string, DateTime>();

        public EntitiesManager()
        {
            _entityTypesCache = new Lazy<List<Type>>(() =>
            {
                try
                {
                    return LibraryEnumeratorFactory.Enumerate<IEnumerable<Type>>(assembly => assembly.
                                                                                                        GetTypes().
                                                                                                        Where(x => !x.IsAbstract && typeof(UserEntity).IsAssignableFrom(x)).
                                                                                                        ToList()).
                                                                                           SelectMany(x => x).
                                                                                           ToList();
                }
                catch (Exception ex)
                {
                    this.RegisterEvent(EventType.Error, "Ошибка загрузки списка типов пользовательских сущностей.", "Первичная загрузка.", null, ex);
                    return new List<Type>();
                }
            });
            AppDomain.CurrentDomain.AssemblyLoad += (a, e) => {
                try
                {
                    var typesNew = e.LoadedAssembly.GetTypes().Where(x => !x.IsAbstract && typeof(UserEntity).IsAssignableFrom(x)).ToList();
                    if (typesNew.Count > 0) _entityTypesCache.Value.AddRange(typesNew);
                }
                catch (Exception ex)
                {
                    this.RegisterEvent(EventType.Error, "Ошибка загрузки списка типов объектов пользователя", $"Из сборки '{e.LoadedAssembly.FullName}'.", null, ex);
                }
            };
        }

        #region CoreComponentBase
        protected sealed override void OnStart()
        {
            // Получаем первичный список при инициализации. На всякий случай, чтобы отловить ошибку пораньше.
            var d = _entityTypesCache.Value;
        }

        protected override void OnStop()
        {
        }
        #endregion

        private UserEntity CreateEntityInstance(DB.UserEntity item, Type entityTypeClr = null)
        {
            if (entityTypeClr == null) entityTypeClr = _entityTypesCache.Value.Where(x => x.Name == item.EntityType || x.FullName == item.EntityType).FirstOrDefault();
            if (entityTypeClr == null && item.EntityType == "SavedObject") entityTypeClr = _entityTypesCache.Value.Where(x => x.Name == "RealtySavedObject").FirstOrDefault();

            if (entityTypeClr == null)
            {
                _cachedErrors.GetOrAddWithExpiration("entity_type_unknown_" + item.EntityType, (k) =>
                {
                    this.RegisterEvent(EventType.Error, "Неизвестный тип объекта пользователя", $"Тип сущности: {item.EntityType}.");
                    return DateTime.Now;
                }, TimeSpan.FromHours(1));
                return null;
            }
            else
            {
                var t = Activator.CreateInstance(entityTypeClr) as UserEntity;// new $res['EntityType']();
                var initResult = t.Init(item.IdEntity, item.Tag, item.Entity);
                if (initResult.IsSuccess)
                {
                    t.IdUser = item.IdUser;
                    return t;
                }
                else
                {
                    this.RegisterEvent(EventType.Error, "Ошибка при инициализации объекта пользователя", $"Тип объекта: {item.EntityType}.\r\nОшибка: {initResult.Message}.");
                    return null;
                }
            }
        }

        ExecutionResultEntities IEntitiesManager.GetEntitiesByEntityType(string entityType)
        {
            try
            {
                if (string.IsNullOrEmpty(entityType)) return new ExecutionResultEntities(false, "Требуется указать тип объекта пользователя для поиска.");

                var entityTypeClr = _entityTypesCache.Value.Where(x => x.Name == entityType || x.FullName == entityType).FirstOrDefault();

                if (entityTypeClr == null) return new ExecutionResultEntities(false, $"Тип объектов пользователя '{entityType}' не существует.");

                using (var db = this.CreateUnitOfWork())
                {
                    var entities = db.Repo1.
                        Where(x => x.EntityType == entityType).
                        ToList().
                        Select(res => CreateEntityInstance(res, entityTypeClr)).
                        Where(x => x != null).
                        ToList();

                    return new ExecutionResultEntities(true, null, entities);
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка во время получения списка объектов пользователя по типу", $"Тип объекта: {entityType}.", null, ex);
                return new ExecutionResultEntities(false, "Ошибка во время получения списка объектов пользователя");
            }
        }

        ExecutionResultEntities IEntitiesManager.GetUserEntities(int idUser, string entityTag)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var sql = db.Repo1.Where(x => x.IdUser == idUser);
                    if (!string.IsNullOrEmpty(entityTag)) sql = sql.Where(x => x.Tag == entityTag);

                    var list = sql.
                        ToList().
                        Select(res => CreateEntityInstance(res, null)).
                        Where(x => x != null).
                        ToList();

                    // todo заняться этим методом в рамках привязки к aspnet.
                    //if (!AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest && AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser() == IdUser)
                    //{
                    //    //TODO надо ли объединять с entities из сессии или лучше сделать, чтобы entities из сессии автоматом переливались в базу, когда юзер авторизовался?
                    //    var listFromSession = GetUserEntitiesSession(Tag);
                    //    if (listFromSession != null) list = list.Union(listFromSession);
                    //}

                    return new ExecutionResultEntities(true, null, list);
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка во время получения списка объектов пользователя по пользователю/тегу", $"ID пользователя: {idUser}.\r\nТег: {entityTag}.", null, ex);
                return new ExecutionResultEntities(false, "Ошибка во время получения списка объектов пользователя");
            }
        }

        // todo заняться этим методом в рамках привязки к aspnet.
        //public IEnumerable<UserEntity> GetUserEntitiesSession(string Tag = null)
        //{
        //    try
        //    {
        //        setError(null);

        //        if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
        //        {
        //            var keys = System.Web.HttpContext.Current.Session.Keys.OfType<string>().Where(x => x.StartsWith(SessionEntityKey)).Select(x => x.Substring(SessionEntityKey.Length)).ToList();
        //            if (keys.Count > 0)
        //            {

        //            }

        //            //TODO
        //            //    if (isset($_SESSION['UserEntity_'.$Tag]) && is_array($_SESSION['UserEntity_'.$Tag]))
        //            //        foreach ($_SESSION['UserEntity_'.$Tag] as $k =>$res)
        //            //{
        //            //        if (class_exists($res['EntityType'], true))
        //            //        {
        //            //        $t = new $res['EntityType']();
        //            //        $t->Main = $this->Main;
        //            //            try
        //            //            {
        //            //            $t->Init($res['IdEntity'], $res['Tag'], $res['Entity']);
        //            //            $t->IdUser = $res['IdUser'];
        //            //            $list[$res['IdEntity']] = $t;
        //            //            }
        //            //            catch (Exception $ex) { $this->Error.= $ex->getMessage().' '; }
        //            //            } 
        //            //    else $this->Error.= "Класс '".$res['EntityType']."' не существует. ";
        //            //        }

        //            //        return $list;
        //        }
        //    }
        //    catch (Exception ex) { setError(ex.Message); }
        //    return null;
        //}



    }
}