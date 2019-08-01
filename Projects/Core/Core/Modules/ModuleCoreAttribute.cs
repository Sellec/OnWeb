using System;

namespace OnWeb.Core.Modules
{
    /// <summary>
    /// Модуль обязательно должен быть помечен данным атрибутом, в противном случае возникает ошибка во время привязки типов.
    /// </summary>
    public class ModuleCoreAttribute : OnUtils.Application.Modules.ModuleCoreAttribute
    {
        /// <summary>
        /// Создает новый экземпляр атрибута.
        /// </summary>
        /// <param name="caption">Отображаемое имя модуля.</param>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="caption"/> является пустой строкой или null.</exception>
        public ModuleCoreAttribute(string caption) : base(caption)
        {
        }
    }
}
