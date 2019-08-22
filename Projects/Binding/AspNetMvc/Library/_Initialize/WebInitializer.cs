using Madaa.Lib.Win.Services.Msdtc;
using OnUtils.Application;
using OnUtils.Startup;
using System;
using System.Collections.Generic;

namespace OnWeb._Initialize
{
    class WebInitializer
    {
        public static void Initialize() 
        {
            var IsDeveloperRuntime = StartupFactory.IsDeveloperRuntime(); 

            try
            {

                if (Debug.IsDeveloper) Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: process='{System.Diagnostics.Process.GetCurrentProcess().ProcessName}'");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(OnWeb.NamespaceAnchor).Name}.Start: error {ex.ToString()}");
            }

            try
            {
                Binding.Razor.WebStartup.Initialize();
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
