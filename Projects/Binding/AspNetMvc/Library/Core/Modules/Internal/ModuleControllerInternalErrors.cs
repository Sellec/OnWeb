﻿using System;
using System.Web.Mvc;

namespace OnWeb.Core.Modules.Internal
{
    using Core.Modules;

    interface IModuleControllerInternalErrors
    {
        void SetException(Exception ex);
    }

    sealed class ModuleControllerInternalErrors<T> : ModuleControllerUser<T>, IModuleControllerInternalErrors where T : ModuleCore<T>
    {
        private Exception _ex = null;

        public override ActionResult Index()
        {
            throw new NotImplementedException();
        }

        public ActionResult PrepareError()
        {
            return base.ErrorHandled(_ex);
        }

        protected override ActionResult ErrorHandled(Exception exception)
        {
            return base.ErrorHandled(_ex);
        }

        public void SetException(Exception ex)
        {
            _ex = ex;
        }
    }
}
