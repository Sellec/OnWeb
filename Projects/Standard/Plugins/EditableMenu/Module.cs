using OnUtils.Data;
using OnUtils.Tasks;
using System;

namespace OnWeb.Plugins.EditableMenu
{
    using Core.Modules;

    /// <summary>
    /// Модуль управления редактируемыми меню.
    /// </summary>
    [ModuleCore("Редактируемые меню")]
    public class Module : ModuleCore<Module>, IUnitOfWorkAccessor<UnitOfWork<DB.Menu>>
    {
        public const string PERM_EDITABLEMENU = "editablemenu";

        protected override void InitModuleCustom()
        {
            RegisterPermission(PERM_EDITABLEMENU, "Управление настраиваемыми меню");
        }

    }
}