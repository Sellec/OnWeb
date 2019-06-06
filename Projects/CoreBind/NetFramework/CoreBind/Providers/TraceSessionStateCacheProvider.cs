//using System;
//using System.Collections.Specialized;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using System.Data;
//using System.Web;
//using System.Web.Configuration;
//using System.Web.SessionState;
//using System.IO;
//using System.Threading.Tasks;

//namespace OnWeb.CoreBind.Providers
//{
//    /// <summary>
//    /// Наш собственный провайдер.
//    /// </summary>
//    class TraceSessionStateCacheProvider : SessionStateStoreProviderBase
//    {
//        int _timeoutSession = 0;
//        int _timeoutSave = 2000;
//        int _timeoutRead = 30000;

//        private static object SyncRootSave = new object();
//        private static object SyncRootRead = new object();

//        private static ConcurrentDictionary<string, DB.Sessions> _cache = null;

//        private static Task _saveTask = null;
//        private static Task _readTask = null;

//        private class InternalStoreInfo
//        {
//            public const string KEY = "SECRET_SESSION_ITEM_DOES_NOT_REMOVE";

//            public SessionStateStoreData item;
//            public string id;
//            public object lockId;
//            public TraceSessionStateProvider provider;
//        }

//        /// <summary>
//        /// Инициализация провайдера, читаем конфигурацию, устаналиваем переменные...
//        /// </summary>
//        public override void Initialize(string name, NameValueCollection config)
//        {
//            if (config == null) throw new ArgumentNullException("config");
//            base.Initialize(name, config);

//            var applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
//            var configuration = WebConfigurationManager.OpenWebConfiguration(applicationName);

//            var configSection = (SessionStateSection)configuration.GetSection("system.web/sessionState");
//            _timeoutSession = 60 * 24 * 365;// (int)configSection.Timeout.TotalMinutes;

//            lock (SyncRootRead)
//            {
//                if (_cache == null) TaskReadFromDB();
//                else
//                {
//                    if (_readTask == null)
//                        _readTask = Task.Delay(_timeoutRead).ContinueWith(t => TaskReadFromDB());
//                }
//            }
//        }

//        private void TaskReadFromDB()
//        {
//            try
//            {
//                lock (SyncRootRead)
//                {
//                    if (_cache == null) _cache = new ConcurrentDictionary<string, DB.Sessions>();

//                    foreach (var res in _dbContext.Sessions.AsNoTracking()) _cache.TryAdd(res.SessionId, res);

//                    //Debug.WriteLine("TaskReadFromDB.Count={0}", _cache.Count);
//                    //foreach (var res in _cache.Values) Debug.WriteLine("TaskReadFromDB.\"{0}\"={1}", res.SessionId, _dbContext.GetState(res));

//                    _readTask = null;
//                }
//            }
//            catch (Exception ex) { Debug.WriteLine("TaskReadFromDB.Error: {0}", ex.Message); }
//        }

//        #region Методы SessionStateStoreProviderBase 
//        public override void Dispose()
//        {
//            //_dataContext.Dispose();
//        }

//        /// <summary>
//        /// Получаем сессию для режима "только для чтения" без необходимости блокировки.
//        /// </summary>
//        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
//        {
//            var sessionItem = GetSessionItem(context, id, false, out locked, out lockAge, out lockId, out actions);
//            context.Items[InternalStoreInfo.KEY] = new InternalStoreInfo() { id = id, lockId = lockId, item = sessionItem, provider = this };
//            return sessionItem;
//        }

//        /// <summary>
//        /// Получаем сессию в режиме эксклюзивного доступа с необходимостью блокировки.
//        /// </summary>
//        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
//        {
//            return GetSessionItem(context, id, true, out locked, out lockAge, out lockId, out actions);
//        }

//        /// <summary>
//        /// Обобщенный вспомогательный метод для получения доступа к сессии в базе данных.
//        /// Используется как GetItem, так и GetItemExclusive.
//        /// </summary>
//        private SessionStateStoreData GetSessionItem(HttpContext context, string id, bool exclusive, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
//        {
//            locked = false;
//            lockAge = new TimeSpan();
//            lockId = null;
//            actions = 0;

//            var sessionItem = GetSessionItem(id);

//            // Сессия не найдена
//            if (sessionItem == null) return null;

//            // Сессия найдена, но заблокирована
//            if (sessionItem.Locked)
//            {
//                locked = true;
//                lockAge = DateTime.UtcNow - sessionItem.LockDate;
//                lockId = sessionItem.LockId;
//                return null;
//            }

//            // Сессия найдена, но она истекла
//            if (DateTime.UtcNow > sessionItem.Expires)
//            {
//                sessionItem.IsDeleted = true;
//                _dbContext.DeleteEntity(sessionItem);
//                _cache.TryRemove(sessionItem.SessionId, out sessionItem);
//                SaveChanges();

