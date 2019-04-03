using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using OnUtils;
using OnUtils.Application;
using OnUtils.Application.Modules;
using OnUtils.Data;
using OnUtils.Utils;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Core.Modules
{
    /// <summary>
    /// Базовый класс для всех модулей. Обязателен при реализации любых модулей, т.к. при задании привязок в DI проверяется наследование именно от этого класса.
    /// </summary>
    public abstract class ModuleCore : ModuleBase<ApplicationCore>
    {
        #region Константы
        public const int CategoryType = 1;
        public const int ItemType = 2;

        public const int ItemsTypeCategory = 1;
        public const int ItemsTypeItem = 2;

        /// <summary>
        /// Обозначает ключ разрешения для сохранения настроек модуля.
        /// </summary>
        public static readonly Guid PermissionSaveConfiguration = "perm_configSave".GenerateGuid();

        #endregion

        private Guid _moduleBaseID = Guid.Empty;
        internal int _moduleId;
        internal string _moduleCaption;
        internal string _moduleUrlName;

        private List<Extensions.ModuleExtension> _extensions = new List<Extensions.ModuleExtension>();
        private ConcurrentDictionary<int, int> _journalForCurrentModule = new ConcurrentDictionary<int, int>();

        /// <summary>
        /// Создает новый экземпляр класса.
        /// </summary>
        [Obsolete("Следует использовать ModuleCore<TSelfReference>")]
        internal ModuleCore()
        {
        }

        #region Инициализация и остановка
        internal virtual void InitModule(IEnumerable<Type> controllerTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Определение дополнительных ресурсов модуля. Вызывается во время инициализации.
        /// </summary>
        protected virtual void InitModuleCustom()
        {

        }
        #endregion

        #region Разрешения
        /// <summary>
        /// Регистрация разрешения для системы доступа. Если разрешение с таким ключом <paramref name="key"/> уже существует, оно будет перезаписано новым.
        /// </summary>
        /// <param name="key">См. <see cref="Permission.Key"/>.</param>
        /// <param name="caption">См. <see cref="Permission.Caption"/>.</param>
        /// <param name="description">См. <see cref="Permission.Description"/>.</param>
        /// <param name="ignoreSuperuser">См. <see cref="Permission.IgnoreSuperuser"/>.</param>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="caption"/> является пустой строкой или null.</exception>
        public void RegisterPermission(string key, string caption, string description = null, bool ignoreSuperuser = false)
        {
            RegisterPermission(string.IsNullOrEmpty(key) ? Guid.Empty : key.GenerateGuid(), caption, description, ignoreSuperuser);
        }

        /// <summary>
        /// Проверяет, доступно ли указанное разрешение <paramref name="key"/> пользователю, ассоциированному с текущим контекстом (см. <see cref="OnUtils.Application.Users.UserContextManager{TApplication}.GetCurrentUserContext"/>).
        /// </summary>
        /// <param name="key">Уникальный ключ разрешения. См. <see cref="Permission.Key"/>.</param>
        /// <returns>Возвращает результат проверки.</returns>
        public CheckPermissionResult CheckPermission(string key)
        {
            return CheckPermission(AppCore.GetUserContextManager().GetCurrentUserContext(), key.GenerateGuid());
        }

        #endregion

        #region Информация о модуле
        /// <summary>
        /// Возвращает идентификатор модуля <see cref="ID"/>, представленный в виде GUID.
        /// </summary>
        public override Guid ModuleID { get;}

        /// <summary>
        /// Возвращает идентификатор модуля в базе данных.
        /// </summary>
        public int ID
        {
            get => _moduleId;
            internal set
            {
                _moduleId = value;
                _moduleBaseID = GuidIdentifierGenerator.GenerateGuid(GuidType.Module, value);
            }
        }

        /// <summary>
        /// Возвращает url-доступное название модуля. Не может быть пустым.
        /// Порядок определения значения свойства следующий:
        /// 1) Если задано - <see cref="ModuleCoreAttribute.DefaultUrlName"/>;
        /// 2) Если задано - <see cref="Configuration.ModuleConfiguration{TModule}.UrlName"/>;
        /// 3) Если предыдущие пункты не вернули значения - используется результат выполнения <see cref="StringExtension.GenerateGuid(string)"/> на основе полного имени (<see cref="Type.FullName"/>) query-типа модуля.
        /// </summary>
        /// <seealso cref="ModuleCoreAttribute.DefaultUrlName"/>
        /// <seealso cref="Configuration.ModuleConfiguration{TModule}.UrlName"/>
        public virtual string UrlName
        {
            get => _moduleUrlName;
        }

        /// <summary>
        /// Возвращает отображаемое название модуля.
        /// </summary>
        public string Caption
        {
            get => _moduleCaption;
        }

        /// <summary>
        /// Список типов контроллеров, относящихся к модулю.
        /// </summary>
        public Dictionary<int, Type> ControllerTypes
        {
            get;
            internal set;
        }

        /// <summary>
        /// Возвращает идентификатор модуля.
        /// </summary>
        public int IdModule
        {
            get => ID;
        }
        #endregion

        #region Блок расширений
        public void registerExtensionNeeded<T>() where T : Extensions.ModuleExtension
        {
            if (!hasExtension<T>())
            {
                Type _type = null;

                LibraryEnumeratorFactory.Enumerate((assembly) =>
                {
                    if (_type == null)
                    {
                        var t = assembly.GetType(typeof(T).FullName, false);
                        if (t != null) _type = t;
                    }
                });

                if (_type != null)
                {
                    var extension = Activator.CreateInstance(_type, new object[] { this }) as Extensions.ModuleExtension;
                    _extensions.Add(extension);
                }
            }
        }

        public bool hasExtension<T>(bool IsForAdmin = false) where T : Extensions.ModuleExtension
        {
            var tt = typeof(T);
            var extension = (from p in _extensions
                             where (p as T) != null && p.Attributes.IsAdminPart == IsForAdmin
                             select p).FirstOrDefault();

            return (extension != null);
        }


        public T getExtension<T>(bool IsForAdmin = false) where T : Extensions.ModuleExtension
        {
            var tt = typeof(T);
            var extension = (from p in _extensions
                             where (p as T) != null && p.Attributes.IsAdminPart == IsForAdmin
                             select p).FirstOrDefault();

            return (T)extension;
        }

        /// <summary>
        /// Возвращает список подключенных расширений.
        /// </summary>
        public List<Extensions.ModuleExtension> GetExtensions()
        {
            return _extensions;
        }

        /////**
        ////* Возвращает строку с текущими ошибками расширений. 
        ////* 
        ////* @return string
        ////*/
        ////protected function extensionsGetErrors()
        ////{
        ////    $errors = array();
        ////    foreach ( $this.mExtensions as $k=>$v) 
        ////    {
        ////        if (strlen($v.Error) > 0) $errors[] = $v.Error;
        ////    }

        ////    return implode("; ", $errors);
        ////}
        #endregion

        #region Блок функций, переопределение которых может потребоваться для расширений и других модулей
        /// <summary>
        /// Возвращает список типов объектов, используемых в модуле.
        /// По-умолчанию возвращает <see cref="Items.ItemTypeFactory.ItemType"/> и <see cref="Items.ItemTypeFactory.CategoryType"/>.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<DB.ItemType> GetItemTypes()
        {
            return new DB.ItemType[] { Items.ItemTypeFactory.ItemType, Items.ItemTypeFactory.CategoryType };
        }

        /// <summary>
        /// Возвращает список идентификатор=название указанного типа для текущего модуля. Например, это может быть список категорий.
        /// </summary>
        /// <param name="IdItemType"></param>
        /// <param name="SortOrder"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        public virtual Types.NestedLinkCollection GetItems(int IdItemType, Types.eSortType SortOrder = Types.eSortType.Default, params object[] _params)
        {
            return null;
            //throw new NotImplementedException(string.Format("Функция 'getItemsList' не переопределена в производном классе '{0}'.", this.GetType().FullName));
        }

        /// <summary>
        /// Возвращает ссылку для переданного объекта.
        /// Вызывается в случае, когда для объекта не был найден адрес в системе маршрутизации по ключу <see cref="Routing.RoutingConstants.MAINKEY"/>.
        /// </summary>
        /// <returns></returns>
        public virtual Uri GenerateLink(Items.ItemBase item)
        {
            throw new NotImplementedException(string.Format("Метод 'GenerateLink' класса '{0}' не определен в производном классе '{1}'", typeof(ModuleCore).FullName, this.GetType().FullName));
        }

        /// <summary>
        /// Возвращает ссылку для переданного объекта.
        /// Вызывается для объектов, для которых не был найден адрес в системе маршрутизации по ключу <see cref="Routing.RoutingConstants.MAINKEY"/>.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyDictionary<Items.ItemBase, Uri> GenerateLinks(IEnumerable<Items.ItemBase> items)
        {
            return items.ToDictionary(x => x, x => GenerateLink(x));
        }

        /// <summary>
        /// Уничтожает и выгружает модуль.
        /// </summary>
        public virtual void Dispose()
        {

        }
        #endregion

        #region Расширения
        /// <summary>
        /// Предоставляет доступ к расширению настраиваемых полей.
        /// </summary>
        public ModuleExtensions.CustomFields.ExtensionCustomsFieldsBase Fields
        {
            get { return getExtension<ModuleExtensions.CustomFields.ExtensionCustomsFieldsBase>(); }
        }

        /// <summary>
        /// Предоставляет доступ к расширению ЧПУ.
        /// </summary>
        internal ModuleExtensions.ExtensionUrl.ExtensionUrl Urls
        {
            get { return getExtension<ModuleExtensions.ExtensionUrl.ExtensionUrl>(); }
        }
        #endregion

        #region Ошибки 
        private int GetJournalForErrors()
        {
            var result = AppCore.Get<Journaling.IManager>().RegisterJournal(Journaling.JournalingConstants.IdSystemJournalType, "Журнал событий модуля '" + this.Caption + "'", "ModuleErrors_" + ID);
            if (!result.IsSuccess) Debug.WriteLine("Ошибка получения журнала событий модуля '{0}': {1}", this.Caption, result.Message);
            return result.Result?.IdJournal ?? -1;
        }

        /// <summary>
        /// Регистрирует событие в журнал модуля.
        /// </summary>
        /// <param name="eventType">См. <see cref="Journaling.IManager.RegisterEvent(int, Journaling.EventType, string, string, DateTime?, Exception)"/>.</param>
        /// <param name="message">См. <see cref="Journaling.IManager.RegisterEvent(int, Journaling.EventType, string, string, DateTime?, Exception)"/>.</param>
        /// <param name="messageDetailed">См. <see cref="Journaling.IManager.RegisterEvent(int, Journaling.EventType, string, string, DateTime?, Exception)"/>.</param>
        /// <param name="ex">См. <see cref="Journaling.IManager.RegisterEvent(int, Journaling.EventType, string, string, DateTime?, Exception)"/>.</param>
        protected internal void RegisterEvent(Journaling.EventType eventType, string message, string messageDetailed = null, Exception ex = null)
        {
            var idJournal = _journalForCurrentModule.GetOrAddWithExpiration(0, c => GetJournalForErrors(), TimeSpan.FromMinutes(5));

            var msg = "";
            // todo придумать, как получать эти данные. возможно, объявить интерфейсы.
            //var Request = HttpContext.Current != null && HttpContext.Current.Request != null ? HttpContext.Current.Request : null;

            //if (Request != null)
            //{ 
            //    msg += $"URL запроса: {Request.Url}\r\n";
            //    if (Request.UrlReferrer != null) msg += $"URL-referer: {Request.UrlReferrer}\r\n";
            //}

            //if (UserManager.Instance != null)
            //{
            //    if (!!AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest) msg += $"Пользователь: Гость\r\n";
            //    else msg += $"Пользователь: {AppCore.GetUserContextManager().GetCurrentUserContext().getData().ToString()} (id: {AppCore.GetUserContextManager().GetCurrentUserContext().ID})\r\n";
            //}

            //if (Request != null)
            //{
            //    if (!string.IsNullOrEmpty(Request.UserAgent)) msg += $"User-agent: {Request.UserAgent}\r\n";
            //    var ipdns = new Dictionary<string, string>() { { "IP", Request.UserHostAddress }, { "DNS", Request.UserHostName } }.Where(x => !string.IsNullOrEmpty(x.Value)).ToList();
            //    if (ipdns.Count > 0) msg += $"User {string.Join(" / ", ipdns.Select(x => x.Key))}: {string.Join("/", ipdns.Select(x => x.Value))}\r\n";
            //}

            msg += messageDetailed;

            AppCore.Get<Journaling.IManager>().RegisterEvent(idJournal, eventType, message, msg, null, ex);
        }

        #endregion

    }

    /// <summary>
    /// Базовый класс для всех модулей. Обязателен при реализации любых модулей, т.к. при задании привязок в DI проверяется наследование именно от этого класса.
    /// </summary>
    /// <typeparam name="TSelfReference">Должен ссылаться сам на себя.</typeparam>
    public abstract class ModuleCore<TSelfReference> : ModuleCore
        where TSelfReference : ModuleCore<TSelfReference>
    {
        internal Configuration.ModuleConfigurationManipulator<TSelfReference> _configurationManipulator = null;

        /// <summary>
        /// Создает новый объект модуля. 
        /// </summary>
#pragma warning disable CS0618
        public ModuleCore()
#pragma warning restore CS0618
        {
            if (!typeof(TSelfReference).IsAssignableFrom(this.GetType())) throw new TypeAccessException($"Параметр-тип {nameof(TSelfReference)} должен находиться в цепочке наследования текущего типа.");
        }

        /// <summary>
        /// Возвращает объект типа <typeparamref name="TConfiguration"/>, содержащий параметры модуля.
        /// Возвращенный объект находится в режиме "только для чтения" - изменение параметров невозможно, попытка выполнить set вызовет <see cref="InvalidOperationException"/>.
        /// Все объекты конфигурации, созданные путем вызова этого метода, манипулируют одним набором значений. 
        /// </summary>
        /// <exception cref="InvalidOperationException">Возникает, если модуль не зарегистрирован.</exception>
        /// <seealso cref="Configuration.ModuleConfigurationManipulator{TModule}"/>
        /// <seealso cref="GetConfigurationManipulator"/>
        public TConfiguration GetConfiguration<TConfiguration>() 
            where TConfiguration : Configuration.ModuleConfiguration<TSelfReference>, new()
        {
            return _configurationManipulator.GetUsable<TConfiguration>();
        }

        /// <summary>
        /// Возвращает манипулятор конфигурацией, предоставляющий возможности получения и редактирования конфигурации.
        /// </summary>
        public Configuration.ModuleConfigurationManipulator<TSelfReference> GetConfigurationManipulator()
        {
            return _configurationManipulator;
        }

        internal override void InitModule(IEnumerable<Type> controllerTypes)
        {
            RegisterPermission(PermissionSaveConfiguration, "Сохранение настроек модуля");
            RegisterPermission(ModulesConstants.PermissionManage, "Управление модулем");

            if (controllerTypes != null)
            {
                var controllerTypesSplitIntoTypes = controllerTypes.
                    Select(x => new { Type = x, Attribute = x.GetCustomAttribute<ModuleControllerAttribute>() }).
                    Where(x => x.Attribute != null).
                    GroupBy(x => x.Attribute.ControllerTypeID, x => x.Type).
                    Select(x => new { ControllerTypeID = x.Key, ControllerType = x.Last() }).
                    ToList();

                ControllerTypes = controllerTypesSplitIntoTypes.ToDictionary(x => x.ControllerTypeID, x => x.ControllerType);
            }
            else
            {
                ControllerTypes = new Dictionary<int, Type>();
            }

            InitModuleCustom();
            //RegisterAction("extensionsGetData");
        }

        /// <summary>
        /// Возвращает url-доступное название модуля. Не может быть пустым.
        /// Порядок определения значения свойства следующий:
        /// 1) Если задано - <see cref="ModuleCoreAttribute.DefaultUrlName"/>;
        /// 2) Если задано - <see cref="Configuration.ModuleConfiguration{TModule}.UrlName"/>;
        /// 3) Если предыдущие пункты не вернули значения - используется результат выполнения <see cref="StringExtension.GenerateGuid(string)"/> на основе полного имени (<see cref="Type.FullName"/>) query-типа модуля.
        /// </summary>
        /// <seealso cref="ModuleCoreAttribute.DefaultUrlName"/>
        /// <seealso cref="Configuration.ModuleConfiguration{TModule}.UrlName"/>
        public override string UrlName
        {
            get
            {
                if (string.IsNullOrEmpty(_moduleUrlName)) _moduleUrlName = typeof(TSelfReference).FullName.GenerateGuid().ToString();
                return _moduleUrlName;
            }
        }
    }
}