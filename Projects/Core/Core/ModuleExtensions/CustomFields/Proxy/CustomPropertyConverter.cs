using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Proxy
{
    class CustomPropertyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //base.WriteJson(writer, value, serializer);
            //var date = value as DateTime;
            //var niceLookingDate = date.ToString("MMMM dd, yyyy 'at' H:mm tt");
            //writer.WriteValue(niceLookingDate);

            var fields = (value as Data.DefaultSchemeWData);
            var jo = JObject.FromObject(value); ;
            //    // special serialization logic based on instance-specific flag
            //    jo = new JObject();
            //    jo.Add("names", string.Join(", ", new string[] { foo.A, foo.B, foo.C }));

            foreach(var field in fields.Values)
            {
                if (!string.IsNullOrEmpty(field.alias))
                {
                    var fieldValue = fields.GetType().GetProperty(field.alias).GetValue(fields);
                    jo.Add(field.alias, fieldValue == null ? null : JToken.FromObject(fieldValue));
                }
            }

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanRead
        {
            get => false;
        }

        public override bool CanConvert(Type objectType)
        {
            var type = typeof(Data.DefaultSchemeWData);
            return type.IsAssignableFrom(objectType) && type != objectType;
        }
    }
}
