namespace OnWeb.Core.DB
{
    using OnUtils.Data;
    using OnUtils.Data.UnitOfWork;

#pragma warning disable CS1591 // todo внести комментарии.
    public class CoreContext : UnitOfWorkBase
    {
        internal IRepository<captcha> captcha { get; }

        internal IRepository<config_extensions> config_extensions { get; }
        public IRepository<config_menus> config_menus { get; }
        public IRepository<ModuleConfig> Module { get; }
        internal IRepository<config_modules_extensions> config_modules_extensions { get; }

        public IRepository<ItemParent> ItemParent { get; }
        public IRepository<ItemType> ItemType { get; }
        public IRepository<Language> Language { get; }
        public IRepository<menus> menus { get; }
        public IRepository<Sessions> Sessions { get; }
        public IRepository<Theme> Theme { get; }

        public IRepository<MessageQueue> MessageQueue { get; }
        public IRepository<MessageQueueHistory> MessageQueueHistory { get; }


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

        public IRepository<WordCase> WordCase { get; }
    }
}
