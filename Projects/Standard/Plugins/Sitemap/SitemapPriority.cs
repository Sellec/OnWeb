namespace OnWeb.Modules.Sitemap
{
    /// <summary>
    /// Вероятная частота изменения страницы. Это значение предоставляет общую информацию для поисковых систем и может не соответствовать точно частоте сканирования этой страницы. 
    /// </summary>
    public enum SitemapPriority
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }
}