using OnUtils.Application.Modules;
using OnUtils.Data;

namespace OnWeb.Plugins.EditableMenu
{
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