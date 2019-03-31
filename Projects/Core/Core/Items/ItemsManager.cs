using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.Items
{
    /// <summary>
    /// Менеджер, управляющий сущностями и типами сущностей.
    /// </summary>
    public class ItemsManager : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IUnitOfWorkAccessor<UnitOfWork<DB.ItemParent>>
    {
        private class ParentsInternal
        {
            public int item;
            public int type;
            public int parent;
            public int level;
        }

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion

        /// <summary>
        /// Позволяет сохранить список взаимосвязей родитель:потомок для типа сущностей <paramref name="idItemsType"/> (см. <see cref="ItemTypeFactory"/>).
        /// </summary>
        /// <param name="module">Модуль</param>
        /// <param name="relationsList">Список взаимосвязей</param>
        /// <param name="idItemsType">Идентификатор типа сущности (см. <see cref="DB.ItemType.IdItemType"/>).</param>
        /// <returns>Возвращает true, если сохранение прошло успешно и false, если возникла ошибка. Возвращает true, если <paramref name="relationsList"/> пуст.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="module"/> равен null.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="relationsList"/> равен null.</exception>
        public bool SaveChildToParentRelations(Modules.ModuleCore module, IEnumerable<ChildToParentRelation> relationsList, int idItemsType)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            if (relationsList == null) throw new ArgumentNullException(nameof(relationsList));

            try
            {
                relationsList = relationsList.ToList();
                if (relationsList.Count() == 0) return true;

                var toBase = new List<ParentsInternal>();

                toBase.Add(new ParentsInternal()
                {
                    item = 0,
                    type = idItemsType,
                    parent = 0,
                    level = 0,
                });

                var levelMax = 100;
                foreach (var pair in relationsList)
                {
                    var level = 0;

                    toBase.Add(new ParentsInternal()
                    {
                        item = pair.IdChild,
                        type = idItemsType,
                        parent = pair.IdChild,
                        level = level++,
                    });

                    var s = pair.IdParent > 0 ? pair.IdParent : 0;

                    while (s > 0 || level <= levelMax)
                    {
                        toBase.Add(new ParentsInternal()
                        {
                            item = pair.IdChild,
                            type = idItemsType,
                            parent = s,
                            level = level++,
                        });

                        if (s == 0 || level == levelMax) break;

                        //$s = 0;
                        //if (isset($items[$s]) && $items[$s] > 0) $s = $items[$s];
                        var s_ = relationsList.Where(x => x.IdChild == s).FirstOrDefault();
                        if (s_ != null) s = s_.IdParent;
                        else if (s_ == null && s > 0) break;
                        s = s > 0 ? s : 0;
                    }
                }

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope())
                {
                    foreach (var item in toBase.GroupBy(x => $"{x.item}_{x.type}_{x.parent}", x => x).Select(x => x.First()))
                    {
                        db.Repo1.Add(new DB.ItemParent()
                        {
                            IdModule = module.IdModule,
                            IdItem = item.item,
                            IdItemType = item.type,
                            IdParentItem = item.parent,
                            IdLevel = item.level
                        });
                    }

                    db.DataContext.ExecuteQuery($"DELETE FROM ItemParent WHERE IdModule='{module.IdModule}' AND IdItemType='{idItemsType}'");//" AND IdItem IN (".implode(', ', $ids).")");
                    if (toBase.Count > 0)
                    {
                        db.SaveChanges();
                        scope.Commit();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.Error, "Не удалось сохранить список пар", $"Модуль: {module.GetType().FullName}\r\nСписок пар: {relationsList.Count()}\r\nТип сущностей: {idItemsType}.", null, ex);
                return false;
            }

        }
    }
}
