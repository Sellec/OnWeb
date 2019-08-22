using System;
using System.Web.Mvc;

namespace OnWeb.Core.Modules
{
    /// <summary>
    /// Обозначает контроллер модуля.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleControllerAttribute : Attribute
    {
        /// <summary>
        /// Создает новый экземпляр атрибута с указанным типом контроллера. Не должен применяться в пользовательском коде.
        /// </summary>
        public ModuleControllerAttribute(int controllerType)
        {
            ControllerTypeID = controllerType;
        }

        /// <summary>
        /// Обозначает тип контроллера
        /// </summary>
        public int ControllerTypeID
        {
            get;
            private set;
        }
    }
}
