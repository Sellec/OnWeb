using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Scheme
{
    using DB;

    /// <summary>
    /// Представляет частичную схему, основанную на полной схеме <see cref="DefaultScheme"/>.
    /// </summary>
    [NotMapped]
    public class Scheme : IScheme<Field.IField>
    {
        private List<int> _fields = new List<int>();

        /// <summary>
        /// Создает новый объект типа <see cref="Scheme"/>.
        /// </summary>
        /// <param name="fields">Список полей, относящихся к частичной схеме.</param>
        /// <param name="defaultScheme">Полная схема, к которой относится создаваемый объект частичной схемы.</param>
        public Scheme(List<int> fields, DefaultScheme defaultScheme)
        {
            if (defaultScheme == null) throw new Exception("Схема по-умолчанию не может быть пустым объектом.");

            this.Default = defaultScheme;
            if (fields != null)
                foreach (var field in fields)
                    if (Default.ContainsKey(field))
                        _fields.Add(field);
        }

        #region IReadOnlyDictionary<int, Data.DB.Field.IField>
        /// <summary>
        /// Возвращает поле с идентификатором <paramref name="idField"/>, если такое поле присутствует в схеме, в противном случае - null.
        /// </summary>
        public Field.IField this[int key]
        {
            get
            {
                return _fields.Contains(key) ? Default[key] : null;
            }
            set
            {
                if (!_fields.Contains(key) && Default.ContainsKey(key)) _fields.Add(key);
            }
        }

        /// <summary>
        /// Возвращает количество полей в схеме.
        /// </summary>
        public int Count
        {
            get { return _fields.Count; }
        }

        /// <summary>
        /// Возвращает значение, указывающее, что коллекция находится в режиме "только для чтения".
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Возвращает коллекцию идентификаторов полей в схеме.
        /// </summary>
        public IEnumerable<int> Keys
        {
            get { return _fields; }
        }

        /// <summary>
        /// Возвращает коллекцию полей в схеме.
        /// </summary>
        public IEnumerable<Field.IField> Values
        {
            get { return Default.Where(x => _fields.Contains(x.Key)).Select(x => x.Value) as ICollection<Field.IField>; }
        }

        /// <summary>
        /// Очищает схему.
        /// </summary>
        public void Clear()
        {
            _fields.Clear();
        }

        /// <summary>
        /// Возвращает true, если поле с идентификатором <paramref name="idField"/> присутствует в схеме, в противном случае - false.
        /// </summary>
        public bool ContainsKey(int idField)
        {
            return _fields.Contains(idField);
        }

        public void CopyTo(KeyValuePair<int, Field.IField>[] array, int arrayIndex)
        {

        }

        /// <summary>
        /// Возвращает перечислитель, осуществляющий перебор полей в схеме.
        /// </summary>
        public IEnumerator<KeyValuePair<int, Field.IField>> GetEnumerator()
        {
            var fields = Default.Where(x => _fields.Contains(x.Key)).Select(x => x).GetEnumerator();
            return fields;
        }

        /// <summary>
        /// Удаляет первое вхождение указанного поля в схеме.
        /// </summary>
        public bool Remove(KeyValuePair<int, Field.IField> item)
        {
            return _fields.Remove(item.Key);
        }

        /// <summary>
        /// Удаляет поле с идентификатором <paramref name="idField"/> из схемы.
        /// </summary>
        public bool Remove(int idField)
        {
            return _fields.Remove(idField);
        }

        /// <summary>
        /// Возвращает поле с идентификатором <paramref name="idField"/>.
        /// </summary>
        /// <param name="idField">Идентификатор поля, которое необходимо получить.</param>
        /// <param name="value">Этот метод возвращает объект поля с указанным идентификатором, если такое поле присутствует в схеме; в противном случае — null. Этот параметр передается неинициализированным.</param>
        /// <returns>Возвращает true, если в схеме содержится поле с указанным идентификатором, в противном случае - false.</returns>
        public bool TryGetValue(int idField, out Field.IField value)
        {
            value = this[idField];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Свойства
        /// <summary>
        /// Идентификатор схемы полей.
        /// </summary>
        public uint IdScheme { get; internal set; }

        /// <summary>
        /// Название схемы полей.
        /// </summary>
        public string NameScheme { get; internal set; }

        /// <summary>
        /// Ссылка на полную схему полей.
        /// Для <see cref="DefaultScheme.Default"/> ссылается на саму себя.
        /// </summary>
        public DefaultScheme Default { get; private set; }

        /// <summary>
        /// Указывает, является ли объект полной или частичной схемой.
        /// </summary>
        public bool IsFullScheme { get { return false; } }
        #endregion
    }

}


