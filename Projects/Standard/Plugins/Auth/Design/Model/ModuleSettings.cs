using System.Collections.Generic;

namespace OnWeb.Plugins.Auth.Design.Model
{
    using Core.DB;

    public class ModuleSettings : Auth.Model.Configuration
    {
        public List<Role> Roles { get; set; }

        public List<UserLogHistoryEventType> EventTypes { get; set; }
    }
}
