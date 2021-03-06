﻿using OnUtils.Application.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OnWeb.Core.Modules
{
    public class ModuleControllerTypesManager : CoreComponentBase, IModuleRegisteredHandler<WebApplication>
    {
        private static MethodInfo _methodInfo = typeof(ModuleControllerTypesManager).GetMethod(nameof(OnModuleInitialized), BindingFlags.Instance | BindingFlags.NonPublic);
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
        void IModuleRegisteredHandler<WebApplication>.OnModuleInitialized<TModule>(TModule module)
        {
            if (typeof(IModuleCore).IsAssignableFrom(typeof(TModule)))
                _methodInfo.MakeGenericMethod(typeof(TModule)).Invoke(this, new object[] { module });
        }

        private void OnModuleInitialized<TModuleType>(TModuleType module) where TModuleType : ModuleCore<TModuleType>
        {
            var controllerTypes = AppCore.GetBindedTypes<IModuleController<TModuleType>>();

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

        public Dictionary<int, Type> GetModuleControllerTypes<TModuleType>() where TModuleType : ModuleCore<TModuleType>
        {
            return GetModuleControllerTypes(typeof(TModuleType));
        }

        public Dictionary<int, Type> GetModuleControllerTypes(Type moduleType)
        {
            return _moduleControllerTypesList.TryGetValue(moduleType, out var value) ? value : new Dictionary<int, Type>();
        }
    }
}
