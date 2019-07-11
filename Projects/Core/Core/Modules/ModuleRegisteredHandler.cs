using OnUtils.Application.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OnWeb.Core.Modules
{
    class ModuleRegisteredHandler : CoreComponentBase, IModuleRegisteredHandler
    {
        private ConcurrentDictionary<Type, Dictionary<int, Type>> _moduleControllerTypesList = new ConcurrentDictionary<Type, Dictionary<int, Type>>();

        #region CoreComponentBase
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }
        #endregion

        #region IModuleRegisteredHandler
        void IModuleRegisteredHandler.OnModuleInitialized<TModule>(TModule module)
        {
            var controllerTypes = AppCore.GetBindedTypes<IModuleController<TModule>>();

            if (controllerTypes != null)
            {
                var controllerTypesSplitIntoTypes = controllerTypes.
                    Select(x => new { Type = x, Attribute = x.GetCustomAttribute<ModuleControllerAttribute>() }).
                    Where(x => x.Attribute != null).
                    GroupBy(x => x.Attribute.ControllerTypeID, x => x.Type).
                    Select(x => new { ControllerTypeID = x.Key, ControllerType = x.Last() }).
                    ToList();

                _moduleControllerTypesList[module.QueryType] = controllerTypesSplitIntoTypes.ToDictionary(x => x.ControllerTypeID, x => x.ControllerType);
            }
            else
            {
                _moduleControllerTypesList[module.QueryType] = new Dictionary<int, Type>();
            }

        }
        #endregion
    }
}
