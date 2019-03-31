namespace OnWeb.Plugins.Materials.DB
{
    using Utils.Data;

    public partial class DataLayerContext : UnitOfWorkBase
    {
        public virtual IRepository<Page> Pages { get; set; }
        public virtual IRepository<PageCategory> PageCategories { get; set; }

        public virtual IRepository<News> News { get; set; }
    }

}
