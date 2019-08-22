using System.Linq;

namespace System.Web.Mvc
{
    /// <summary>
    /// </summary>
    public static class ModelMetadataExtension
    {
        /// <summary>
        /// Возвращает отображаемое имя для свойства <paramref name="propertyName"/> в модели метаданных <paramref name="metadata"/>.
        /// </summary>
        public static string PropertyDisplayName(this ModelMetadata metadata, string propertyName)
        {
            var metaproperty = metadata.Properties.Where(x => x.PropertyName == propertyName).FirstOrDefault();
            return metaproperty?.DisplayName;
        }
    }
}
