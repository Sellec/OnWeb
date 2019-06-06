using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnWeb.Core.Configuration
{
    class ConfigurationValuesProvider : IReadOnlyDictionary<string, object>
    {
        private DataTable _storage = new DataTable();

        public ConfigurationValuesProvider()
        {
            var columns = new DataColumn[] {
                new DataColumn("Property", typeof(string)) { DefaultValue = "" },
                new DataColumn("Value", typeof(object)) { DefaultValue = null }
            };
            _storage.Columns.AddRange(columns);
            _storage.PrimaryKey = new DataColumn[] { columns[0] };
        }

        public void Load(string serializedJson)
        {
            try
            {
                if (!string.IsNullOrEmpty(serializedJson))
                {
                    var source = JsonConvert.DeserializeObject<Hashtable>(serializedJson);
                    if (source != null)
                        foreach (var key in source.Keys)
                            this.Add(key.ToString(), source[key]);

                    _storage.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public void Load(IDictionary<string, object> values)
        {
            try
            {
                if (values != null)
                    foreach (var pair in values)
                        this.Add(pair.Key, pair.Value);

                _storage.AcceptChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public string Save()
        {
            var serialized = JsonConvert.SerializeObject(ToDictionary());
            return serialized;
        }

        public object this[string key]
        {
            get
            {
                var row = (from DataRow p in _storage.Rows where p["Property"].Equals(key) select p).FirstOrDefault();
                if (row != null) return row["Value"];
                return null;
            }

        }

        public int Count
        {
            get => _storage.Rows.Count;
        }

        public IEnumerable<string> Keys
        {
            get => (from DataRow p in _storage.Rows select p["Property"].ToString()).ToList();
        }

        public IEnumerable<object> Values
        {
            get => (from DataRow p in _storage.Rows select p["Value"]).ToList();
        }

        private void Add(string key, object value)
        {
            _storage.LoadDataRow(new object[] { key, value }, true);
        }

        public bool ContainsKey(string key)
        {
            return (from DataRow p in _storage.Rows where p["Property"].Equals(key) select p).Count() > 0;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return (from DataRow p in _storage.Rows select new KeyValuePair<string, object>(p["Property"].ToString(), p["Value"])).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(string key)
        {
            var row = (from DataRow p in _storage.Rows where p["Property"].Equals(key) select p).FirstOrDefault();
            if (row != null) _storage.Rows.Remove(row);

            return true;
        }

        public void Clear()
        {
            _storage.Clear();
        }

        public bool TryGetValue(string key, out object value)
        {
            value = null;
            if (!ContainsKey(key)) return false;
            value = this[key];
            return true;
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            var row = (from DataRow p in _storage.Rows where p["Property"].Equals(key) select p).FirstOrDefault();
            if (row != null)
            {
                try
                {
                    var value = row["Value"];
                    if (value == null) return defaultValue;
                    if (value is T) return (T)value;

                    var type = typeof(T);

                    try
                    {
                        if (type.IsEnum)
                        {
                            var underlyingType = Enum.GetUnderlyingType(type);
                            if (value.GetType() == underlyingType)
                            {
                                return Enum.IsDefined(type, value) ? (T)Enum.Parse(type, value.ToString()) : defaultValue;
                            }
                            else
                            {
                                var converterValue = System.ComponentModel.TypeDescriptor.GetConverter(value.GetType());
                                if (converterValue.CanConvertTo(underlyingType))
                                {
                                    var valConverted = converterValue.ConvertTo(value, underlyingType);
                                    return Enum.IsDefined(type, valConverted) ? (T)Enum.Parse(type, valConverted.ToString()) : defaultValue;
                                }
                            }
                        }
                    }
                    catch { }

                    var converter = System.ComponentModel.TypeDescriptor.GetConverter(type);
                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        if (converter.IsValid(value))
                        {
                            var valConverted = converter.ConvertFrom(value);
                            return (T)valConverted;
                        }
                    }

                    try
                    {
                        var tt = Convert.ChangeType(value, type);
                        return (T)tt;
                    }
                    catch { }

                    try
                    {
                        var tt = JsonConvert.DeserializeObject<T>(value.ToString());
                        return tt;
                    }
                    catch { }

                }
                catch { }
            }

            return defaultValue;
        }

        public void Set<T>(string key, T value)
        {
            _storage.LoadDataRow(new object[] { key, value }, true);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return (from DataRow p in _storage.Rows select p).ToDictionary(x => x["Property"]?.ToString(), x => x["Value"]);
        }
    }
}
