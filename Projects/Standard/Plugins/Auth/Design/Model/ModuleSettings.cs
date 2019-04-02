using System.Collections.Generic;

namespace OnWeb.Plugins.Auth.Design.Model
{
    using Core.DB;

    public class ModuleSettings
    {
        public List<Role> Roles;

        public List<UserLogHistoryEventType> EventTypes;
    }
}