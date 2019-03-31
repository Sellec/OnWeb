using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using TraceCore.Data;
using TraceCore.Tasks;

namespace OnWeb.Plugins.FileManager
{
    [ModuleCore("FileManager", "Управление файлами")]
    public class Module : ModuleCore2<UnitOfWork<DB.File>>
    {
        internal override void InitModuleImmediately(List<ModuleCoreCandidate> candidatesTypes)
        {
            base.InitModuleImmediately(candidatesTypes);
            this.AutoRegister("fm");

            /**
             * Обслуживание индексов запускаем один раз при старте и раз в несколько часов
             * */
            BackgroundServicesFactory.Instance.Providers.First().SetTask(typeof(Module).FullName + "_" + nameof(MaintenanceIndexes), DateTime.Now.AddSeconds(30), () => MaintenanceIndexesStatic());
            BackgroundServicesFactory.Instance.Providers.First().SetTask(typeof(Module).FullName + "_" + nameof(MaintenanceIndexes) + "_hourly6", Cron.HourInterval(6), () => MaintenanceIndexesStatic());

            /**
             * Прекомпиляция шаблонов при запуске.
             * */
            //if (!Debug.IsDeveloper)
            //    Tasks.BackgroundServicesFactory.Instance.Providers.First().SetTask(typeof(Module).FullName + "_" + nameof(RazorPrecompilationStatic), DateTime.Now.AddMinutes(1), () => RazorPrecompilationStatic());

#if DEBUG
            /**
             * Регулярная сборка мусора для сборки в режиме отладки.
             * */
            BackgroundServicesFactory.Instance.Providers.First().SetTask(typeof(Module).FullName + "_" + nameof(GCCollect) + "_minutely1", Cron.MinuteInterval(1), () => GCCollectStatic());
#endif

            /**
             * Регулярная проверка новых слов в лексическом менеджере.
             * */
            BackgroundServicesFactory.Instance.Providers.First().SetTask(typeof(Lexicon.Manager).FullName + "_" + nameof(Lexicon.Manager.PrepareNewWords) + "_minutely2", Cron.MinuteInterval(2), () => LexiconNewWordsStatic());

            ModelMetadataProviders.Current = new MVC.TraceModelMetadataProviderWithFiles();
        }

        public Dictionary<string, Conversations.ConversationBase> Conversations { get; } = new Dictionary<string, Conversations.ConversationBase>();

        #region Maintenance indexes
        public static void MaintenanceIndexesStatic()
        {
            var module = ModulesManager.getModule<Module>();
            if (module == null) throw new Exception("Модуль не найден.");

            module.MaintenanceIndexes();
        }

        private void MaintenanceIndexes()
        {
            try
            {
                using (var db = CreateContext())
                {
                    var result = db.DataContext.StoredProcedure<object>("Maintenance_RebuildIndexes", new { MinimumIndexFragmentstionToSearch = 5 });
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.CriticalError, $"Ошибка обслуживания индексов", null, ex);
                Debug.WriteLine("FileManager.Module.MaintenanceIndexes: {0}", ex.Message);
            }
        }
        #endregion

        #region Lexicon new words
        public static void LexiconNewWordsStatic()
        {
            Lexicon.Manager.PrepareNewWords();
        }
        #endregion

        #region RazorPrecompilation
        public static void RazorPrecompilationStatic()
        {
            var module = ModulesManager.getModule<Module>();
            if (module == null) throw new Exception("Модуль не найден.");

            module.RazorPrecompilation();
        }

        private void RazorPrecompilation()
        {
            try
            {
                ApplicationCore.Instance.ResourceManager.GeneratePrecompiled();
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.CriticalError, $"Ошибка прекомпиляции шаблонов", null, ex);
                Debug.WriteLine("FileManager.Module.RazorPrecompilation: {0}", ex.Message);
            }
        }
        #endregion

#if DEBUG
        #region GC collect for debug
        public static void GCCollectStatic()
        {
            var module = ModulesManager.getModule<Module>();
            if (module == null) throw new Exception("Модуль не найден.");

            module.GCCollect();
        }

        private void GCCollect()
        {
            GC.Collect();
        }
        #endregion
#endif

    }
}
