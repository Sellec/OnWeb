using System;
using System.Web.Mvc;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field
{
#pragma warning disable CS1591 // todo внести комментарии.
    public interface IField
    {
        /// <summary>
        /// Возвращает или задает идентификатор поля.
        /// </summary>
        int IdField { get; set; }

        string name { get; set; }

        string nameAlt { get; set; }

        string alias { get; set; }

        /// <summary>
        /// Возвращает тип поля (см. описание <see cref="FieldType"/>). 
        /// Если внутренний тип поля не соответствует ни одному из известных типов полей (см. <see cref="FieldTypesCollection"/>), то возвращает <see cref="Field.FieldTypesCollection.UnknownField"/>.
        /// </summary>
        FieldType FieldType { get; }

        FieldValueType IdValueType { get; set; }

        int size { get; set; }

        bool IsValueRequired { get; set; }

        string nameEnding { get; set; }

        string formatCheck { get; set; }

        float ParameterNumeric01 { get; set; }

        float ParameterNumeric02 { get; set; }

        DateTime DateChangeBase { get; }

        ValueVariantCollection data { get; }

        /// <summary>
        /// Указывает, что поле может хранить несколько значений. Для некоторых типов полей (например, <see cref="Field.FieldTypes.SourceMultipleFieldType"/>) значение всегда будет true.
        /// </summary>
        bool IsMultipleValues { get; }

        #region Методы
        /// <summary>
        /// Возвращает отображаемое название поля. Если задан, то выводится <see cref="nameAlt"/>. Если нет, то выводится <see cref="name"/>.
        /// </summary>
        string GetDisplayName();

        /// <summary>
        /// Возвращает Runtime-тип, соответствующий допустимому типу значений поля.
        /// </summary>
        Type GetValueType();

        /// <summary>
        /// Возвращает представление для значения поля в зависимости от типа данных поля (в зависимости от <see cref="GetValueType"/>) и типа вывода.
        /// </summary>
        /// <param name="value">Значение, которое следует преобразовать. Тип значения должен соответствовать типу, возвращаемому методом <see cref="GetValueType"/>.</param>
        /// <param name="outputType">Тип вывода. Если равен <see cref="OutputType.Text"/>, то возвращается <see cref="string"/>. Если равен <see cref="OutputType.Html"/>, то возвращается <see cref="MvcHtmlString"/> </param>
        string GetDisplayValue(object value, OutputType outputType);

        #endregion

    }
}
