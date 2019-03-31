using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.Core.ModuleExtensions.CustomFields.MetadataAndValues
{
    public class RequiredAttributeCustom : System.ComponentModel.DataAnnotations.RequiredAttribute
    {
        //private Field.FieldType _fieldType = null;

        //public RequiredAttributeCustom(Type type)
        //{
        //    var fieldType = Field.FieldTypesCollection.GetType(type);
        //    if (fieldType == null) throw new ArgumentException("Неизвестный тип поля", nameof(type));
        //    _fieldType = fieldType;
        //}

        //public override bool IsValid(object value)
        //{
        //    if (base.IsValid(value))
        //    {
        //        if (value is System.Collections.ICollection) return (value as System.Collections.ICollection).Count > 0;
        //        return true;
        //    }
        //    else return false;
        //}
    }
}
