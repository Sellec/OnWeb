using OnUtils.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace OnWeb.Plugins.Universal.Pagination
{
    using CoreBind.Modules;

    public abstract class PaginationHelper<TController, TItemSource, TItemView>
        where TController : ModuleControllerBase
        where TItemSource : class
        where TItemView : class
    {
        //private Lazy<MethodInfo> _mGetPaginatedItemsListOptions = new Lazy<MethodInfo>(() => typeof(PaginationHelper<,,,>).GetMethod(nameof(GetPaginatedItemsListOptions), BindingFlags.Instance | BindingFlags.NonPublic));
        private Func<TItemSource, TItemView> _itemSourceToViewConverter = null;

        public PaginationHelper(TController controller, Func<TItemSource, TItemView> itemSourceToViewConverter = null)
        {
            Controller = controller;
            _itemSourceToViewConverter = itemSourceToViewConverter;
        }

        private Func<TItemSource, TItemView> GetItemSourceToViewConverter()
        {
            if (_itemSourceToViewConverter == null)
            {
                if (typeof(TItemSource) != typeof(TItemView))
                    throw new ArgumentNullException(nameof(_itemSourceToViewConverter), "Может не указываться только в том случае, когда {TItemSource} равен {TItemVeiew}.");
                else
                    _itemSourceToViewConverter = (s) => s as TItemView;
            }

            return _itemSourceToViewConverter;
        }

        /// <summary>
        /// Вызывается внутри <see cref="ViewPaginatedItemsList{TModel}(TModel, IQueryable{TItemSource}, ListViewOptions, int?)"/>. 
        /// Должен возвращать объект типа <see cref="ListViewOptions"/> (или наследующий тип). 
        /// В случае возврата значения <see cref="null"/> генерируется исключение <see cref="NullReferenceException"/>.
        /// </summary>
        protected virtual ListViewOptions GetItemsListOptions<TModel>(TModel model) where TModel : class
        {
            return new ListViewOptions();
        }

        /// <summary>
        /// Обрабатывает данные из запроса <paramref name="sqlBaseQuery"/>, проводит ряд преобразований (сортировка, группировка и т.п.)
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="sqlBaseQuery"></param>
        /// <param name="listViewOptions"></param>
        /// <param name="IdPage"></param>
        /// <returns></returns>
        public ActionResult ViewPaginatedItemsList<TModel>(
            TModel model,
            IQueryable<TItemSource> sqlBaseQuery,
            ListViewOptions listViewOptions = null,
            int? IdPage = null
        )
            where TModel : class
        {
            if (listViewOptions == null)
            {
                //listViewOptions = (ListViewOptions)_mGetPaginatedItemsListOptions.Value.MakeGenericMethod(typeof(TModel)).Invoke(this, new object[] { model });
                listViewOptions = GetItemsListOptions(model);
                if (listViewOptions == null) throw new NullReferenceException("Метод 'UniversalController{TModule, TContext, TItemSource, TItemView}.GetPaginatedItemsListOptions' не должен возвращать null.");
            }

            var sqlBase = listViewOptions.BuildSortQuery(sqlBaseQuery);

            var data_items = this.ViewPrepareItems(sqlBase, out PagedView pages, out InfoCount infoCount, listViewOptions, IdPage ?? 0);
            if (!pages.PageFound) throw new ErrorCodeException(HttpStatusCode.NotFound, "Нет такой страницы.");

            return ExecuteView(model, sqlBase, data_items, listViewOptions, pages, infoCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pages"></param>
        /// <param name="infoCount"></param>
        /// <param name="listViewOptions"></param>
        /// <param name="IdPage">Если не задан, то отображается весь список объектов</param>
        /// <returns></returns>
        protected virtual List<TItemView> ViewPrepareItems(
            IQueryable<TItemSource> sql,
            out PagedView pages,
            out InfoCount infoCount,
            ListViewOptions listViewOptions,
            int? IdPage = null)
        {
            pages = null;
            infoCount = null;

            var startPosition = 0;

            sql = sql.Where(x => x != null);

            var itemsCount = sql.Count();

            if (IdPage.HasValue)
            {
                /*
                 * Рассчитываем страницы
                 * */
                pages = new PagedView();

                //Выбираем из настроек или из listViewOptions количество объектов на одной странице.
                var numberPerPage = listViewOptions != null && listViewOptions.numpage != 0 ? listViewOptions.numpage : 10;

                //Считаем текущую страницу. Первая страница начинается с 1.
                var currentPage = Math.Max(1, IdPage.Value);

                //Считаем стартовую позицию в списке объектов.
                startPosition = (currentPage - 1) * numberPerPage;

                //Считаем количество страниц.
                var countPages = itemsCount == 0 ? 0 : Math.Max(1, (int)Math.Ceiling(1.0 * itemsCount / numberPerPage));

                if (startPosition >= itemsCount)
                {
                    if (itemsCount == 0 && currentPage == 1)
                    {
                    }
                    else
                    {
                        pages.PageFound = false;
                        return null;
                    }
                }

                var stpages = new Dictionary<int, int>();
                var currentPage2 = currentPage - 1;

                if (currentPage2 > 3)
                    for (int i = currentPage2 - 1; i <= currentPage2; i++)
                        stpages[i] = i;
                else
                    for (int i = 1; i <= currentPage2; i++)
                        stpages[i] = i;

                var fnpages = new Dictionary<int, int>();
                if (currentPage2 < (countPages - 3))
                    for (int i = currentPage2 + 2; i <= currentPage2 + 3; i++)
                        fnpages[i] = i;
                else
                    for (int i = currentPage2 + 2; i <= countPages; i++)
                        fnpages[i] = i;

                pages.pages = countPages;
                pages.curpage = currentPage;
                pages.stpg = stpages;
                pages.fnpg = fnpages;
                pages.np = countPages - 3;

                infoCount = new InfoCount()
                {
                    all = itemsCount,
                    start = startPosition + 1,
                    page = currentPage * numberPerPage,
                    objectsPerPageTheory = numberPerPage,
                    objectsPerPage = Math.Min(itemsCount - (currentPage - 1) * numberPerPage, numberPerPage)
                };
            }
            else
            {
                infoCount = new InfoCount()
                {
                    all = itemsCount,
                    start = 1,
                    page = 0,
                    objectsPerPageTheory = itemsCount,
                    objectsPerPage = itemsCount
                };
            }

            //if (listViewOptions != null && listViewOptions.skip.HasValue) startPosition += listViewOptions.skip.Value;

            var data_items = sql.Skip(infoCount.start - 1).Take(infoCount.objectsPerPage).ToList().Select(x => GetItemSourceToViewConverter()(x)).ToList();

            return data_items;
        }

        protected abstract ActionResult ExecuteView<TModel>(
            TModel model,
            IQueryable<TItemSource> sqlBaseQueryPrepared,
            List<TItemView> objectsToView,
            ListViewOptions listViewOptions,
            PagedView pages,
            InfoCount infoCount
        ) where TModel : class;

        protected TController Controller { get; private set; }
    }

}