//                return null;
//            }

//            // Сессия найдена, требуется эксклюзинвый доступ.
//            if (exclusive)
//            {
//                lock (sessionItem.SyncRoot)
//                {
//                    sessionItem.LockId += 1;
//                    sessionItem.Locked = true;
//                    sessionItem.LockDate = DateTime.UtcNow;

//                    SaveChanges();
//                }
//            }

//            locked = exclusive;
//            lockAge = DateTime.UtcNow - sessionItem.LockDate;
//            lockId = sessionItem.LockId;

//            var data = (sessionItem.ItemContent == null)
//                ? CreateNewStoreData(context, _timeoutSession)
//                : Deserialize(context, sessionItem.ItemContent, _timeoutSession);

//            data.Items["UserId"] = sessionItem.IdUser;

//            return data;
//        }

//        /// <summary>
//        /// Удаляем блокировку сессии, освобождаем ее для других потоков.
//        /// </summary>
//        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
//        {
//            var sessionItem = GetSessionItem(id);
//            if (sessionItem.LockId != (int)lockId) return;

//            lock (sessionItem.SyncRoot)
//            {
//                sessionItem.Locked = false;
//                sessionItem.Expires = DateTime.UtcNow.AddMinutes(_timeoutSession);
//                SaveChanges();
//            }
//        }

//        /// <summary>
//        /// Сохраняем состояние сессии и снимаем блокировку.
//        /// </summary>
//        public override void SetAndReleaseItemExclusive(HttpContext context,
//                                                        string id,
//                                                        SessionStateStoreData item,
//                                                        object lockId,
//                                                        bool newItem)
//        {
//            context.Items[InternalStoreInfo.KEY] = null;

//            var intLockId = lockId == null ? 0 : (int)lockId;
//            var userId = (int)item.Items["UserId"];

//            var data = ((SessionStateItemCollection)item.Items);
//            data.Remove("UserId");

//            // Сериализуем переменные
//            var itemContent = Serialize(data);

//            // Если это новая сессия, которой еще нет в базе данных.
//            if (newItem)
//            {
//                CreateUninitializedItem(context, id, _timeoutSession);
//                //return;//Не выходим из функции, т.к. в новую сессию еще надо записать данные.
//            }

//            // Если это старая сессия, проверяем совпадает ли ключ блокировки, 
//            // а после сохраняем состояние и снимаем блокировку.
//            var sessionItem = GetSessionItem(id);
//            lock (sessionItem.SyncRoot)
//            {
//                if (lockId == null || sessionItem.LockId == (int)lockId)
//                {
//                    sessionItem.IdUser = userId;
//                    sessionItem.ItemContent = itemContent;
//                    sessionItem.Expires = DateTime.UtcNow.AddMinutes(_timeoutSession);
//                    sessionItem.Locked = false;
//                    SaveChanges();
//                }
//            }
//        }

//        public static void SaveUnsavedSessionItem()
//        {
//            if (HttpContext.Current != null)
//            {
//                if (HttpContext.Current.Items.Contains(InternalStoreInfo.KEY))
//                {
//                    var sessionInfo = HttpContext.Current.Items[InternalStoreInfo.KEY] as InternalStoreInfo;
//                    if (sessionInfo != null)
//                    {
//                        if (sessionInfo.item.Items.Dirty)
//                        {
//                            sessionInfo.provider.SetAndReleaseItemExclusive(HttpContext.Current, sessionInfo.id, sessionInfo.item, sessionInfo.lockId, false);
//                        }
//                    }

//                    HttpContext.Current.Items.Remove(InternalStoreInfo.KEY);
//                }
//            }

//        }

//        /// <summary>
//        /// Удаляет запись о состоянии сессии.
//        /// </summary>
//        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
//        {
//            var sessionItem = GetSessionItem(id);
//            lock (sessionItem.SyncRoot)
//            {
//                if (sessionItem.LockId != (int)lockId) return;

//                sessionItem.IsDeleted = true;
//                try { _dbContext.DeleteEntity(sessionItem); }
//                catch { }
//                _cache.TryRemove(sessionItem.SessionId, out sessionItem);
//                SaveChanges();
//            }
//        }

//        /// <summary>
//        /// Сбрасывает счетчик жизни сессии.
//        /// </summary>
//        public override void ResetItemTimeout(HttpContext context, string id)
//        {
//            var sessionItem = GetSessionItem(id);
//            if (sessionItem == null) return;

//            lock (sessionItem.SyncRoot)
//            {
//                sessionItem.Expires = DateTime.UtcNow.AddMinutes(_timeoutSession);
//                SaveChanges();
//            }
//        }

