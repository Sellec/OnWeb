using System;
using System.Collections.Generic;

namespace OnWeb.Core.Routing
{
    /// <summary>
    /// Описывает отдельный адрес (ЧПУ) в системе маршрутизации, привязанный к определенной сущности.
    /// </summary>
    public class RegisterItem
    {
        /// <summary>
        /// Целочисленный идентификатор сущности. Как правило, это значение столбца-идентификатора для какого-либо объекта.
        /// </summary>
        public int IdItem { get; set; }

        /// <summary>
        /// Идентификатор типа сущности. Рекомендуется использование фабрики типов <see cref="Items.ItemTypeFactory"/> для получения идентификатора типа (например, метод <see cref="Items.ItemTypeFactory.GetItemType(Type)"/>).
        /// Этот идентификатор используется для отделения друг от друга сущностей разного типа с одинаковыми идентификаторами (<see cref="IdItem"/>).
        /// </summary>
        public int IdItemType { get; set; }

        /// <summary>
        /// Название метода в контроллере модуля, на который ссылается адрес. При поиске необходимого модуля и его контроллера при распознавании ЧПУ используется идентификатор модуля в таблице <see cref="DB.Routing.IdModule"/> в записи, которой сооветствует данный ЧПУ.
        /// </summary>
        public string action { get; set; }

        /// <summary>
        /// Значения аргументов метода <see cref="action"/>, которые должны быть переданы для открытия конкретного адреса.
        /// </summary>
        public List<ActionArgument> Arguments { get; set; } = null;

        /// <summary>
        /// Уникальный ключ адреса, по которому для сущности можно получить определенный адрес. Адреса по-умолчанию (основной адрес сущности) обозначаются ключом <see cref="RoutingConstants.MAINKEY"/>.
        /// </summary>
        public string UniqueKey { get; set; } = null;

        /// <summary>
        /// Адрес ЧПУ, который соответствует комбинации {<see cref="IdItem"/>/<see cref="IdItemType"/>/<see cref="action"/>/<see cref="UniqueKey"/>}.
        /// </summary>
        public string Url { get; set; }

#if DEBUG
        /// <summary>
        /// </summary>
        public object DebugItem { get; set; }
#endif
    }
}
