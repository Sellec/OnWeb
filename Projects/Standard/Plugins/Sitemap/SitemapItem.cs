﻿using System;

namespace OnWeb.Plugins.Sitemap
{
    /// <summary>
    /// Описывает один адрес для карты сайта.
    /// </summary>
    public sealed class SitemapItem
    {
        /// <summary>
        /// URL-адрес страницы. Этот URL-адрес должен начинаться с префикса (например, HTTP) и заканчиваться косой чертой, если Ваш веб-сервер требует этого. Длина этого значения не должна превышать 2048 символов.
        /// </summary>
        public Uri Location { get; set; }

        /// <summary>
        /// Дата последнего изменения страницы. 
        /// Обратите внимание, что этот тег не имеет отношения к заголовку "If-Modified-Since (304)", который может вернуть сервер, поэтому поисковые системы могут по-разному использовать информацию из этих двух источников.
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Вероятная частота изменения этой страницы. Это значение предоставляет общую информацию для поисковых систем и может не соответствовать точно частоте сканирования этой страницы.
        /// Значение "всегда" должно использоваться для описания документов, которые изменяются при каждом доступе к этим документам. Значение "никогда" должно использоваться для описания архивных URL-адресов. 
        /// Имейте в виду, что значение для этого тега рассматривается как подсказка, а не как команда.
        /// Несмотря на то, что сканеры поисковой системы учитывают эту информацию при принятии решений, они могут сканировать страницы с пометкой "ежечасно" менее часто, чем указано, а страницы с пометкой "ежегодно" – более часто, чем указано.
        /// Сканеры могут периодически сканировать страницы с пометкой "никогда", чтобы отслеживать неожиданные изменения на этих страницах.
        /// </summary>
        public SitemapPriority? ChangeFrequency { get; set; }

        /// <summary>
        /// Приоритетность URL относительно других URL на Вашем сайте. Допустимый диапазон значений — от 0,0 до 1,0. Это значение не влияет на процедуру сравнения Ваших страниц со страницами на других сайтах — оно только позволяет указать поисковым системам, какие страницы, по Вашему мнению, более важны для сканеров.
        /// Приоритет страницы по умолчанию — 0,5.
        /// Следует учитывать, что приоритет, который Вы назначили странице, не влияет на положение Ваших URL на страницах результатов той или иной поисковой системы.Поисковые системы используют эту информацию при обработке URL, которые относятся к одному и тому же сайту, поэтому можно использовать этот тег для увеличения вероятности присутствия в поисковом индексе Ваших самых важных страниц.
        /// Кроме того, следует учитывать, что назначение высокого приоритета всем URL на Вашем сайте не имеет смысла. Поскольку приоритетность – величина относительная, этот параметр используется для того, чтобы определить очередность обработки URL в пределах сайта.
        /// </summary>
        public decimal? Priority { get; set; }
    }
}