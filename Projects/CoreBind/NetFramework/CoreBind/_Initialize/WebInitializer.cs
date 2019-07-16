using Madaa.Lib.Win.Services.Msdtc;
using OnUtils.Application;
using OnUtils.Startup;
using System;
using System.Collections.Generic;

namespace OnWeb.CoreBind._Initialize
{
    class WebInitializer
    {
        public static void Initialize() 
        {
            var IsDeveloperRuntime = StartupFactory.IsDeveloperRuntime(); 

            try
            {

                if (Debug.IsDeveloper) Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: process='{System.Diagnostics.Process.GetCurrentProcess().ProcessName}'");

                if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "Microsoft.VisualStudio.Web.Host")
                {
                    var runtimeVersion = IntPtr.Size == 4 ? "32" : "64";
                    var managerInjectorType = IntPtr.Size == 4 ?
                        Type.GetType("ManagedInjector.Injector, ManagedInjector-x86, Version=1.0.7025.18016, Culture=neutral, PublicKeyToken=8e22adab863b765a", false) :
                        Type.GetType("ManagedInjector.Injector, ManagedInjector-x64, Version=1.0.7025.18014, Culture=neutral, PublicKeyToken=8e22adab863b765a", false);

                    if (Debug.IsDeveloper) Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: managerInjectorType='{managerInjectorType?.ToString()}'");
                    if (managerInjectorType != null) 
                    {
                        var launchMethod = managerInjectorType.GetMethod("Launch");
                        var loadAssemblyMethod = managerInjectorType.GetMethod("LoadAssembly");
                        if (launchMethod != null && loadAssemblyMethod != null)
                        {
                            var utilsInjectorType = typeof(ModuleInjector);
                            var webInjectorType = typeof(WebRazorHostFactoryInjector);

                            var newtonsoftAssembly = typeof(Newtonsoft.Json.JsonSerializer).Assembly;
                            var utilsAssembly = utilsInjectorType.Assembly;
                            var applicationAssembly = typeof(ApplicationCore<>).Assembly;
                            var mvcAssembly = typeof(System.Web.Mvc.ActionResult).Assembly;
                            var webCoreAssembly = typeof(WebApplicationBase).Assembly;
                            var webAssembly = webInjectorType.Assembly;

                            //var webAssemblyReferences = webAssembly.GetReferencedAssemblies().Select(x => System.Reflection.Assembly.Load(x.FullName)).ToList();
                            //try
                            //{
                            //}
                            //catch (Exception ex)
                            //{

                            //}

                            var devEnvProcesses = System.Diagnostics.Process.GetProcessesByName("devenv");
                            foreach (var process in devEnvProcesses)
                            {
                                try
                                {
                                    // launchMethod.Invoke(null, new object[] { process.MainWindowHandle, newtonsoftAssembly.Location, "", "" });
                                    launchMethod.Invoke(null, new object[] { process.MainWindowHandle, utilsAssembly.Location, utilsInjectorType.FullName, nameof(ModuleInjector.InjectorLoader) });
                                    loadAssemblyMethod.Invoke(null, new object[] { process.MainWindowHandle, applicationAssembly.Location });
                                    loadAssemblyMethod.Invoke(null, new object[] { process.MainWindowHandle, mvcAssembly.Location });
                                    //foreach (var webAssemblyReference in webAssemblyReferences)
                                    //{
                                    //    try
                                    //    {
                                    //        loadAssemblyMethod.Invoke(null, new object[] { process.MainWindowHandle, webAssemblyReference.Location });
                                    //    }
                                    //    catch (Exception ex)
                                    //    {
                                    //        Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: process '{process.ProcessName}' (pid {process.Id}), error loading web assembly - '{webAssembly.FullName}'");
                                    //    }
                                    //}
                                    loadAssemblyMethod.Invoke(null, new object[] { process.MainWindowHandle, webCoreAssembly.Location });
                                    launchMethod.Invoke(null, new object[] { process.MainWindowHandle, webAssembly.Location, webInjectorType.FullName, nameof(WebRazorHostFactoryInjector.LoadIntoHost) });
                                    Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: process '{process.ProcessName}' (pid {process.Id}), success");
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(typeof(OnWeb.NamespaceAnchor).Name + ".Start: process '{0}' (pid {1}), error: {2}", process.ProcessName, process.Id, ex.ToString());
                                }
                            }
                        }
                        else if (launchMethod == null && loadAssemblyMethod == null && Debug.IsDeveloper) Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: launch && loadAssemblyMethod method not found");
                        else if (launchMethod == null && Debug.IsDeveloper) Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: launch method not found");
                        else if (loadAssemblyMethod == null && Debug.IsDeveloper) Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: loadAssemblyMethod method not found");
                    }
                    else if (Debug.IsDeveloper) Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: not managerInjectorType");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: error {ex.ToString()}");
            }

