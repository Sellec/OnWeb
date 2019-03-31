using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Universal.Pagination
{
    public class ListViewOptions
    {
        public string Prefix { get; set; }

        public string sorting { get; set; }

        public int numpage { get; set; }

        //непонятный параметр, пока что закомментировал.
        //public int? skip { get; set; }

        public virtual IQueryable<TItem> BuildSortQuery<TItem>(IQueryable<TItem> query) 
        {
            return query;//.OrderBy(x => x);
        }
    }

}