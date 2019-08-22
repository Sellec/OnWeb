using OnUtils.Application.Modules;
using OnUtils.Application.Users;
using System;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Core.Modules
{
    /// <summary>
    /// Атрибут, позволяющий задать url-доступное имя метода и разрешение для доступа к данному методу. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleActionAttribute : Attribute
    {
        public ModuleActionAttribute(string alias = null, string permission = null) : this(alias, string.IsNullOrEmpty(permission) ? Guid.Empty : permission.GenerateGuid())
        {
        }

        protected ModuleActionAttribute(string alias = null, Guid? permission = null)
        {
            Alias = alias;
            Permission = permission ?? Guid.Empty;
        }

        #region Property
        /// <summary>
        /// Псевдоним метода. К методу можно обратиться по его имени (например, Index), но и через прописанный в свойстве псевдоним. 
        /// </summary>
        public string Alias
        {
            get;
            protected set;
        }

        /// <summary>
        /// Разрешение, требующееся для открытия данного метода. Для подробностей см. <see cref="ModuleBase{TApplication}.CheckPermission(Guid)"/>. 
        /// </summary>
        public Guid Permission
        {
            get;
            protected set;
        }

        /// <summary>
        /// Название метода для отображения в информационных сообщениях, списках методов и т.п.
        /// </summary>
        public string Caption
        {
            get;
            protected set;
        }
        #endregion
    }
}
