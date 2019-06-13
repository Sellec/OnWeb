using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field
{
    /// <summary>
    /// Представляет тип поля. Предоставляет информацию о типе поля, методы для проверки значений и прочее.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public abstract class FieldType
    {
        #region Методы
        /// <summary>
        /// Вызывается один раз при добавлении объекта в коллекцию типов полей.
        /// Если экземпляр этого типа поля уже зарегистрирован, то новый экземпляр не создается.
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// Проверяет список значений на корректность и соответствие всем параметрам этого типа поля.
        /// </summary>
        /// <param name="values">Список значений для проверки. Не может быть null, может содержать null.</param>
        /// <param name="field">Проверяемое поле.</param>
        /// <returns></returns>
        public abstract ValuesValidationResult Validate(IEnumerable<object> values, IField field);

        /// <summary>
        /// Возвращает дополнительные атрибуты, которые следует применить к генерируемому свойству в модели.
        /// </summary>
        /// <param name="field">Поле, для которого следует вернуть атрибуты.</param>
        public virtual IEnumerable<System.Reflection.Emit.CustomAttributeBuilder> GetCustomAttributeBuildersForModel(IField field)
        {
            return null;
        }

        public virtual ValueVariantCollection CreateValuesCollection(IField field, IEnumerable<DB.CustomFieldsValue> source)
        {
            return new ValueVariantCollection(source != null ? source.OrderBy(x => x.Order) : Enumerable.Empty<DB.CustomFieldsValue>());
        }

        /// <summary>
        /// Возвращает значение <see cref="TypeName"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return TypeName;
        }
        #endregion

        #region Свойства
        public static implicit operator int(FieldType fieldType)
        {
            return fieldType != null ? fieldType.IdType : 0;
        }

        public static implicit operator FieldType(int fieldType)
        {
            return FieldTypesCollection.GetType(fieldType);
        }

        /// <summary>
        /// Числовой уникальный идентификатор типа поля. Нужен для перехода со старых версий, где используется тип int для типа поля.
        /// Если такой же идентификатор встречается при регистрации нового типа поля в <see cref="FieldTypesCollection.RegisterFieldType{TFieldType}"/>, то будет сгенерировано исключение <see cref="ArgumentException"/>.
        /// </summary>
        public abstract int IdType
        {
            get;
        }

        /// <summary>
        /// Название типа поля.
        /// </summary>
        public abstract string TypeName
        {
            get;
        }

        /// <summary>
        /// Указывает, является ли значение, сохраняемое полем этого типа, ключом из источника данных (False) или "сырым" значением (т.е. непосредственно записанным в поле) (True).
        /// </summary>
        public abstract bool IsRawOrSourceValue
        {
            get;
        }

        /// <summary>
        /// Если не null, то принудительно заменяет настроенное значение поля <see cref="IField.IsMultipleValues"/>.
        /// </summary>
        public virtual bool? ForcedIsMultipleValues
        {
            get;
        }

        /// <summary>
        /// Если не null, то принудительно заменяет настроенный тип поля <see cref="IField.IdValueType"/>.
        /// </summary>
        public virtual FieldValueType? ForcedIdValueType
        {
            get;
        }
        #endregion

    }
}
