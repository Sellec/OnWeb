using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Plugins.Communication
{
    using Core.Configuration;

    public class ModuleConfiguration : ModuleConfiguration<ModuleCommunication>
    {
        public string AdminUserName { get; set; }
    }
}
