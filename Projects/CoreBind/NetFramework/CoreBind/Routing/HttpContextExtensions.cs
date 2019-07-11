﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using OnWeb;

namespace System.Web.Mvc
{
    /// <summary>
    /// </summary>
    public static class HttpContextRoutingExtensions
    {
        const string EXTENSIONPREFIX = "HttpContextExtensions_";

        public static WebApplicationBase GetAppCore(this HttpContextBase context)
        {
            return context.Items.Contains(EXTENSIONPREFIX + "ApplicationCore") ? (WebApplicationBase)context.Items[EXTENSIONPREFIX + "ApplicationCore"] : null;
        }

        public static void SetAppCore(this HttpContext context, WebApplicationBase appCore)
        {
            context.Items[EXTENSIONPREFIX + "ApplicationCore"] = appCore;
        }
    }
}