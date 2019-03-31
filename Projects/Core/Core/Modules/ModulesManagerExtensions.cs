using OnUtils.Application.Modules;
using System.Collections.Generic;

namespace OnWeb
{
    using Core;
    using Core.Modules;

    /// <summary>
    /// Расширения для менеджера модулей. Учитывая, что менеджер модулей для веб-ядра не может наследоваться ни от чего, кроме как от <see cref="ModulesManager"/> и регистрироваться только через SetSingleton{ModulesManager{ApplicationCore}, ModulesManager}, ошибок InvalidCastException не должно возникать.
    /// </summary>
    public static class ModulesManagerExtensions
    {
        /// <summary>
        /// Возращает список модулей, зарегистрированных в системе.
        /// </summary>
        public static List<ModuleCore> GetModules(this ModulesManager<ApplicationCore> manager)
        {
            return ((ModulesManager)manager).GetModulesInternal();
        }

        /// <summary>
        /// Возвращает модуль с url-доступным именем <paramref name="urlName"/> (см. <see cref="ModuleCore.UrlName"/>).
        /// </summary>
        /// <returns>Объект модуля либо null, если подходящий модуль не найден.</returns>
        public static ModuleCore GetModule(this ModulesManager<ApplicationCore> manager, string urlName)
        {
            return ((ModulesManager)manager).GetModuleInternal(urlName);
        }

        /// <summary>
        /// Возвращает модуль с идентификатором <paramref name="moduleID"/> (см. <see cref="ModuleCore.IdModule"/>). 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="moduleID">Идентификатор модуля.</param>
        /// <returns>Объект модуля либо null, если подходящий модуль не найден.</returns>
        public static ModuleCore GetModule(this ModulesManager<ApplicationCore> manager, int moduleID)
        {
            return ((ModulesManager)manager).GetModuleInternal(moduleID);
        }

    }
}
