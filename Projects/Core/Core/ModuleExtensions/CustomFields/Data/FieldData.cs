using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Data
{
    using DB;

#pragma warning disable CS1591 // todo внести комментарии.
    [NotMapped]
    public class FieldData : Field.IField, IReadOnlyCollection<object>
    {
        private static IEnumerator<object> _emptyCollection = Enumerable.Empty<object>().GetEnumerator();

        private CustomFieldsField _field = null;
        internal HashSet<object> _values = null;

        public FieldData(Field.IField field)
        {
            _field = (CustomFieldsField)field;
            if (field == null) throw new Exception("Поле не может быть пустым.");
        }

        #region IField
        /// <summary>
        /// См. <see cref="Field.IField.DateChangeBase"/> 
        /// </summary>
        public DateTime DateChangeBase
        {
            get => _field.DateChangeBase;
        }

        /// <summary>
        /// См. <see cref="Field.IField.alias"/> 
        /// </summary>
        public string alias
        {
            get => _field.alias;
            set => _field.alias = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.IdField"/> 
        /// </summary>
        public int IdField
        {
            get => _field.IdField;
            set => _field.IdField = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.IsValueRequired"/> 
        /// </summary>
        public bool IsValueRequired
        {
            get => _field.IsValueRequired;
            set => _field.IsValueRequired = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.name"/> 
        /// </summary>
        public string name
        {
            get => _field.name;
            set => _field.name = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.size"/> 
        /// </summary>
        public int size
        {
            get => _field.size;
            set => _field.size = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.formatCheck"/> 
        /// </summary>
        public string formatCheck
        {
            get => _field.formatCheck;
            set => _field.formatCheck = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.FieldType"/> 
        /// </summary>
        public Field.FieldType FieldType
        {
            get => _field.FieldType;
        }

        /// <summary>
        /// См. <see cref="Field.IField.IdValueType"/> 
        /// </summary>
        public Field.FieldValueType IdValueType
        {
            get => _field.IdValueType;
            set => _field.IdValueType = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.nameEnding"/> 
        /// </summary>
        public string nameEnding
        {
            get => _field.nameEnding;
            set => _field.nameEnding = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.nameAlt"/> 
        /// </summary>
        public string nameAlt
        {
            get => _field.nameAlt;
            set => _field.nameAlt = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.ParameterNumeric01"/> 
        /// </summary>
        public float ParameterNumeric01
        {
            get => _field.ParameterNumeric01;
            set => _field.ParameterNumeric01 = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.ParameterNumeric02"/> 
        /// </summary>
        public float ParameterNumeric02
        {
            get => _field.ParameterNumeric02;
            set => _field.ParameterNumeric02 = value;
        }

        /// <summary>
        /// См. <see cref="Field.IField.IsMultipleValues"/> 
        /// </summary>
        public bool IsMultipleValues
        {
            get => _field.IsMultipleValues;
        }

        /// <summary>
        /// См. <see cref="Field.IField.GetDisplayName"/> 
        /// </summary>
        public string GetDisplayName()
        {
            return _field.GetDisplayName();
        }

        /// <summary>
        /// См. <see cref="Field.IField.GetValueType"/> 
        /// </summary>
        public Type GetValueType()
        {
            return _field.GetValueType();
        }

        /// <summary>
        /// См. <see cref="Field.IField.GetDisplayValue(object, Field.OutputType)"/> 
        /// </summary>
        public string GetDisplayValue(object value, Field.OutputType outputType = Field.OutputType.Text)
        {
            return _field.GetDisplayValue(value, outputType);
        }

        public Field.ValueVariantCollection data
        {
            get => _field.data;
        }

        #endregion

        #region Values
        internal void AddValue(CustomFieldsData value)
        {
            if (!_field.IsMultipleValues) ClearValue();

            if (_values == null) _values = new HashSet<object>();

            if (_field.IdValueType == Field.FieldValueType.KeyFromSource)
                _values.Add(value.IdFieldValue);
            else
            {
                object val = value.FieldValue;
                var typeToConvert = GetValueType();
                if (typeToConvert == typeof(bool))
                {
                    bool valueParsed;
                    if (bool.TryParse(value.FieldValue, out valueParsed)) val = valueParsed;
                    else
                    {
                        int valueParsed2;
                        if (int.TryParse(value.FieldValue, out valueParsed2)) val = valueParsed2 != 0;
                    }
                }

                try
                {
                    _values.Add(Convert.ChangeType(val, typeToConvert));
                }
                catch (FormatException) { }
                catch { throw; }
            }
        }

        public void ClearValue()
        {
            if (_values != null) _values.Clear();
            _values = null;
        }

        public virtual object Value
        {
            get
            {
                var valueType = GetValueType();

                if (_field.IsMultipleValues)
                {
                    var vals = Activator.CreateInstance(typeof(HashSet<>).MakeGenericType(valueType));
                    var add = vals.GetType().GetMethod(nameof(_values.Add));

                    if (_values != null)
                        foreach (var val in _values)
                            add.Invoke(vals, new object[] { Convert.ChangeType(val, valueType) });

                    return vals;
                }
                else
                {
                    if (_values == null) return null;
                    else return _values.Select(x => Convert.ChangeType(x, valueType)).FirstOrDefault();
                }
            }
            set
            {
                Type valueType = null;

                var values = new HashSet<object>();
                if (value == null)
                {
                    //values
                }
                else if (value.GetType().IsValueType || value is string)
                {
                    values.Add(value);
                    valueType = value.GetType();
                }
                else
                {
                    var genericType = OnUtils.Types.TypeHelpers.ExtractGenericInterface(value.GetType(), typeof(IEnumerable<>));
                    if (genericType != null)
                    {
                        foreach (var val in (value as IEnumerable))
                        {
                            if (val != null)
                            {
                                values.Add(val);
                                valueType = val.GetType();
                            }
                        }
                    }
                    else throw new NotSupportedException($"Тип значения '{value.GetType().FullName}' не поддерживается.");
                }

                if (valueType != null)
                {
                    if (valueType != GetValueType()) throw new ArgumentException($"Поле '{GetDisplayName()}' принимает значения типа '{GetValueType().FullName}', не '{valueType.FullName}'.");
                }

                if (!IsMultipleValues && values.Count > 1) throw new ArgumentOutOfRangeException($"Для поля '{GetDisplayName()}' не допускается указание нескольких значений.");
                if (_field.FieldType == null || _field.FieldType is Field.FieldTypes.UnknownFieldType) throw new NullReferenceException($"Тип поля '{GetDisplayName()}' не определен!");

                var result = _field.FieldType.Validate(values, _field);
                if (result == ValidationResult.Success || (result != null && result.IsSuccess))
                {
                    ClearValue();
                    if (result.Values.Count() > 0) _values = new HashSet<object>(result.Values.ToArray());
                }
                else throw new ValidationException(result.ErrorMessage);
            }
        }

        /// <summary>
        /// Возвращает true, если для данного поля задано любое значение. Возвращает false, если для этого поля нет значения.
        /// </summary>
        /// <returns></returns>
        public bool IsValueEmpty()
        {
            return _values == null || _values.Count == 0;
        }
        #endregion

        #region IReadOnlyCollection<object>
        public int Count
        {
            get => _values != null ? _values.Count : 0;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _values != null ? _values.GetEnumerator() : _emptyCollection;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values != null ? _values.GetEnumerator() : _emptyCollection;
        }
        #endregion

        /// <summary>
        /// Возвращает значение/значения поля в виде строки, где значения разделены точкой с запятой.
        /// </summary>
        public override string ToString()
        {
            if (_field.IsMultipleValues)
            {
                return _values == null ? "" : string.Join("; ", _values.Select(x => _field.GetDisplayValue(x)));
            }
            else
            {
                return Value != null ? Value.ToString() : "";
            }
        }

    }
}
