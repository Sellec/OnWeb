using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Configuration
{
    /// <summary>
    /// Варианты результата выполнения функции <see cref="ModuleConfigurationManipulator{TModule}.ApplyConfiguration{TConfiguration}(TConfiguration)"/>.
    /// </summary>
    public enum ApplyConfigurationResult
    {
        /// <summary>
        /// Отсутствует доступ к сохранению конфигурации модуля. См. <see cref="Modules.ModuleCore.PermissionSaveConfiguration"/>.
        /// </summary>
        PermissionDenied,

        /// <summary>
        /// Сохранение прошло успешно.
        /// </summary>
        Success,
    }
}
