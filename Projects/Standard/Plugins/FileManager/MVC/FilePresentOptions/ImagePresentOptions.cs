using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace System.Web.Mvc
{
    /// <summary>
    /// Дополнительные свойства и настройки для загрузчика изображений (<see cref="FileType.Image"/>).
    /// </summary>
    public class ImagePresentOptions : FilePresentOptions
    {
        /// <summary>
        /// Если значение задано, то ширина изображения будет подогнана под указанную ширину.
        /// </summary>
        public int? MaxWidth { get; set; }

        /// <summary>
        /// Если значение задано, то ширина изображения будет подогнана под указанную высоту.
        /// </summary>
        public int? MaxHeight { get; set; }
    }
}