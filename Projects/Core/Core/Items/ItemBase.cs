using OnUtils.Application.Modules;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Core.Items
{
    using Modules;
    using OnUtils.Items;

    /// <summary>
    /// Базовый класс для сущностей.
    /// Предоставляет некоторый набор методов и виртуальных свойств, используемых во многих расширениях и частях движка.
    /// Некоторые же части движка работают ТОЛЬКО с объектами, унаследованными от <see cref="ItemBase"/> (например, расширение CustomFields).
    /// Поддерживает атрибут <see cref="ConstructorInitializerAttribute"/> для методов класса. 
    /// </summary>
    [Serializable]
    public abstract partial class ItemBase : IItemBase
    {
        //[NotMapped]
        //[Newtonsoft.Json.JsonIgnore]
        private Uri _link = null;

        /// <summary>
        /// Беспараметрический конструктор, создающий сущность, НЕ привязанную к модулю. 
        /// Подробнее про привязку к модулю см. <see cref="Owner"/>.
        /// </summary>
        public ItemBase() : this(null)
        {
        }

        /// <summary>
        /// Конструктор, принимающий в качестве аргумента ссылку на модуль-владелец сущности.
        /// Вызов конструктора с <paramref name="owner"/> = null аналогичен вызову беспараметрического конструктора.
        /// Подробнее про привязку к модулю см. <see cref="Owner"/>.
        /// </summary>
        public ItemBase(ModuleCore owner)
        {
            this.Owner = owner;

            MethodMarkCallerAttribute.CallMethodsInObject<ConstructorInitializerAttribute>(this);
        }

        #region Методы
        /// <summary>
        /// Возвращает ссылку на объект, сгенерированную с учетом типа объекта.
        /// В первую очередь пытается получить ссылку, обращаясь к методу <see cref="GenerateLinkBase"/>, который может быть перегружен у наследуемых типов. 
        /// Если <see cref="GenerateLinkBase"/> возвращает null, то пытается обратиться к методу <see cref="ModuleCore.GenerateLink(ItemBase)"/> владельца <see cref="Owner"/>, если владелец задан.
        /// Если предыдущие попытки не увенчались успехом, возвращает null.
        ///
        /// После первого обращения значение ссылки кешируется во внутреннем свойстве и при последующих обращениях не вызывает регенерации, 
        /// если не задано свойство <paramref name="ignoreCache"/>.
        /// </summary>
        /// <param name="ignoreCache">Если равно true, ссылка генерируется при каждом обращении к методу.</param>
        public Uri GenerateLink(bool ignoreCache = false)
        {
            if (_link != null && !ignoreCache) return _link;
            if (Owner != null) _link = OwnerModule.GenerateLink(this);
            return _link;
        }

        /// <summary>
        /// См. <see cref="GenerateLink(bool)"/>. 
        /// </summary>
        protected virtual Uri GenerateLinkBase()
        {
            return null;
        }
        #endregion

        #region Свойства
        /// <summary>
        /// Возвращает идентификатор объекта.
        /// Должен быть переопределен в класса-потомке и, для сущностей из БД, привязан к целочисленному свойству-идентификатору.
        /// </summary>
        [NotMapped]
        public abstract int ID
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает название (заголовок) объекта.
        /// Должен быть переопределен в класса-потомке. Например, для сущностей из БД, может возвращать заголовок статьи, логин или никнейм пользователя и т.п.
        /// </summary>
        [NotMapped]
        public abstract string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает и задает дату последнего изменения объекта, если поддерживается классом-потомком.
        /// По-умолчанию (без переопределения в классе-потомке) возвращает null.
        /// </summary>
        [NotMapped]
        public virtual DateTime DateChangeBase
        {
            get; set;
        }

        /// <summary>
        /// Модуль, к которому относится объект. Может быть пустым.
        /// Привязка к модулю важна для работы некоторых методов и некоторого функционала движка.
        /// Важен для работы метода <see cref="GenerateLink(bool)"/>.
        /// Может быть задан напрямую, может быть передан в качестве аргумента для конструктора, может быть автоматически определен в конструкторе для класса <see cref="ItemBase{TModuleType}"/> (см. описание класса).
        /// </summary>
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public object Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Модуль, к которому относится объект. Может быть пустым.
        /// Привязка к модулю важна для работы некоторых методов и некоторого функционала движка.
        /// Важен для работы метода <see cref="GenerateLink(bool)"/>.
        /// Может быть задан напрямую, может быть передан в качестве аргумента для конструктора, может быть автоматически определен в конструкторе для класса <see cref="ItemBase{TModuleType}"/> (см. описание класса).
        /// </summary>
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public ModuleCore OwnerModule
        {
            get { return Owner as ModuleCore; }
        }

        #endregion

        /// <summary>
        /// Возвращает <see cref="Caption"/> при приведении к строке.
        /// </summary>
        public override string ToString()
        {
            return Caption;
        }

    }

    /// <summary>
    /// Базовый класс для сущностей с привязкой к модулю.
    /// Параметр-тип <typeparam name="TModuleType"/> позволяет беспараметрическому конструктору автоматически 
    /// найти объект модуля (обращением к <see cref="ModulesManager{TApplication}.GetModule{TModule}(bool)"/>) и задать в <see cref="ItemBase.Owner"/>.
    /// </summary>
    public abstract class ItemBase<TModuleType> : ItemBase where TModuleType : ModuleCore
    {
        /// <summary>
        /// </summary>
        public ItemBase() : base(GetModule())
        {
        }

        /// <summary>
        /// </summary>
        public ItemBase(TModuleType module) : base(module)
        {
        }

        private static TModuleType GetModule()
        {
            var module = DeprecatedSingletonInstances.ModulesManager.GetModule<TModuleType>();
            return module;
        }
    }
}
