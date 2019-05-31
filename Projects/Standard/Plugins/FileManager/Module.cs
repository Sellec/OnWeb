using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using OnUtils.Data;
using OnUtils.Tasks;

namespace OnWeb.Plugins.FileManager
{
    using Core.Configuration;
    using Core.DB;
    using Core.Items;
    using Core.Modules;
    using CoreBind.Modules;
    using Core.Types;
    using Core.Modules;
    using Core.Routing;
    using CoreBind.Modules;
    using CoreBind.Routing;
    using Core.DB;
    using Core.Configuration;
    using Core.DB;
    using Core.Items;
    using Core.Modules;
    using CoreBind.Modules;
    using Core.Types;
    using Core.Modules;
    using Core.Routing;
    using CoreBind.Modules;
    using CoreBind.Routing;
    using Core.DB;
    using Core.Journaling;

    [ModuleCore("Управление файлами")]
    public class Module : ModuleCore<Module>, IUnitOfWorkAccessor<UnitOfWork<DB.File>>
    {
        private static Module _thisModule = null;

        protected override void InitModuleCustom()
        {
            _thisModule = this;

            /*
             * Обслуживание индексов запускаем один раз при старте и раз в несколько часов
             * */
            TasksManager.SetTask(typeof(Module).FullName + "_" + nameof(MaintenanceIndexes), DateTime.Now.AddSeconds(30), () => MaintenanceIndexesStatic());
            TasksManager.SetTask(typeof(Module).FullName + "_" + nameof(MaintenanceIndexes) + "_hourly6", Cron.HourInterval(6), () => MaintenanceIndexesStatic());

            /*
             * Прекомпиляция шаблонов при запуске.
             * */
            //if (!Debug.IsDeveloper)
            //    Tasks.TasksManager.SetTask(typeof(Module).FullName + "_" + nameof(RazorPrecompilationStatic), DateTime.Now.AddMinutes(1), () => RazorPrecompilationStatic());

#if DEBUG
            /**
             * Регулярная сборка мусора для сборки в режиме отладки.
             * */
            TasksManager.SetTask(typeof(Module).FullName + "_" + nameof(GCCollect) + "_minutely1", Cron.MinuteInterval(1), () => GCCollectStatic());
#endif

            /**
             * Регулярная проверка новых слов в лексическом менеджере.
             * */
            TasksManager.SetTask(typeof(Lexicon.LexiconManager).FullName + "_" + nameof(Lexicon.LexiconManager.PrepareNewWords) + "_minutely2", Cron.MinuteInterval(2), () => LexiconNewWordsStatic());

            ModelMetadataProviders.Current = new MVC.TraceModelMetadataProviderWithFiles();
        }

        public Dictionary<string, Conversations.ConversationBase> Conversations { get; } = new Dictionary<string, Conversations.ConversationBase>();

        #region Maintenance indexes
        public static void MaintenanceIndexesStatic()
        {
            var module = _thisModule;
            if (module == null) throw new Exception("Модуль не найден.");

            module.MaintenanceIndexes();
        }

        private void MaintenanceIndexes()
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var result = db.DataContext.StoredProcedure<object>("Maintenance_RebuildIndexes", new { MinimumIndexFragmentstionToSearch = 5 });
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.CriticalError, $"Ошибка обслуживания индексов", null, ex);
                Debug.WriteLine("FileManager.Module.MaintenanceIndexes: {0}", ex.Message);
            }
        }
        #endregion

        #region Lexicon new words
        public static void LexiconNewWordsStatic()
        {
            _thisModule.AppCore.Get<Lexicon.LexiconManager>().PrepareNewWords();
        }
        #endregion

        #region RazorPrecompilation
        public static void RazorPrecompilationStatic()
        {
            var module = _thisModule;
            if (module == null) throw new Exception("Модуль не найден.");

            module.RazorPrecompilation();
        }

        private void RazorPrecompilation()
        {
            try
            {
                throw new NotImplementedException();
                // todo ApplicationCore.Instance.ResourceManager.GeneratePrecompiled();
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.CriticalError, $"Ошибка прекомпиляции шаблонов", null, ex);
                Debug.WriteLine("FileManager.Module.RazorPrecompilation: {0}", ex.Message);
            }
        }
        #endregion

#if DEBUG
        #region GC collect for debug
        public static void GCCollectStatic()
        {
            var module = _thisModule;
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
