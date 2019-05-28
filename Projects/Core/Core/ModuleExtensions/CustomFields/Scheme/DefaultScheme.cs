using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Scheme
{
    using DB;

    /// <summary>
    /// Полная схема полей. 
    /// </summary>
    [NotMapped]
    public class DefaultScheme : IScheme<Field.IField>
    {
        private Dictionary<int, Field.IField> _dictionary = new Dictionary<int, Field.IField>();

        /// <summary>
        /// Создает новый объект типа <see cref="DefaultScheme"/>.
        /// </summary>
        /// <param name="fields">Список полей, относящихся к полной схеме.</param>
        public DefaultScheme(IList<Field.IField> fields = null)
        {
            if (fields != null)
                foreach (var field in fields)
                    _dictionary[field.IdField] = field;

            Schemes = new Dictionary<uint, IScheme<Field.IField>>();
        }

        #region IDictionary<int, Field.IField>
        /// <summary>
        /// Возвращает поле с идентификатором <paramref name="idField"/>, если такое поле присутствует в схеме, в противном случае - null.
        /// </summary>
        public Field.IField this[int idField]
        {
            get => _dictionary.TryGetValue(idField, out Field.IField value) ? value : null;
            set => _dictionary[idField] = value;
        }

        /// <summary>
        /// Возвращает количество полей в схеме.
        /// </summary>
        public int Count
        {
            get => _dictionary.Count;
        }

        /// <summary>
        /// Возвращает коллекцию идентификаторов полей в схеме.
        /// </summary>
        public IEnumerable<int> Keys
        {
            get => _dictionary.Keys;
        }

        /// <summary>
        /// Возвращает коллекцию полей в схеме.
        /// </summary>
        public IEnumerable<Field.IField> Values
        {
            get => _dictionary.Values;
        }

        /// <summary>
        /// Очищает схему.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Возвращает true, если поле с идентификатором <paramref name="idField"/> присутствует в схеме, в противном случае - false.
        /// </summary>
        public bool ContainsKey(int idField)
        {
            return _dictionary.ContainsKey(idField);
        }

        /// <summary>
        /// Копирует данные схемы в массив <paramref name="array"/>, начиная с указанного индекса массива.
        /// </summary>
        public void CopyTo(KeyValuePair<int, Field.IField>[] array, int arrayIndex)
        {
            (_dictionary as IDictionary<int, Field.IField>).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Возвращает перечислитель, осуществляющий перебор полей в схеме.
        /// </summary>
        public IEnumerator<KeyValuePair<int, Field.IField>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        /// <summary>
        /// Удаляет первое вхождение указанного поля в схеме.
        /// </summary>
        public bool Remove(KeyValuePair<int, Field.IField> item)
        {
            return (_dictionary as IDictionary<int, Field.IField>).Remove(item);
        }

        /// <summary>
        /// Удаляет поле с идентификатором <paramref name="idField"/> из схемы.
        /// </summary>
        public bool Remove(int idField)
        {
            return _dictionary.Remove(idField);
        }

        /// <summary>
        /// Возвращает поле с идентификатором <paramref name="idField"/>.
        /// </summary>
        /// <param name="idField">Идентификатор поля, которое необходимо получить.</param>
        /// <param name="value">Этот метод возвращает объект поля с указанным идентификатором, если такое поле присутствует в схеме; в противном случае — null. Этот параметр передается неинициализированным.</param>
        /// <returns>Возвращает true, если в схеме содержится поле с указанным идентификатором, в противном случае - false.</returns>
        public bool TryGetValue(int idField, out Field.IField value)
        {
            return _dictionary.TryGetValue(idField, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_dictionary as IEnumerable).GetEnumerator();
        }
        #endregion

        #region Свойства
        /// <summary>
        /// Название схемы полей.
        /// </summary>
        public string NameScheme { get => "По-умолчанию"; }

        /// <summary>
        /// Ссылка на полную схему полей.
        /// Для <see cref="DefaultScheme.Default"/> ссылается на саму себя.
        /// </summary>
        public DefaultScheme Default { get => this; }

        /// <summary>
        /// Указывает, является ли объект полной или частичной схемой.
        /// </summary>
        public bool IsFullScheme { get => true; }

        /// <summary>
        /// Возвращает коллекцию частичных схем.
        /// </summary>
        public Dictionary<uint, IScheme<Field.IField>> Schemes;

        #endregion

    }
}
