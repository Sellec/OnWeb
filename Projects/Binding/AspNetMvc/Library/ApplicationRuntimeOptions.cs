using System;

namespace OnWeb
{
    /// <summary>
    /// Описывает дополнительные настройки работы ASP.Net pipeline.
    /// </summary>
    [Flags]
    public enum ApplicationRuntimeOptions
    {
        /// <summary>
        /// </summary>
        None = 0,

        /// <summary>
        /// Позволяет обрабатывать пути, находящиеся не внутри каталога приложения. Создается виртуальный каталог Symlinks внутри каталога приложения.
        /// </summary>
        IncludeSymlinks = 1,
    }
}
