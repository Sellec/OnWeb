using OnUtils.Tasks;
using OnUtils.Application;
using OnUtils;
using System;

namespace OnWeb.Modules.Lexicon
{
    using Core.Modules;

    /// <summary>
    /// Представляет модуль, предоставляющий функционал для работы с лексикой - склонение, формы слов и т.д.
    /// </summary>
    public abstract class ModuleLexicon : ModuleCore<ModuleLexicon>
    {
        private static ModuleLexicon _thisModule = null;

        protected override void InitModuleCustom()
        {
            _thisModule = this;

            /*
             * Регулярная проверка новых слов в лексическом менеджере.
             * */
            TasksManager.SetTask(typeof(Lexicon.LexiconManager).FullName + "_" + nameof(Lexicon.LexiconManager.PrepareNewWords) + "_minutely2", Cron.MinuteInterval(2), () => LexiconNewWordsStatic());
        }

        #region Lexicon new words
        [ApiReversible]
        public static void LexiconNewWordsStatic()
        {
            _thisModule.AppCore.Get<Lexicon.LexiconManager>().PrepareNewWords();
        }
        #endregion

    }


}
