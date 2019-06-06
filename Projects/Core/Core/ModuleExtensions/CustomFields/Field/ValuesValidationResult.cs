using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class ValuesValidationResult : ValidationResult
    {
        public ValuesValidationResult(string errorMessage) : base(errorMessage)
        {
            this.Values = null;
        }

        public ValuesValidationResult(IEnumerable<object> values) : base("")
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            this.Values = values;
        }

        public IEnumerable<object> Values { get; private set; }

        public bool IsSuccess
        {
            get => this == ValidationResult.Success || Values != null;
        }
    }
}
