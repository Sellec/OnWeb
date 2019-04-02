using System;
using System.Linq;
using System.Web.Mvc;

using OnUtils.Data;

namespace OnWeb.Plugins.Support
{
    using Core.Modules;
    using Core.DB;
    using CoreBind.Modules;
    using CoreBind.Routing;
    using Core.Items;
    using Core.Types;

    public class ModuleController : ModuleControllerUser<Module>
    {
        [ModuleAction("index")]
        public ActionResult Index()
        {
            return TicketNew();
        }

        public ActionResult TicketNew()
        {
            return display("TicketNew.cshtml", new Tickets.Ticket());
        }

        public ActionResult TicketNewJson()
        {
            var answer = JsonAnswer();

            try
            {
                return display("TicketNewInner.cshtml", new Tickets.Ticket());
            }
            catch (Exception ex)
            {
                this.RegisterEventWithCode(System.Net.HttpStatusCode.InternalServerError, "Ошибка открытия формы для нового тикета", ex.Message, ex);
                answer.FromFail("Ошибка открытия формы для нового тикета");
            }

            return ReturnJson(answer);
        }

        public JsonResult TicketSave(Tickets.Ticket model)
        {

            var answer = JsonAnswer<User>();

            try
            {
                //if (!IsReCaptchaValid) throw new Exception(CaptchManager.getError());

                if (ModelState.IsValid)
                {
                    using (var db = new UnitOfWork<User>())
                    {
                        if (!string.IsNullOrEmpty(model.email))
                        {
                            if (db.Repo1.Where(x => x.email.ToLower() == model.email.ToLower()).Count() > 0) ModelState.AddModelError(nameof(model.email), "Этот email-адрес уже используется!");
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    //var preparedData = new Model.PreparedForRegister();
                    //var symbols = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    //var salt = "";

                    //var rand = new Random();
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    var index = rand.Next(0, symbols.Length - 1);
                    //    salt = salt + symbols[index];
                    //}

                    //preparedData.salt = salt;

                    //var regMode = AppCore. (RegisterMode)ApplicationCore.Instance.Config.Get("register_mode", 0);
                    //switch (regMode)
                    //{
                    //    case RegisterMode.SelfConfirmation:
                    //        preparedData.State = TraceWeb.DB.UserState.RegisterNeedConfirmation;
                    //        break;

                    //    case RegisterMode.Immediately:
                    //        preparedData.State = TraceWeb.DB.UserState.Active;
                    //        break;

                    //    case RegisterMode.ManualCheck:
                    //        preparedData.State = TraceWeb.DB.UserState.RegisterWaitForModerate;
                    //        break;
                    //}

                    //preparedData.email = model.email;
                    //preparedData.phone = model.phone;

                    //preparedData.IP_reg = Request.ServerVariables["REMOTE_ADDR"];
                    //preparedData.password = model.password;

                    //preparedData.name = model.name;

                    //preparedData.DateReg = DateTime.Now;
                    //preparedData.Fields.CopyValuesFrom(model.Fields);

                    //preparedData.Superuser = 0;

                    //var result = Module.RegisterUser(preparedData);
                    //answer = result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RegisterSave: {0}", ex.Message);
                answer.FromException(ex);
            }

            return ReturnJson(answer);
        }

    }
}
