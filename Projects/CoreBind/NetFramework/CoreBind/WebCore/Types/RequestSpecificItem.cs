using System;
using System.Collections.Generic;

using System.Web;

namespace OnWeb.CoreBind.WebCore.Types
{
    public class RequestSpecificItem<T> : Lazy<T>, IDisposable where T : new()
    {
        private Action _disposableAction = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposableAction">Обязательно вызывается при уничтожении текущего объекта.</param>
        public RequestSpecificItem(Action disposableAction = null)
        {
            _disposableAction = disposableAction;

            var app = HttpContext.Current.ApplicationInstance as HttpApplicationBase;
            if (app != null)
            {
                app._requestSpecificDisposables.Enqueue(this);
            }
        }

        void IDisposable.Dispose()
        {
            List<Exception> exx = null;

            try
            {
                if (IsValueCreated && Value is IDisposable) (Value as IDisposable).Dispose();
            }
            catch (Exception ex)
            {
                if (exx == null) exx = new List<Exception>();
                exx.Add(ex);
            }

            try
            {
                _disposableAction?.Invoke();
            }
            catch (Exception ex)
            {
                if (exx == null) exx = new List<Exception>();
                exx.Add(ex);
            }

            if (exx != null) throw new AggregateException(exx);
        }
    }

}
