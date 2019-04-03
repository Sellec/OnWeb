using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Support.Reasons
{
    /// <summary>
    /// Описывает обоснование тикета - категорию/подкатегорию/дополнительные признаки. 
    /// </summary>
    public class ReasonBase
    {
        /// <summary>
        /// Основной ключ для идентификации обоснования.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Название обоснования.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Требуется ли обязательный выбор дочернего обоснования.
        /// </summary>
        public bool IsNeedSubReason { get; }

        /// <summary>
        /// Список дочерних обоснований.
        /// </summary>
        public List<ReasonBase> SubReasonList { get; } = new List<ReasonBase>();

        #region Equals
        public override bool Equals(object obj)
        {
            if (obj is ReasonBase) return Key.Equals((obj as ReasonBase).Key);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
        #endregion

    }
}