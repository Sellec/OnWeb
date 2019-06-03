using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System.Linq;

namespace OnWeb.Core.Modules.Extensions
{
    using Core.Modules;
    using Core.Routing;

#pragma warning disable CS1591 // todo внести комментарии.
    public class ModuleExtension : CoreComponentBase<ApplicationCore>
    {
        public      string _mModuleName;
        public      int _mModuleID = 0;
        public      string _mCheckDataIDObj = null;
        public      string _mCheckFuncName = "";
        public      string _GetDataFuncName = "";

        public      int ModuleID_Override = 0;

        protected ModuleCore Module = null;
        // todo internal ModuleController Controller = null;
        public object ControllerBase = null;
        public object ModelStateBase = null;

        protected object DataPostResult = null;
    
        public ModuleExtension()
        {
        }
    
        public ModuleExtension(ModuleCore moduleObject)
        {
            this.Module = moduleObject;

            var attrs = this.GetType().GetCustomAttributes(typeof(ModuleExtensionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                var attr = attrs.FirstOrDefault() as ModuleExtensionAttribute;
                if (attr != null)
                {
                    Attributes = attr;
                }
            }
        
            this._initializeCustoms();
        }

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion

        protected virtual void _initializeCustoms()
        {
        }

        #region Property
        //*
        // * 
        // * */
        //protected System.Web.HttpContext Context
        //{
        //    get { return System.Web.HttpContext.Current; }
        //}

        /*
         * Атрибуты текущего расширения
         * */
        public ModuleExtensionAttribute Attributes
        {
            get;
            private set;
        } = new ModuleExtensionAttribute("", false);

        ///// <summary>
        ///// См. <see cref="Controller.Request"/>.
        ///// </summary>
        //public HttpRequestBase Request
        //{
        //    get { return this.Controller?.Request; }
        //}


        ///// <summary>
        ///// Возвращает объект <see cref="Controller.ModelState"/> для выполняемого контроллера. 
        ///// Если попытка обратиться к свойству производится не в контексте запроса, то может вернуть null.
        ///// </summary>
        //protected ModelStateDictionary ModelState
        //{
        //    get { return Controller?.ModelState; }
        //}


        #endregion

        /////*
        //// * Устанавливаем ссылку на объект, который будет искать запись с id в базе.
        //// * */
        ////public void _setDataIDHandler()
        ////{
        ////    if (!is_object($handler) || !method_exists($handler,$this._mCheckFuncName)) return false;
        ////    $this._mCheckDataIDObj = $handler;
        ////}

        /////*
        ////Проверка существования записи с таким id в базе
        ////*/
        ////public function _checkDataID($id=0)
        ////{
        ////    if ($this._mCheckDataIDObj != NULL) 
        ////    {
        ////        return call_user_func_array(array($this._mCheckDataIDObj, $this._mCheckFuncName), array($id));
        ////    }
        ////    if (method_exists($this.Module, "_checkDataID")) 
        ////    {
        ////        return call_user_func_array(array($this.Module, "_checkDataID"), array($id));
        ////    }
        ////    return "Не удалось найти объект в базе!";
        ////}

        public virtual Types.NestedLinkCollection getAdminMenu()
        {
            return null;
        }

        //todo ActionResult display
        //protected ActionResult display(string template, object model = null)
        //{
        //    return this.Controller.display(template, model);
        //}

        //protected void assign(string name, object value)
        //{
        //    this.Controller.assign(name, value);
        //}

        //#region Json
        ///// <summary>
        ///// См. <see cref="ModuleController.ReturnJson{TData}(Types.ResultWData{TData})"/> 
        ///// </summary>
        //protected JsonResult ReturnJson<TData>(Types.ResultWData<TData> result)
        //{
        //    return Controller.ReturnJson<TData>(result);
        //}

        ///// <summary>
        ///// См. <see cref="ModuleController.ReturnJson(Types.ResultWData{Object})"/> 
        ///// </summary>
        //protected JsonResult ReturnJson(bool success, string message, object data = null)
        //{
        //    return ReturnJson<object>(new Types.ResultWData<object>(success, message, data));
        //}
        //#endregion

        protected int getModuleID()
        {
            return this.ModuleID_Override > 0 ? this.ModuleID_Override : this.Module.ID;
        }

    }

    public class ModuleExtension<TContextType> : ModuleExtension where TContextType : UnitOfWorkBase, new()
    {
        /// <summary>
        /// Создает новый экземпляр объекта <see cref="ModuleExtension"/>.
        /// </summary>
        public ModuleExtension()
        {

        }

        /// <summary>
        /// Создает новый экземпляр объекта <see cref="ModuleExtension"/>.
        /// </summary>
        public ModuleExtension(ModuleCore moduleObject) : base(moduleObject)
        {
        }

        protected TContextType CreateContext()
        {
            return new TContextType();
        }
    }

}