            try
            {
                Razor.WebStartup.Initialize();
            }
            catch (Exception ex) { Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.ModuleInitializer Error: {ex.ToString()}"); }

            if (!IsDeveloperRuntime) CheckMSDTC();
        }

        static void CheckMSDTC()
        {
            try
            {
                /*
                 * Проверка MSDTC
                 * */

                var msdtcManager = new MsdtcManager(false, 1000);

                Debug.WriteLine("MSDTC: параметры службы (dcomcnfg). Установлена = {0}. Установлена и запущена = {1}.", msdtcManager.IsServiceInstalled(), msdtcManager.IsServiceInstalledAndRunning());
                Debug.WriteLine("MSDTC: Network DTC. Доступ к сети DTC = {0}. Диспетчер транзакций - разрешить входящие = {1}, разрешить исходящие = {2}", msdtcManager.NetworkDtcAccess, msdtcManager.AllowInbound, msdtcManager.AllowOutbound);
                if (msdtcManager.NetworkDtcAccess == NetworkDTCAccessStatus.Unknown)
                {
                    Debug.WriteLine("MSDTC: Настройки безопасности Network DTC недоступны. Возможно, нет доступа на чтение реестра. Проверьте настройки вручную (Требуется следующее: доступ к сети DTC = Включен; диспетчер транзакций - разрешить входящие = Включен; диспетчер транзакций - разрешить исходящие = Включен)");
                }
                else
                {
                    if (msdtcManager.NetworkDtcAccess != NetworkDTCAccessStatus.On || !msdtcManager.AllowInbound || !msdtcManager.AllowOutbound)
                    {
                        var toCorrect = new List<string>();
                        if (msdtcManager.NetworkDtcAccess != NetworkDTCAccessStatus.On) toCorrect.Add("Доступ к сети DTC - включить");
                        if (!msdtcManager.AllowInbound) toCorrect.Add("Разрешить входящие - включить");
                        if (!msdtcManager.AllowOutbound) toCorrect.Add("Разрешить исходящие - включить");

                        Debug.WriteLine("MSDTC: выполнение корректирующих действий:\r\n - " + string.Join(";\r\n - ", toCorrect) + ".");

                        if (msdtcManager.NetworkDtcAccess != NetworkDTCAccessStatus.On) msdtcManager.NetworkDtcAccess = NetworkDTCAccessStatus.On;
                        if (!msdtcManager.AllowInbound) msdtcManager.AllowInbound = true;
                        if (!msdtcManager.AllowOutbound) msdtcManager.AllowOutbound = true;

                        if (msdtcManager.NeedRestart)
                        {
                            Debug.WriteLine("MSDTC: служба требует перезагрузки, выполняем попытку...");
                            msdtcManager.RestartService();
                            Debug.WriteLine("MSDTC: служба перезагружена.");
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("Web.MSDTC: {0}", ex.Message); }
        }
    }
}
