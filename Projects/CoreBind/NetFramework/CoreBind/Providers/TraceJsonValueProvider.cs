using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Providers
{
    using OnUtils.Types;

    class TraceJsonValueProvider<TValue> : DictionaryValueProvider<TValue>
    {
        public TraceJsonValueProvider(IDictionary<string, TValue> dictionary, CultureInfo culture)
            : base(dictionary, culture)
        {

        }

        public override ValueProviderResult GetValue(string key)
        {
            var value = base.GetValue(key);
            return value != null ? new TraceJsonValueProviderResult(value.RawValue, value.AttemptedValue, value.Culture) : null;
        }


    }

    class TraceJsonValueProviderResult : ValueProviderResult
    {
        public TraceJsonValueProviderResult(object rawValue, string attemptedValue, CultureInfo culture)
            : base(rawValue, attemptedValue, culture)
        {
        }

        public override object ConvertTo(Type type, CultureInfo culture)
        {
            object result = null;

            try
            {
                result = base.ConvertTo(type, culture);
            }
            catch (Exception)
            {
                try
                {
                    result = TryToConvert(this.RawValue, type, culture);
                }
                catch (Exception)
                {
                    throw new InvalidOperationException("Невозможно преобразовать.");
                }
            }

            return result;
        }

        private object TryToConvert(object value, Type type, CultureInfo culture)
        {
            object result = null;
            try
            {
                var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(value, new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                    TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None
                });

                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject(serialized, type);
                return deserialized;
            }
            catch (Exception)
            {
                Type sourceElement = null;
                Type destElement = null;

                IEnumerable sourceIterator = null;

                if (value.GetType().IsArray) sourceElement = value.GetType().GetElementType();
                else
                {
                    var enumerableInterface = TypeHelpers.ExtractGenericInterface(value.GetType(), typeof(IEnumerable<>));
                    if (enumerableInterface != null)
                    {
                        sourceElement = enumerableInterface.GetGenericArguments()[0];
                        sourceIterator = value as IEnumerable;
                    }
                }

                if (type.IsArray) destElement = type.GetElementType();
                else
                {
                    var enumerableInterface = TypeHelpers.ExtractGenericInterface(type, typeof(IEnumerable<>));
                    if (enumerableInterface != null) destElement = enumerableInterface.GetGenericArguments()[0];
                }

                if (sourceElement != null && destElement != null)
                {
                    try
                    {
                        var destElements = new List<object>();
                        foreach (var element in sourceIterator)
                        {
                            var converted = (element is IConvertible) ? Convert.ChangeType(element, destElement) : TryToConvert(element, destElement, culture);
                            destElements.Add(converted);
                        }

                        var r = TryToConvert(destElements, type, culture);
                        result = r;
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            return result;
        }
    }

}
