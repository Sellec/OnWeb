using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.FileManager.CustomFieldsFileTypes
{
    public class RequiredAttributeForSingleValue : System.ComponentModel.DataAnnotations.RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            if (value is int && (int)value == 0) return false;

            return base.IsValid(value);
        }
    }

    public class RequiredAttributeForMultipleValue : System.ComponentModel.DataAnnotations.RequiredAttribute
    {
        public override bool IsValid(object value)
        {
           if (value == null) return false;
            if (value is IEnumerable<int> && (value as IEnumerable<int>).Where(x => x > 0).Count() == 0) return false;
            if (value is IEnumerable<File> && (value as IEnumerable<File>).Where(x => x != null && x.IdFile > 0).Count() == 0) return false;

            return base.IsValid(value);
        }
    }

}