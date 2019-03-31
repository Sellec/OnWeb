using OnUtils.Data;

namespace OnWeb.Plugins.ModuleAdminmain
{
    using Core.DB;

    /// <summary>
    /// Контекст для доступа к данным.
    /// </summary>
    public class DataContext : UnitOfWorkBase
    {
        /// <summary>
        /// </summary>
        public IRepository<ModuleConfig> ConfigModules { get; }

        /// <summary>
        /// </summary>
        public IRepository<menus> EditableMenu { get; }

        /// <summary>
        /// </summary>
        public IRepository<Routing> Routes { get; }

        /// <summary>
        /// </summary>
        public IRepository<RoutingType> RouteTypes { get; }
    }
}