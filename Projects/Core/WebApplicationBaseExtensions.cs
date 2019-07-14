﻿using OnUtils.Application;

namespace OnWeb
{
    /// <summary>
    /// </summary>
    public static class WebApplicationBaseExtensions
    {
        /// <summary>
        /// Возвращает модуль ядра веб-приложения.
        /// </summary>
        public static Plugins.WebCoreModule.WebCoreModule GetWebCoreModule(this ApplicationCore appCore)
        {
            return appCore.Get<Plugins.WebCoreModule.WebCoreModule>();
        }

        /// <summary>
        /// Возвращает модуль ядра веб-приложения.
        /// </summary>
        public static Plugins.WebCoreModule.WebCoreConfiguration GetWebConfig(this ApplicationCore appCore)
        {
            return ((WebApplicationBase)appCore).WebConfig;
        }

    }
}