using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Data
{
#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Представляет полную схему полей объекта со значениями. Включает в себя информацию о дополнительных схемах и значениях полей в этих схемах.
    /// </summary>
    [NotMapped]
    public class DefaultSchemeWData : ISchemeWData
    {
        private ConcurrentDictionary<int, FieldData> _dictionary = new ConcurrentDictionary<int, FieldData>();
        private Scheme.DefaultScheme _defaultScheme = null;
        private Dictionary<uint, SchemeWData> _schemes = new Dictionary<uint, SchemeWData>();
        internal Scheme.SchemeItem _schemeItemSource = null;

        /// <summary>
        /// Создает новый объект <see cref="DefaultSchemeWData"/> на основании полной схемы полей <paramref name="defaultScheme"/>.
        /// </summary>
        /// <param name="defaultScheme"></param>
        public DefaultSchemeWData(Scheme.DefaultScheme defaultScheme)
        {
            _defaultScheme = defaultScheme;
        }

        #region IDictionary<int, FieldData>
        /// <summary>
        /// Возвращает поле с указанным <see cref="DB.CustomFieldsField.IdField"/>. Если такое поле отсутствует в коллекции, то возвращает null. 
        /// </summary>
        /// <param name="idField">Идентификатор поля.</param>
        /// <returns></returns>
        public FieldData this[int idField]
        {
            get
            {
                if (_defaultScheme.TryGetValue(idField, out Field.IField value))
                {
                    var data = _dictionary.GetOrAdd(idField, key => new FieldData(value));
                    return data;
                }

                return null;
            }
        }

        /// <summary>
        /// Возвращает количество полей в схеме.
        /// </summary>
        public int Count
        {
            get => _defaultScheme.Count;
        }

        /// <summary>
        /// Список идентификаторов полей в схеме.
        /// </summary>
        public IEnumerable<int> Keys
        {
            get => _defaultScheme.Keys;
        }

        /// <summary>
        /// Список объектов с данными полей в схеме.
        /// </summary>
        public IEnumerable<FieldData> Values
        {
            get => Keys.Select(x => this[x]).ToList();
        }

        /// <summary>
        /// Очищает схему от полей.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Проверяет наличие поля в схеме.
        /// </summary>
        /// <param name="IdField">Идентификатор поля.</param>
        /// <returns></returns>
        public bool ContainsKey(int IdField)
        {
            return _defaultScheme.ContainsKey(IdField);
        }

        public void CopyTo(KeyValuePair<int, FieldData>[] array, int arrayIndex)
        {
            throw new NotSupportedException("Метод не поддерживается для схем.");
        }

        public IEnumerator<KeyValuePair<int, FieldData>> GetEnumerator()
        {
            return Keys.Select(x => new KeyValuePair<int, FieldData>(x, this[x])).GetEnumerator();
        }

        public bool Remove(KeyValuePair<int, FieldData> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(int key)
        {
            return _dictionary.TryRemove(key, out FieldData value);
        }

        public bool TryGetValue(int key, out FieldData value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Свойства
        public string NameScheme
        {
            get { return _defaultScheme.NameScheme; }
        }

        public Scheme.DefaultScheme Default
        {
            get { return _defaultScheme.Default; }
        }

        public bool IsFullScheme
        {
            get { return _defaultScheme.IsFullScheme; }
        }

        //public ReadOnlyDictionary<int, SchemeWData> Schemes
        //{
        //    get;
        //    internal set;
        //}

        #endregion

        /// <summary>
        /// </summary>
        internal protected TOutType ProxyGetValue<TOutType>(int idField)
        {
            var fieldData = this[idField];
            if (fieldData != null)
            {
                var value = fieldData.Value;
                var d = typeof(TOutType);
                if (value != null) return (TOutType)value;
            }
            return default(TOutType);
        }

        /// <summary>
        /// </summary>
        internal protected void ProxySetValue<TOutType>(int idField, TOutType value)
        {
            var fieldData = this[idField];
            if (fieldData != null)
            {
                fieldData.Value = value;
            }
        }

        /// <summary>
        /// Копирует значения полей из источника <paramref name="valuesSource"/>, если эти поля присутствуют в текущей полной схеме. 
        /// </summary>
        /// <param name="valuesSource">Источник значений полей.</param>
        /// <param name="copyFilter">Если задан, то используется для определения того, нужно ли копировать значение конкретного поля.</param>
        public void CopyValuesFrom(ISchemeWData valuesSource, Func<FieldData, bool> copyFilter = null)
        {
            if (valuesSource != null)
            {
                foreach (var field in valuesSource)
                {
                    if (TryGetValue(field.Key, out FieldData value) && (copyFilter == null || copyFilter(field.Value)))
                    {
                        value.Value = field.Value.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает дополнительную схему с идентификатором <paramref name="idScheme"/> со значениями полей текущего объекта.
        /// Для значения 0 возвращает текущий объект.
        /// </summary>
        public ISchemeWData GetScheme(uint idScheme)
        {
            if (idScheme == 0) return this;

            if (!_schemes.ContainsKey(idScheme))
            {
                var scheme = this.Default.Schemes.GetValueOrDefault(idScheme, (k) => new Scheme.Scheme(null, this.Default)) as Scheme.Scheme;
                var schemeWData = new SchemeWData(scheme, this);
                _schemes[idScheme] = schemeWData;
            }

            return _schemes[idScheme];
        }
    }
}
