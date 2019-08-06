using OnUtils.Application.Configuration;

namespace OnWeb.Core.Configuration
{
    using Modules;

    /// <summary>
    /// Предоставляет доступ к настройкам модуля TModule.
    /// </summary>
    /// <typeparam name="TModule">Должен быть query-типом модуля, зарегистрированным в привязках типов.</typeparam>
    public class ModuleConfiguration<TModule> : ModuleConfiguration<WebApplication, TModule>
        where TModule : ModuleCore<TModule>
    {
    }
}
