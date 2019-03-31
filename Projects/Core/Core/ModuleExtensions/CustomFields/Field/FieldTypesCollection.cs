using OnUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field
{
    /// <summary>
    /// Представляет коллекцию типов полей.
    /// </summary>
    public static class FieldTypesCollection
    {
        private static object SyncRoot = new object();
        private static Dictionary<Type, FieldType> _fieldTypes = new Dictionary<Type, FieldType>();

        static FieldTypesCollection()
        {
            lock (SyncRoot)
            {
                UnknownField = new FieldTypes.UnknownFieldType();

                var fieldType = typeof(FieldType);
                LibraryEnumeratorFactory.Enumerate(assembly => assembly.
                    GetTypes().
                    Where(x => fieldType.IsAssignableFrom(x) && !x.IsAbstract && x.IsClass).
                    ForEach(x => RegisterFieldType(x)), null, LibraryEnumeratorFactory.EnumerateAttrs.ExcludeKnownExternal | LibraryEnumeratorFactory.EnumerateAttrs.ExcludeMicrosoft | LibraryEnumeratorFactory.EnumerateAttrs.ExcludeSystem);
            }
        }

        /// <summary>
        /// Регистрирует новый тип поля.
        /// Если тип поля еще не регистрировался, то создается экземпляр указанного типа и для него вызывается метод <see cref="FieldType.Init"/>.
        /// </summary>
        public static void RegisterFieldType<TFieldType>() where TFieldType : FieldType
        {
            RegisterFieldType(typeof(TFieldType));
        }

        /// <summary>
        /// Регистрирует новый тип поля.
        /// Если тип поля еще не регистрировался, то создается экземпляр указанного типа и для него вызывается метод <see cref="FieldType.Init"/>.
        /// </summary>
        /// <param name="fieldType"></param>
        public static void RegisterFieldType(Type fieldType)
        {
            lock (SyncRoot)
            {
                if (!typeof(FieldType).IsAssignableFrom(fieldType)) throw new ArgumentException("Параметр должен быть наследником типа FieldType", nameof(fieldType));
                if (fieldType == typeof(FieldTypes.UnknownFieldType)) return;

                if (!_fieldTypes.ContainsKey(fieldType))
                {
                    var typeInstance = (FieldType)Activator.CreateInstance(fieldType);
                    if (string.IsNullOrEmpty(typeInstance.TypeName)) throw new Exception("Название типа поля не может быть пустым.");
                    if (_fieldTypes.Values.Where(x => x.IdType == typeInstance.IdType).Count() > 0) throw new Exception($"Тип поля с числовым идентификатором {typeInstance.IdType} уже зарегистрирован.");

                    typeInstance.Init();
                    _fieldTypes.Add(fieldType, typeInstance);
                }
            }
        }

        /// <summary>
        /// Возвращает список объектов, представляющих типы полей.
        /// </summary>
        public static List<FieldType> GetTypes()
        {
            //однажды вызвало блокировку. todo проверить.
            //lock (SyncRoot)
            {
                return _fieldTypes.Values.ToList();
            }
        }

        /// <summary>
        /// Возвращает тип поля с идентификатором типа поля <see cref="FieldType.IdType"/> равным <paramref name="typeId"/>.
        /// Если передан неизвестный тип поля, то возвращает <see cref="UnknownField"/>.
        /// </summary>
        public static FieldType GetType(int typeId)
        {
            var fieldType = _fieldTypes.Values.Where(x => x.IdType == typeId).FirstOrDefault();
            if (fieldType == null) fieldType = UnknownField;
            return fieldType;
        }

        /// <summary>
        /// Возвращает тип поля с типом <typeparamref name="TFieldType"/>.
        /// Если передан неизвестный тип поля, то возвращает <see cref="UnknownField"/>.
        /// </summary>
        public static FieldType GetType<TFieldType>() where TFieldType : FieldType
        {
            var fieldType = _fieldTypes.GetValueOrDefault(typeof(TFieldType), UnknownField);
            return fieldType;
        }



        internal static FieldTypes.UnknownFieldType UnknownField
        {
            get;
        }

    }
}
