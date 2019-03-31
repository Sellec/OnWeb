using OnUtils.Application.Modules;
using OnUtils.Architecture.AppCore;
using OnUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Routing
{
    using Core;
    using Core.Modules;
    using Core.ServiceMonitor;
    using CoreBind.Modules;
    using Journaling = Core.Journaling;

    /// <summary>
    /// Предоставляет функции для работы с маршрутизацией.
    /// </summary>
    public class Manager : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IAutoStart, IMonitoredService
    {
        private Guid _serviceID = StringsHelper.GenerateGuid(nameof(Manager));

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
        /// Создает url на основе контроллера с типом <typeparamref name="TModuleController"/> и метода, вызванного в лямбда-выражении <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="TModuleController">Тип контроллера одного из модулей. На основе типа контроллера определяется модуль, к которому относится контроллер (на основе универсального параметра TModule типа <see cref="ModuleControllerBase"/>/<see cref="ModuleControllerBase"/>). Затем проверяются зарегистрированные типы контроллеров в определенном модуле (<see cref="ModuleCore.ControllerTypes"/>) и проверяется нахождение типа <typeparamref name="TModuleController"/> в цепочках наследования.</typeparam>
        /// <param name="expression">Выражение с вызовом метода контроллера, на основе которого будет определена часть url.</param>
        /// <returns>Возвращает сформированный url или генерирует исключение.</returns>
        /// <exception cref="ArgumentException">Исключение возникает, если в выражении <paramref name="expression"/> нет вызова метода или вызван метод, не относящийся к <typeparamref name="TModuleController"/>.</exception>
        /// <exception cref="NotSupportedException">
        /// <para>Исключение возникает в ряде случаев: </para>
        /// <para>1) Тип контроллера не наследуется от <see cref="ModuleControllerBase"/>/<see cref="ModuleControllerBase"/>);</para>
        /// <para>2) Модуль с типом {TModule} из сигнатуры <typeparamref name="TModuleController"/> не найден в числе модулей;</para>
        /// <para>3) В списке контроллеров модуля {TModule} не найден контроллер, тип которого наследуется или равен <typeparamref name="TModuleController"/>. Подразумевается, что вызов CreateRoute должен найти определенный action среди тех контроллеров модуля, которые будут откликаться на обработчик http-запроса. Если же передается контроллер, который не попадает в обработчик http-запроса, то нет смысла пытаться сформировать для него url;</para>
        /// <para>4) В списке контроллеров модуля {TModule} найдено несколько контроллеров (скорее всего, пользовательский и администрирование), в цепочке наследования которых находится <typeparamref name="TModuleController"/>. Подразумевается, что невозможно определить, на основе каких правил формировать url - для администрирования подставляются префиксы /admin, для пользовательской части url формируется "как есть".</para>
        /// <para>5) Найден подходящий тип контроллера в модуле {TModule}, однако на данный момент тип этого контроллера не поддерживается в <see cref="Routing.ControllerTypeFactory"/>, т.е. для этого контроллера нет подходящего обработчика http-запросов.</para>
        /// </exception>
        /// <exception cref="HandledException">Исключение возникает, если было сгенерировано любое другое нештатное исключение во время работы модуля.</exception>
        public string CreateRoute<TModuleController>(Expression<Func<TModuleController, ActionResult>> expression) where TModuleController : ModuleControllerBase
        {
            try
            {
                if (expression.Body.NodeType != ExpressionType.Call) throw new HandledException("", new ArgumentException("Выражение должно содержать вызов метода.", nameof(expression)));

                var controllerType = typeof(TModuleController);
                Type moduleType;
                if (controllerType.IsAssignableToGenericType(typeof(ModuleControllerUser<>)))
                {
                    var genericType = OnUtils.Types.TypeHelpers.ExtractGenericType(controllerType, typeof(ModuleControllerUser<>));
                    moduleType = genericType.GenericTypeArguments[0];
                }
                else if (controllerType.IsAssignableToGenericType(typeof(ModuleControllerUser<,>)))
                {
                    var genericType = OnUtils.Types.TypeHelpers.ExtractGenericType(controllerType, typeof(ModuleControllerUser<,>));
                    moduleType = genericType.GenericTypeArguments[0];
                }
                else throw new HandledException("", new NotSupportedException("Передаваемый тип контроллера должен быть наследником универсального типа ModuleController<> или ModuleController<,>"));

                var module = AppCore.Get<ModulesManager<ApplicationCore>>().GetModules().Where(x => x.GetType().Equals(moduleType)).FirstOrDefault() as ModuleCore;
                if (module == null)
                {
                    var modulesMethod = typeof(ModulesManager<ApplicationCore>).GetMethod(nameof(ModulesManager<ApplicationCore>.GetModule));
                    if (modulesMethod != null && modulesMethod.IsGenericMethod)
                    {
                        modulesMethod = modulesMethod.MakeGenericMethod(moduleType);
                        module = modulesMethod.Invoke(null, new object[] { true }) as ModuleCore;
                    }
                }
                if (module == null) throw new HandledException("", new NotSupportedException($"Модуль типа '{moduleType.FullName}' не найден среди активных модулей."));

                var controllerTypeCandidates = module.ControllerTypes.Where(x => controllerType.IsAssignableFrom(x.Value)).ToList();
                if (controllerTypeCandidates.Count == 0) throw new HandledException("", new NotSupportedException($"Среди контроллеров модуля '{module.Caption}' нет контроллера, в цепочке наследования которого находится тип '{controllerType.FullName}'."));
                else if (controllerTypeCandidates.Count > 1) throw new HandledException("", new NotSupportedException($"Среди контроллеров модуля '{module.Caption}' есть несколько контроллеров, в цепочке наследования котороых находится тип '{controllerType.FullName}'. Используйте в качестве {nameof(TModuleController)} другой тип, точно относящийся к одному из контроллеров, либо уменьшите общность наследования контроллеров модуля."));

                var controllerTypeCandidate = controllerTypeCandidates.First();
                var controllerTypeFromFactory = Routing.ControllerTypeFactory.GetControllerTypes().Where(x => x.ControllerTypeID == controllerTypeCandidate.Key).FirstOrDefault();
                if (controllerTypeFromFactory == null) throw new HandledException("", new NotSupportedException($"Контроллер найден, но тип не поддерживается. См. '{typeof(Routing.ControllerTypeFactory).FullName}'."));

                var methodCall = (expression.Body as MethodCallExpression);
                if (methodCall.Method.DeclaringType != controllerType) throw new HandledException("", new ArgumentException("Выражение должно вызывать метод указаного контроллера.", nameof(expression)));

                var methodName = methodCall.Method.Name;

                var possibleActionAttribute = methodCall.Method.GetCustomAttribute<ModuleActionAttribute>(true);
                if (possibleActionAttribute != null && !string.IsNullOrEmpty(possibleActionAttribute.Alias)) methodName = possibleActionAttribute.Alias;

                var arguments = methodCall.Arguments.Select(x =>
                {
                    LambdaExpression lambda = Expression.Lambda(x);
                    var compiledExpression = lambda.Compile();
                    var value = compiledExpression.DynamicInvoke();
                    return $"{value}";
                }).ToList();

                var url = controllerTypeFromFactory.CreateRelativeUrl(module.UrlName, methodName, arguments.ToArray());
                return url;
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
            get { return _serviceID; }
        }

        string IMonitoredService.ServiceName
        {
            get { return "Менеджер адресов КЛАДР/ФИАС"; }
        }

        string IMonitoredService.ServiceStatusDetailed
        {
            get { return string.Empty; }
        }

        bool IMonitoredService.IsSupportsCurrentStatusInfo
        {
            get { return false; }
        }

        ServiceStatus IMonitoredService.ServiceStatus
        {
            get { throw new NotImplementedException(); }
        }

        #endregion



    }
}