//        /// <summary>
//        /// Создается новый объект, который будет использоваться для хранения состояния сессии в течении запроса.
//        /// Мы можем установить в него некоторые предопределенные значения, которые нам понадобятся.
//        /// </summary>
//        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
//        {
//            var data = new SessionStateStoreData(new SessionStateItemCollection(),
//                                                    SessionStateUtility.GetSessionStaticObjects(context),
//                                                    timeout);

//            if (data.Items["UserId"] == null) data.Items["UserId"] = 0;
//            return data;
//        }

//        /// <summary>
//        /// Создание пустой записи о новой сессии в хранилище сессий.
//        /// </summary>
//        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
//        {
//            var session = new DB.Sessions
//            {
//                SessionId = id,
//                IdUser = 0,
//                Created = DateTime.UtcNow,
//                Expires = DateTime.UtcNow.AddMinutes(timeout),
//                LockDate = DateTime.UtcNow,
//                Locked = false,
//                ItemContent = null,
//                LockId = 0,
//            };

//            _cache[id] = session;
//            SaveChanges();
//        }

//        private DB.Sessions GetSessionItem(string id)
//        {
//            DB.Sessions item = null;
//            if (_cache.TryGetValue(id, out item) && !item.IsDeleted)
//            {
//                return item;
//            }
//            else
//            {
//                return null;
//            }
//        }
//        #endregion

//        private void SaveChanges()
//        {
//            lock (SyncRootSave)
//            {
//                if (_saveTask == null)
//                    _saveTask = Task.Delay(_timeoutSave).ContinueWith(t =>
//                    {
//                        //lock (SyncRootSave)
//                        {
//                            try
//                            {
//                                int addedUpdated = 0;
//                                _cache.ToList().ForEach(p =>
//                                {
//                                    lock (p.Value.SyncRoot)
//                                    {
//                                        var entityState = _dbContext.GetState(p.Value);
//                                        if (!p.Value.IsDeleted)
//                                        {
//                                            if (entityState == Data.ItemState.Detached)
//                                            {
//                                                _dbContext.Sessions.AddOrUpdate(p.Value);
//                                                addedUpdated++;
//                                            }
//                                        }
//                                        else if (p.Value.IsDeleted)
//                                        {
//                                            if (entityState != Data.ItemState.Detached)
//                                            {
//                                                _dbContext.DeleteEntity(p.Value);

//                                                DB.Sessions item = null;
//                                                _cache.TryRemove(p.Key, out item);
//                                            }
//                                        }
//                                    }
//                                });

//                                int updated = _dbContext.SaveChanges();
//                                Debug.WriteLineNoLog($"SessionStateProvider: Update complete with {updated} items, aupd={addedUpdated}");
//                            }
//                            catch (Data.Errors.UpdateConcurrencyException ex)
//                            {
//                                Debug.WriteLine("SessionStateProvider: Update error2: {0}", ex.Message);
//                                foreach (var entry in ex.Entries)
//                                {
//                                    DB.Sessions item = entry.Entity as DB.Sessions;
//                                    _dbContext.Sessions.Delete(entry.Entity as DB.Sessions);
//                                    _cache.TryRemove(item.SessionId, out item);
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                Debug.WriteLine("SessionStateProvider: Update error: {0}", ex.Message);
//                            }
//                            finally
//                            {
//                                _saveTask = null;
//                            }
//                        }
//                    });
//            }
//        }


//        #region Property

//        private Session.SessionContext _dbContext
//        {
//            get => ApplicationCore.Instance.getInstance<Session.SessionContext>(ApplicationCore.eContextType.AppDomainContext);
//        }

//        #endregion

//        #region Ненужные методы в данной реализации

//        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback) { return false; }
//        public override void EndRequest(HttpContext context) { }
//        public override void InitializeRequest(HttpContext context) { }

//        #endregion

//        #region Вспомогательные методы сериализации и десериализации

//        private byte[] Serialize(SessionStateItemCollection items)
//        {
//            var ms = new MemoryStream();
//            var writer = new BinaryWriter(ms);

//            if (items != null) items.Serialize(writer);
//            writer.Close();

//            return ms.ToArray();
//        }

//        private SessionStateStoreData Deserialize(HttpContext context, Byte[] serializedItems, int timeout)
//        {
//            var ms = new MemoryStream(serializedItems);

//            var sessionItems = new SessionStateItemCollection();

//            if (ms.Length > 0)
//            {
//                var reader = new BinaryReader(ms);
//                sessionItems = SessionStateItemCollection.Deserialize(reader);
//            }

//            return new SessionStateStoreData(sessionItems, SessionStateUtility.GetSessionStaticObjects(context), timeout);
//        }

//        #endregion
//    }

//    namespace Session
//    {
//        using System;
//        using System.ComponentModel.DataAnnotations.Schema;
//        using System.Linq;

//        class SessionContext : Data.UnitOfWorkBase
//        {
//            public Data.IRepository<DB.Sessions> Sessions { get; set; }
//        }
//    }
//}