using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Common
{
    public class DtoJsonConverter<TModel> : JsonConverter where TModel : new()
    {
        public static void Register(JsonSerializerSettings serializerSettings)
        {
            serializerSettings.Converters.Add(new DtoJsonConverter<TModel>());
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(TModel));
        }
        public TModel Bind(JObject jObject, JsonSerializer serializer)
        {
            serializer ??= new JsonSerializer();
            var result = new TModel();
            typeof(TModel).GetProperties().ToList().ForEach(p =>
            {
                var property = jObject.Properties().SingleOrDefault(j => j.Name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
                if(IsDateOnly(p))
                {
                    p.SetValue(result, property == null ? new DateOnly() : DateOnly.FromDateTime(property.Value.Value<DateTime>()));
                }
                else if(IsDateTime(p))
                {
                    p.SetValue(result, property == null ? DateTime.MinValue : property.Value.Value<DateTime>());
                }
                else if(IsTimeOnly(p))
                {
                    p.SetValue(result, property == null ? new TimeOnly() : TimeOnly.FromDateTime(property.Value.Value<DateTime>()));
                }
                else
                {
                    p.SetValue(result, property == null ? null : property.Value.ToObject(p.PropertyType, serializer));
                }
            });
            return result;
        }
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            return Bind(jObject, serializer);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        public static bool IsDateOnly(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.Equals(typeof(DateOnly));
        }
        public static bool IsDateTime(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.Equals(typeof(DateTime));
        }
        public static bool IsTimeOnly(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.Equals(typeof(TimeOnly));
        }
        public static bool IsType<T>(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.Equals(typeof(T));
        }
    }
}
