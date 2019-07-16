using OnUtils.Application.Modules;
using OnUtils.Architecture.AppCore;
using OnUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using Journaling = OnUtils.Application.Journaling;

namespace OnWeb.CoreBind.Routing
{
    using Core;
    using Core.Modules;
    using Core.ServiceMonitor;
    using CoreBind.Modules;

    /// <summary>
    /// Предоставляет функции для работы с маршрутизацией.
    /// </summary>
    public class RoutingManager : CoreComponentBase, IComponentSingleton, IAutoStart, IMonitoredService
    {
        private Guid _serviceID = StringsHelper.GenerateGuid(nameof(RoutingManager));

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
            var controllerTypes = new List<ControllerType>() { new ControllerTypeDefault(), new ControllerTypeAdmin() };
            controllerTypes.ForEach(x => x.Start(AppCore));
            controllerTypes.ForEach(x => ControllerTypeFactory.Add(x));
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion

        /// <summary>
        /// Возвращает относительный или абсолютный url (см. <paramref name="includeAuthority"/>) на основе контроллера с типом <typeparamref name="TModuleController"/> и метода, вызванного в лямбда-выражении <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="TModule">Тип модуля.</typeparam>
        /// <typeparam name="TModuleController">Тип контроллера, относящийся к модулю <typeparamref name="TModule"/>. Проверяются зарегистрированные типы контроллеров (<see cref="ModuleCore.ControllerTypes"/>) в модуле <typeparamref name="TModule"/> и проверяется нахождение типа <typeparamref name="TModuleController"/> в цепочках наследования.</typeparam>
        /// <param name="expression">Выражение, содержащее вызов метода контроллера, к которому следует построить маршрут. Все аргументы вызываемого метода должны быть указаны. Если аргумент указывается как null, то он игнорируется. Если аргумент задан явно, то он передается в адресе.</param>
        /// <param name="includeAuthority">Если равно true, то формируется абсолютный url, включающий в себя адрес сервера.</param>
        /// <returns>Возвращает сформированный url или генерирует исключение.</returns>
        /// <exception cref="ArgumentException">Исключение возникает, если в выражении <paramref name="expression"/> нет вызова метода или вызван метод, не относящийся к <typeparamref name="TModuleController"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// <para>Исключение возникает в ряде случаев: </para>
        /// <para>1) Тип контроллера не наследуется от <see cref="ModuleControllerUser{TModule}"/>/<see cref="ModuleControllerAdmin{TModule}"/>);</para>
        /// <para>2) Модуль с типом <typeparamref name="TModule"/> не найден в числе активных модулей;</para>
        /// <para>3) В списке контроллеров модуля <typeparamref name="TModule"/> не найден контроллер, тип которого наследуется или равен <typeparamref name="TModuleController"/>. Подразумевается, что вызов CreateRoute должен найти определенный action среди тех контроллеров модуля, которые будут откликаться на обработчик http-запроса. Если же передается контроллер, который не попадает в обработчик http-запроса, то нет смысла пытаться сформировать для него url;</para>
        /// <para>4) В списке контроллеров модуля <typeparamref name="TModule"/> найдено несколько контроллеров (скорее всего, пользовательский и администрирование), в цепочке наследования которых находится <typeparamref name="TModuleController"/>. Подразумевается, что невозможно определить, на основе каких правил формировать url - для администрирования подставляются префиксы /admin, для пользовательской части url формируется "как есть".</para>
        /// <para>5) Найден подходящий тип контроллера в модуле <typeparamref name="TModule"/>, однако на данный момент тип этого контроллера не поддерживается в <see cref="ControllerTypeFactory"/>, т.е. для этого контроллера нет подходящего обработчика http-запросов.</para>
        /// </exception>
        /// <exception cref="HandledException">Исключение возникает, если было сгенерировано любое другое нештатное исключение во время работы модуля.</exception>
        public Uri CreateRoute<TModule, TModuleController>(Expression<Func<TModuleController, ActionResult>> expression, bool includeAuthority = false)
            where TModule : ModuleCore<TModule>
            where TModuleController : IModuleController<TModule>
        {
            try
            {
                if (expression.Body.NodeType != ExpressionType.Call) throw new HandledException("", new ArgumentException("Выражение должно содержать вызов метода.", nameof(expression)));

                var module = AppCore.Get<TModule>();
                if (module == null) throw new HandledException("", new NotSupportedException($"Модуль типа '{typeof(TModule).FullName}' не найден среди активных модулей."));

                var controllerType = typeof(TModuleController);
                if (!controllerType.IsAssignableToGenericType(typeof(ModuleControllerUser<>)) && !controllerType.IsAssignableToGenericType(typeof(ModuleControllerUser<>)))
                    throw new HandledException("", new NotSupportedException($"Передаваемый тип контроллера должен быть наследником универсального типа {typeof(ModuleControllerUser<>).FullName} или {typeof(ModuleControllerAdmin<>).FullName}."));

                var controllerTypes = AppCore.Get<ModuleControllerTypesManager>().GetModuleControllerTypes<TModule>();

                var controllerTypeCandidates = controllerTypes.Where(x => controllerType.IsAssignableFrom(x.Value)).ToList();
                if (controllerTypeCandidates.Count == 0) throw new HandledException("", new NotSupportedException($"Среди контроллеров модуля '{module.Caption}' нет контроллера, в цепочке наследования которого находится тип '{controllerType.FullName}'."));
                else if (controllerTypeCandidates.Count > 1) throw new HandledException("", new NotSupportedException($"Среди контроллеров модуля '{module.Caption}' есть несколько контроллеров, в цепочке наследования котороых находится тип '{controllerType.FullName}'. Используйте в качестве {nameof(TModuleController)} другой тип, точно относящийся к одному из контроллеров, либо уменьшите общность наследования контроллеров модуля."));

                var controllerTypeCandidate = controllerTypeCandidates.First();
                var controllerTypeFromFactory = ControllerTypeFactory.GetControllerTypes().Where(x => x.ControllerTypeID == controllerTypeCandidate.Key).FirstOrDefault();
                if (controllerTypeFromFactory == null) throw new HandledException("", new NotSupportedException($"Контроллер найден, но тип не поддерживается. См. '{typeof(ControllerTypeFactory).FullName}'."));

                var methodCall = (expression.Body as MethodCallExpression);
                if (!methodCall.Method.DeclaringType.IsAssignableFrom(controllerType)) throw new HandledException("", new ArgumentException("Выражение должно вызывать метод указаного контроллера.", nameof(expression)));

                var methodName = methodCall.Method.Name;

                var possibleActionAttribute = methodCall.Method.GetCustomAttributes<ModuleActionAttribute>(true).FirstOrDefault();
                if (possibleActionAttribute != null && !string.IsNullOrEmpty(possibleActionAttribute.Alias)) methodName = possibleActionAttribute.Alias;

                var arguments = methodCall.Arguments.Select(x =>
                {
                    LambdaExpression lambda = Expression.Lambda(x);
                    var compiledExpression = lambda.Compile();
                    var value = compiledExpression.DynamicInvoke();
                    return $"{value}";
                }).ToList();

                var defaultMethod = methodCall.Method.DeclaringType.GetMethod(nameof(ModuleControllerUser<Plugins.WebCoreModule.WebCoreModule>.Index), BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
                var isDefaultMethod = defaultMethod == methodCall.Method;
                if (isDefaultMethod && arguments.Count == 0) methodName = null;

                var url = controllerTypeFromFactory.CreateRelativeUrl(module.UrlName, methodName, arguments.ToArray());
                return includeAuthority ? new Uri(AppCore.ServerUrl, url) : new Uri(url, UriKind.Relative);
            }
            catch (HandledException ex)
            {
                this.RegisterServiceEvent(Journaling.EventType.Error, "Ошибка создания переадресации", null, ex.InnerException);
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                this.RegisterServiceEvent(Journaling.EventType.Error, "Ошибка создания переадресации", null, ex);
                throw new HandledException("Ошибка переадресации", ex);
            }
        }

        #region ServiceMonitor.IMonitoredService
        Guid IMonitoredService.ServiceID
        {
            get => _serviceID;
        }

        string IMonitoredService.ServiceName
        {
            get => "Менеджер адресов КЛАДР/ФИАС";
        }

        string IMonitoredService.ServiceStatusDetailed
        {
            get => string.Empty;
        }

        bool IMonitoredService.IsSupportsCurrentStatusInfo
        {
            get => false;
        }

        ServiceStatus IMonitoredService.ServiceStatus
        {
            get => throw new NotImplementedException();
        }

        #endregion



    }
}

