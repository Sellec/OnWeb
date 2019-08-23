using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.MessageHandlers;
using OnUtils.Architecture.ObjectPool;
using OnWeb.Modules.MessagingEmail;

namespace OnWeb.Site
{
    public class MvcApplication : HttpApplicationBase
    {
        public MvcApplication() 
        {
            
        }

        protected override void OnAfterApplicationStart()
        {
            try
            {
                var physicalApplicationPath = AppCore.ApplicationWorkingFolder;

#if DEBUG
                var paths = new List<string>();

                if (AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("OnWeb.Binding.AspNetMvc,")).Count() > 0)
                    paths.Add(Path.GetFullPath(Path.Combine(physicalApplicationPath, "../../Binding/AspNetMvc/Library")));

                if (AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("OnWeb.Standard.AspNetMvc,")).Count() > 0)
                    paths.Add(Path.GetFullPath(Path.Combine(physicalApplicationPath, "../../Standard/AspNetMvc/Library")));

                AppCore.Get<Core.Storage.ResourceProvider>().SourceDevelopmentPathList.AddRange(paths);
#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine("OnAfterApplicationStart.Development: {0}", ex.Message);
                throw;
            }
        }

        protected override void OnBeginRequest()
        {
            base.OnBeginRequest();
            //AppCore.GetUserContextManager().SetCurrentUserContext(AppCore.GetUserContextManager().CreateGuestUserContext());
           // AppCore.GetUserContextManager().SetCurrentUserContext(AppCore.GetUserContextManager().GetSystemUserContext());
        }

        public interface testinterface
        {
            int tttt { get; set; }

            object test { get; }

            void methodTest();
        }

        //public class mapper : OnUtils.Architecture.InterfaceMapper.MapperBase
        //{
        //    protected override object OnPrepareMethodCall(MethodInfo method, object[] arguments)
        //    {
        //        return null;
        //    }

        //    protected override object OnPreparePropertyGet(PropertyInfo property)
        //    {
        //        if (property.Name == nameof(testinterface.test)) return new List<int>();
        //        else if (property.Name == nameof(testinterface.tttt)) return 111;
        //        return null;
        //    }

        //    protected override void OnPreparePropertySet(PropertyInfo property, object value)
        //    {
                
        //    }
        //}

        protected override void OnBeforeApplicationStart()
        {
            //base.OnBeforeApplicationStart();

            //var ins = OnUtils.Architecture.InterfaceMapper.Mapper.CreateObjectFromInterface<mapper, testinterface>();
            //try
            //{
            //    //ins.methodTest();
            //    //var d = ins.tttt;
            //    ins.tttt = 10;
            //    var dd = ins.test;
            //}
            //catch (Exception ex)
            //{
            //    var d = ex.Message;
            //}
        }

        protected override string ConnectionString
        {
            get
            {
                try
                {
                    var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    if (!Debug.IsDeveloper)
                        connectionString = ConfigurationManager.ConnectionStrings["ServerConnection"].ConnectionString;

                    return connectionString;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("MvcApplication.ConnectionString: " + ex.ToString());
                }

                return "";
            }
        }
    }
}