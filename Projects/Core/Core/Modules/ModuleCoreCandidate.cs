using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace OnWeb.Core.Modules
{
    /// <summary>
    /// Описывает класс модуля.
    /// </summary>
    public class ModuleCoreCandidate
    {
        private ModuleCoreCandidate (Type type, ModuleCoreAttribute info)
        {
            Type = type;
            Info = info;
        }

        ///// <summary>
        ///// Создает экземпляр класса на основе существующего модуля.
        ///// </summary>
        //public static ModuleCoreCandidate CreateFromModule<ModuleCore>( module)
        //{
        //    return new ModuleCoreCandidate(module.GetType(), module.GetType().GetCustomAttributes<ModuleCoreAttribute>(true).FirstOrDefault());
        //}

        /// <summary>
        /// Тип модуля.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Дополнительная информация о модуле из атрибута <see cref="ModuleCoreAttribute"/>.
        /// </summary>
        public ModuleCoreAttribute Info { get; private set; }

    }
}
