using System.Collections.Generic;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Scheme
{
    /// <summary>
    /// Интерфейс для обращения к свойствам схем.
    /// </summary>
    /// <typeparam name="TField"></typeparam>
    public interface IScheme<TField> : IReadOnlyDictionary<int, TField> where TField : Field.IField
    {
        /// <summary>
        /// Возвращает название схемы полей.
        /// </summary>
        string NameScheme { get; }

        /// <summary>
        /// Ссылка на полную схему полей.
        /// Для <see cref="DefaultScheme.Default"/> ссылается на саму себя.
        /// </summary>
        DefaultScheme Default { get; }

        /// <summary>
        /// Указывает, является ли объект полной или частичной схемой.
        /// </summary>
        bool IsFullScheme { get; }
    }
}
