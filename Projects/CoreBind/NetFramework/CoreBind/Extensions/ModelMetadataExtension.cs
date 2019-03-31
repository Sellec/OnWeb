using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

public static class ModelMetadataExtension
{
    /// <summary>
    /// Возвращает отображаемое имя для свойства <paramref name="propertyName"/> в модели метаданных <paramref name="metadata"/>.
    /// </summary>
    /// <param name="metadata"></param>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    public static string PropertyDisplayName(this ModelMetadata metadata, string propertyName)
    {
        var metaproperty = metadata.Properties.Where(x => x.PropertyName == propertyName).FirstOrDefault();
        return metaproperty?.DisplayName;
    }
}
