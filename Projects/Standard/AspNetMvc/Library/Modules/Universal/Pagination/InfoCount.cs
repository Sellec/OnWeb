using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Modules.Universal.Pagination
{
    /// <summary>
    /// Информация о постраничном выводе.
    /// </summary>
    public class InfoCount
    {
        /// <summary>
        /// Общее количество объектов.
        /// </summary>
        public int all { get; set; }

        /// <summary>
        /// Начальная позиция выборки (начинается с 1, не с 0). Для корректного использования в запросах не забыть отнять единицу.
        /// </summary>
        public int start { get; set; }

        public int page { get; set; }

        /// <summary>
        /// Количество объектов на странице. Теоретическое значение из настроек или из <see cref="ListViewOptions.numpage"/> , согласно которому рассчитывается количество страниц.
        /// </summary>
        public int objectsPerPageTheory { get; set; }

        /// <summary>
        /// Количество объектов на текущей странице. Это может быть либо <see cref="objectsPerPageTheory"/>, либо остаток, если количество объектов на последней странице меньше <see cref="objectsPerPageTheory"/>.
        /// </summary>
        public int objectsPerPage { get; set; }
    }
}