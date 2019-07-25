using OnUtils.Application.DB;
using OnUtils.Data;

namespace OnWeb.Plugins.Adminmain
{
    using Routing.DB;

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
        public IRepository<Role> Role { get; }

        /// <summary>
        /// </summary>
        public IRepository<Routing> Routes { get; }

        /// <summary>
        /// </summary>
        public IRepository<RoutingType> RouteTypes { get; }
    }
}