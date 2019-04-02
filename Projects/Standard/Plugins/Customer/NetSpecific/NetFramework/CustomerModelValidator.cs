using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Customer.NetSpecific.NetFramework
{
    using Core.DB;

    class CustomerModelValidator : ModelValidator
    {
        public CustomerModelValidator(ModelMetadata metadata, ControllerContext controllerContext) : base(metadata, controllerContext)
        {
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            var model = container ?? Metadata.Model;
            var results = new List<ModelValidationResult>();

            if (model is User)
            {
                var model2 = model as User;
                if (string.IsNullOrEmpty(model2.email) && string.IsNullOrEmpty(model2.phone))
                {
                    results.Add(new ModelValidationResult() { MemberName = nameof(model2.email), Message = "Должен быть указан один из реквизитов - e-mail или телефон." });
                    results.Add(new ModelValidationResult() { MemberName = nameof(model2.phone), Message = "Должен быть указан один из реквизитов - e-mail или телефон." });
                }
                else
                {
                    using (var db = new CoreContext())
                    {
                        if (!string.IsNullOrEmpty(model2.email))
                        {
                            if (db.Users.Where(x => x.email == model2.email && x.id != model2.id).Count() > 0)
                                results.Add(new ModelValidationResult()
                                {
                                    MemberName = nameof(model2.email),
                                    //Message = string.Format("Значение '{0}' для уникального поля '{1}' уже существует в базе.", model2.email, metadata.PropertyDisplayName(nameof(model2.email)))
                                    Message = string.Format("{1} '{0}' уже занят.", model2.email, Metadata.PropertyDisplayName(nameof(model2.email)))
                                });
                        }
                        if (!string.IsNullOrEmpty(model2.phone))
                        {
                            var phone = UsersExtensions.preparePhone(model2.phone);
                            if (db.Users.Where(x => x.phone == phone && x.id != model2.id).Count() > 0)
                                results.Add(new ModelValidationResult()
                                {
                                    MemberName = nameof(model2.phone),
                                    //Message = string.Format("Значение '{0}' для уникального поля '{1}' уже существует в базе.", model2.phone, metadata.PropertyDisplayName(nameof(model2.phone)))
                                    Message = string.Format("{1} '{0}' уже занят.", model2.phone, Metadata.PropertyDisplayName(nameof(model2.phone)))
                                });
                        }
                    }
                }
            }

            return results;
        }
    }

    class CustomerModelValidatorProvider : ModelValidatorProvider
    {
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            return new CustomerModelValidator(metadata, context).ToEnumerable();
        }
    }
}