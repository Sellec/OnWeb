using OnUtils.Data;

namespace OnWeb.Plugins.Materials.DB
{
    public partial class DataLayerContext : UnitOfWorkBase
    {
        public virtual IRepository<Page> Pages { get; set; }
        public virtual IRepository<PageCategory> PageCategories { get; set; }

        public virtual IRepository<News> News { get; set; }
    }

}
