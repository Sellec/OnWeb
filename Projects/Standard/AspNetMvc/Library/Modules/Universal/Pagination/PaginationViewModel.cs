using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Modules.Universal.Pagination
{
    /// <summary>
    /// Если модель, передаваемая в представление, наследует от этого типа, то есть возможность использовать некоторые дополнительные возможности.
    /// Например, можно сделать стандартный вывод номеров страниц через вызов представления "Universal/Design/PaginationBlock.cshtml".
    /// </summary>
    public abstract class PaginationViewModel
    {
        public PagedView pages { get; set; }

        public InfoCount infoCount { get; set; }

        public ListViewOptions listViewOptions { get; set; }

        public abstract Uri CurrentURL { get; }
    }
}