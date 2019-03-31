using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.FileManager.Conversations
{
    /// <summary>
    /// Представляет базовые возможности для преобразований файлов на уровне файлового менеджера.
    /// </summary>
    public abstract class ConversationBase : Items.ItemBase<Module>
    {
        /// <summary>
        /// Уникальный ключ преобразования, генерируется динамически при создании экземпляра преобразования.
        /// Суть ключа в том, чтобы передавать его через URL и определять нужное преобразование по этому ключу. 
        /// Ключ динамический, т.е. после перезапуска сайта или пересоздания преобразования он изменится, это исключит злоупотребление ключом со стороны.
        /// </summary>
        public Guid ConversationDynamicKey { get; private set; }

        #region Items.ItemBase
        /// <summary>
        /// Идентификатор преобразования на уровне БД. Пока что заглушка, т.е. TODO
        /// </summary>
        public sealed override int ID { get; set; }

        /// <summary>
        /// Удобочитаемое название преобразования.
        /// </summary>
        public sealed override string Caption { get; set; }

        /// <summary>
        /// </summary>
        public sealed override DateTime DateChangeBase { get; set; }

        /// <summary>
        /// Заглушка. Не работает.
        /// </summary>
        public sealed override Uri Url
        {
            get { return null; }
        }
        #endregion
    }
}