using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace OnWeb._Initialize
{
    using CoreBind.Razor;

    class WebRazorHostFactoryInjector
    {
        public static void LoadIntoHost()
        {
            try
            {
                var hostType = typeof(System.Web.WebPages.Razor.WebRazorHostFactory);
                if (hostType == null)
                {
                    Debug.WriteLine("WebRazorHostFactoryInjector.LoadIntoHost: WebRazorHostFactory not found");
                    return;
                }

                var field = typeof(System.Web.WebPages.Razor.WebRazorHostFactory).GetField("_factories", BindingFlags.Static | BindingFlags.NonPublic);
                if (field == null)
                {
                    Debug.WriteLine("WebRazorHostFactoryInjector.LoadIntoHost: field '_factories' not found");
                    return;
                }
                var fieldValue = field.GetValue(null) as ConcurrentDictionary<string, Func<System.Web.WebPages.Razor.WebRazorHostFactory>>;
                //if (fieldValue.Count > 0) fieldValue[fieldValue.ElementAt(0).Key] = () => new CustomMvcWebRazorHostFactory();
                //else fieldValue.TryAdd("System.Web.Mvc.MvcWebRazorHostFactory, System.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35", () => new CustomMvcWebRazorHostFactory());

                field = hostType.GetField("TypeFactory", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Static);
                if (field == null)
                {
                    Debug.WriteLine("WebRazorHostFactoryInjector.LoadIntoHost: field 'TypeFactory' not found");
                    return;
                }

                if (field == null) Debug.WriteLine("WebRazorHostFactoryInjector.LoadIntoHost: field 'TypeFactory' not found");
                //field.SetValue(null, new Func<string, Type>(s => typeof(CustomMvcWebRazorHostFactory)));

                Debug.WriteLine("WebRazorHostFactoryInjector.LoadIntoHost success");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WebRazorHostFactoryInjector.LoadIntoHost failed: {ex.ToString()}");
            }
        }
    }
}
