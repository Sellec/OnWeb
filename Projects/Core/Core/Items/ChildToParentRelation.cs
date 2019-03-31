using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Items
{
    /// <summary>
    /// Хранит пару родитель:потомок для работы с иерархией вложенности.
    /// </summary>
    public class ChildToParentRelation
    {
        /// <summary>
        /// Идентификатор родительского объекта в иерархии.
        /// </summary>
        public int IdParent { get; set; }

        /// <summary>
        /// Идентификатор дочернего (вложенного) объекта в иерархии.
        /// </summary>
        public int IdChild { get; set; }
    }


}
