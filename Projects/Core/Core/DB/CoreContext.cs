using OnUtils.Data;
using OnUtils.Application.DB;

namespace OnWeb.Core.DB
{

#pragma warning disable CS1591 // todo внести комментарии.
    public class CoreContext : UnitOfWorkBase
    {
        public IRepository<ModuleConfig> Module { get; }

        public IRepository<ItemParent> ItemParent { get; }
        public IRepository<ItemType> ItemType { get; }
        public IRepository<Language> Language { get; }
        public IRepository<Sessions> Sessions { get; }

        public IRepository<PasswordRemember> PasswordRemember { get; }

        public IRepository<UserEntity> UserEntity { get; }
        public IRepository<UserLogHistory> UserLogHistory { get; }
        public IRepository<UserLogHistoryEventType> UserLogHistoryEventType { get; }
        public IRepository<User> Users { get; }

        public IRepository<Role> Role { get; }
        public IRepository<RoleUser> RoleUser { get; }
        public IRepository<RolePermission> RolePermission { get; }

        public IRepository<Subscription> Subscription { get; }
        public IRepository<SubscriptionEmail> SubscriptionEmail { get; }
        public IRepository<SubscriptionRole> SubscriptionRole { get; }
        public IRepository<SubscriptionHistory> SubscriptionHistory { get; }

        public IRepository<Routing> Routing { get; }
        public IRepository<RoutingType> RoutingType { get; }
    }
}
