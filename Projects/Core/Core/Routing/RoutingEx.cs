using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Routing
{
    /// <summary>
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    class RoutingEx : DB.Routing
    {
        public RoutingEx()
        {
        }

        public RoutingEx(DB.Routing source, string mname)
        {
            this.m_name = mname;
            this.Action = source.Action;
            this.Arguments = source.Arguments;
            this.DateChange = source.DateChange;
            this.IdItem = source.IdItem;
            this.IdItemType = source.IdItemType;
            this.IdModule = source.IdModule;
            this.IdRoute = source.IdRoute;
            this.IdUserChange = source.IdUserChange;
            this.UrlFull = source.UrlFull;
        }

        public string m_name;
    }
}
