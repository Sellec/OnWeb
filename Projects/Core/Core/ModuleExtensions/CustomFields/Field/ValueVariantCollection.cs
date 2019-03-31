using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field
{
    /// <summary>
    /// Коллекция допустимых значений для поля <see cref="IField"/>.
    /// </summary>
    public class ValueVariantCollection : List<DB.CustomFieldsValue>
    {
        /// <summary>
        /// Создает новый объект <see cref="ValueVariantCollection"/>.
        /// </summary>
        public ValueVariantCollection()
        {
        }

        /// <summary>
        /// Создает новый объект <see cref="ValueVariantCollection"/> на базе списка значений <paramref name="source"/>.
        /// </summary>
        public ValueVariantCollection(IEnumerable<DB.CustomFieldsValue> source) : base(source)
        {

        }

        private bool TryGetValue(object key, out DB.CustomFieldsValue value)
        {
            value = null;

            if (key != null)
                foreach (var data in this)
                    if (data.IdFieldValue.Equals(key) || data.IdFieldValue.ToString() == key.ToString())
                    {
                        value = data;
                        return true;
                    }

            return false;
        }

        /// <summary>
        /// Проверяет существование варианта значения с <see cref="DB.CustomFieldsValue.IdFieldValue"/>, равным <paramref name="key"/>. 
        /// Сравнение <see cref="DB.CustomFieldsValue.IdFieldValue"/> и <paramref name="key"/> производится как (int)to(int), так и (string)to(string).
        /// </summary>
        public bool ContainsKey(object key)
        {
            DB.CustomFieldsValue value;
            return TryGetValue(key, out value);
        }

        /// <summary>
        /// Возвращает вариант значения с <see cref="DB.CustomFieldsValue.IdFieldValue"/>, равным <paramref name="key"/>. 
        /// См. <see cref="ContainsKey(object)"/>.
        /// Если ключ не найден, то генерирует исключение <see cref="KeyNotFoundException"/>.
        /// </summary>
        public DB.CustomFieldsValue this[object key]
        {
            get
            {
                DB.CustomFieldsValue value;
                if (TryGetValue(key, out value)) return value;
                //else throw new KeyNotFoundException();
                return null;
            }
        }
    }
}
