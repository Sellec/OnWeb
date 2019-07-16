using OnUtils.Application.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace OnWeb.CoreBind.Routing
{
    using Core.Modules;

    /// <summary>
    /// Тип контроллера по-умолчанию. Фактически нужен для того, чтобы любой запрос был обработан контроллером, помеченным атрибутом с таким типом.
    /// Остальные типы контроллеров должны создаваться для специфических запросов.
    /// </summary>
    public class ControllerTypeDefault : ControllerType
    {
        public const int TypeID = 1;

        public ControllerTypeDefault() : base(1, "Контроллер по-умолчанию")
        {
        }

        public override bool IsThisRequestIsThisControllerType(HttpRequestBase request, string relativeURL)
        {
            return true;
        }

        public override string ErrorCannotFindControllerTypeSpecified(IModuleCore module, RouteValueDictionary routeValues)
        {
            return string.Format("Модуль '{0}' не предоставляет доступ в пользовательскую часть.", module.Caption);
        }

        public override bool CheckPermissions(IModuleCore module, RouteValueDictionary routeValues)
        {
            return true;
        }

        public override string CreateRelativeUrl(string moduleName, string actionName, string[] parameters = null)
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(moduleName)) parts.Add(moduleName);
            if (!string.IsNullOrEmpty(actionName)) parts.Add(actionName);
            parts.AddRange(parameters.Where(x => !string.IsNullOrEmpty(x)));

            return $"/{string.Join("/", parts)}";
        }

    }
}
