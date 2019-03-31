using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Universal.Pagination
{
    public class PagedView
    {
        /// <summary>
        /// текущая страница
        /// </summary>
        public int curpage { get; set; }

        /// <summary>
        /// общее количество страниц
        /// </summary>
        public int pages { get; set; }

        public Dictionary<int, int> stpg { get; set; }

        public Dictionary<int, int> fnpg { get; set; }

        public int np { get; set; }

        public bool PageFound { get; set; } = true;
    }
}