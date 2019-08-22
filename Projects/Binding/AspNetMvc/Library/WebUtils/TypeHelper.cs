using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace OnWeb.WebUtils
{
    using OnUtils.Utils;

    /// <summary>
    /// Вспомогательные методы для работы с типами и коллекциями. Взято из System.Web.WebPages.dll
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Проверяет, является ли тип <paramref name="checkedType"/> производным от <paramref name="baseType"/>.
        /// </summary>
        /// <param name="checkedType"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsHaveBaseType(Type checkedType, Type baseType)
        {
            var tt = checkedType;

            while (true)
            {
                if (baseType.IsAssignableFrom(tt)) return true;
                if (tt.IsGenericType && tt.IsConstructedGenericType)
                {
                    var ttGeneric = tt.GetGenericTypeDefinition();
                    if (ttGeneric == baseType) return true;
                    if (IsHaveBaseType(ttGeneric, baseType)) return true;
                }

                tt = tt.BaseType;
                if (tt == typeof(object)) break;
            }

            return false;
        }

        public static RouteValueDictionary ObjectToDictionary(object value)
        {
            var routeValueDictionary = new RouteValueDictionary();
            if (value != null)
            {
                var properties = PropertyHelper.GetProperties(value);
                for (int i = 0; i < properties.Length; i++)
                {
                    var propertyHelper = properties[i];
                    routeValueDictionary.Add(propertyHelper.Name, propertyHelper.GetValue(value));
                }
            }
            return routeValueDictionary;
        }

        public static RouteValueDictionary ObjectToDictionaryUncached(object value)
        {
            var routeValueDictionary = new RouteValueDictionary();
            if (value != null)
            {
                var properties = PropertyHelper.GetProperties(value);
                for (int i = 0; i < properties.Length; i++)
                {
                    var propertyHelper = properties[i];
                    routeValueDictionary.Add(propertyHelper.Name, propertyHelper.GetValue(value));
                }
            }
            return routeValueDictionary;
        }

        public static void AddAnonymousObjectToDictionary(IDictionary<string, object> dictionary, object value)
        {
            var routeValueDictionary = TypeHelper.ObjectToDictionary(value);
            foreach (var current in routeValueDictionary)
            {
                dictionary[current.Key] = current.Value;
            }
        }

        /// <summary>
        /// Определяет, является ли указанный тип анонимным.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false) && type.IsGenericType && type.Name.Contains("AnonymousType") && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) || type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase)))
            {
                var arg_6D_0 = type.Attributes;
                return 0 == 0;
            }
            return false;
        }
    }

}
