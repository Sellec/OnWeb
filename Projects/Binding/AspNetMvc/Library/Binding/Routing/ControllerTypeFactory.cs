using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Binding.Routing
{
    /// <summary>
    /// Предоставляет методы для работы с типами контроллеров.
    /// </summary>
    public static class ControllerTypeFactory
    {
        private static List<ControllerType> _controllerTypes = new List<ControllerType>();

        static ControllerTypeFactory()
        {
        }

        public static void Add(ControllerType controllerType)
        {
            _controllerTypes.AddIfNotExists(controllerType);
        }

        /// <summary>
        /// Возвращает тип контроллера, который соответствует текущему запросу <paramref name="request"/> со строкой адреса <paramref name="relativeURL"/>.
        /// </summary>
        public static ControllerType RoutingPrepareURL(HttpRequestBase request, string relativeURL)
        {
            ControllerType type = null;
            foreach (var controllerType in _controllerTypes)
            {
                if (controllerType.IsThisRequestIsThisControllerType(request, relativeURL))
                {
                    type = controllerType;
                }
            }

            return type;
        }

        /// <summary>
        /// Возвращает список поддерживаемых типов контроллеров.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ControllerType> GetControllerTypes()
        {
            return _controllerTypes.ToList();
        }

    }
}
