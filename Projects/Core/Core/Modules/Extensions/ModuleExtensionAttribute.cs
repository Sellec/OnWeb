using System;

namespace OnWeb.Core.Modules.Extensions
{
#pragma warning disable CS1591 // todo внести комментарии.
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleExtensionAttribute : System.Attribute
    {
        public ModuleExtensionAttribute(string name, bool IsAdminPart = false)
        {
            this.IsAdminPart = IsAdminPart;
            this.Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public bool IsAdminPart
        {
            get;
            private set;
        }
    }
}
