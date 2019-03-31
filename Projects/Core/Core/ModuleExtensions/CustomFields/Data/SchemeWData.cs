using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Data
{
#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Представляет частичную схему полей с данными.
    /// </summary>
    [NotMapped]
    public class SchemeWData : ISchemeWData
    {
        private DefaultSchemeWData _defaultScheme = null;
        private Scheme.Scheme _scheme = null;

        public SchemeWData(Scheme.Scheme scheme, DefaultSchemeWData defaultSchemeWData)
        {
            _scheme = scheme;
            _defaultScheme = defaultSchemeWData;
        }

        #region IDictionary<int, FieldData>
        public FieldData this[int key]
        {
            get { return _scheme.ContainsKey(key) ? (_defaultScheme as IReadOnlyDictionary<int, FieldData>)[key] : null; }
        }

        public int Count
        {
            get { return _scheme.Count; }
        }

        public IEnumerable<int> Keys
        {
            get { return _scheme.Keys; }
        }

        public IEnumerable<FieldData> Values
        {
            get { return _scheme.Select(x => this[x.Key]).ToList(); }
        }

        public void Clear()
        {
            foreach (var item in this) this[item.Key].ClearValue();
        }

        public bool ContainsKey(int key)
        {
            return _scheme.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<int, FieldData>[] array, int arrayIndex)
        {
        }

        public IEnumerator<KeyValuePair<int, FieldData>> GetEnumerator()
        {
            return _scheme.Select(x => new KeyValuePair<int, FieldData>(x.Key, _defaultScheme[x.Key])).GetEnumerator();
        }

        public bool Remove(int key)
        {
            if (!ContainsKey(key)) return false;

            this[key].ClearValue();
            return true;
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
            get { return _scheme.NameScheme; }
        }

        public Scheme.DefaultScheme Default
        {
            get { return _scheme.Default; }
        }

        public bool IsFullScheme
        {
            get { return _scheme.IsFullScheme; }
        }

        #endregion

    }
}
