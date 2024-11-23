using System;
using System.Dynamic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Appointment.Common
{
    public class UpdateModelConverter<TUpdateModel> : JsonConverter where TUpdateModel : new()
    {
        public static void Register(JsonSerializerSettings settings)
        {
            settings.Converters.Add(new UpdateModelConverter<TUpdateModel>());
        }

        public TUpdateModel Bind(JObject jObject, JsonSerializer serializer = null)
        {
            if(serializer == null)
            {
                serializer = new JsonSerializer();
            }
            var result = Create();

            typeof(TUpdateModel).GetProperties().ToList().ForEach(p =>
            {
                var property = jObject.Properties().SingleOrDefault(j => j.Name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
                if(IsPropertyUpdater(p))
                {
                    var type = GetTypeInsidePropertyUpdater(p);
                    p.SetValue(result, property == null
                        ? Keep(type)
                        : NewPropertyUpdaterValue(type, property, serializer));
                }
                else
                {
                    p.SetValue(result, NewValue(p.PropertyType, property, serializer));
                }
            });

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(TUpdateModel));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            return Bind(jObject, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static object Keep(Type type)
        {
            var generic = typeof(PropertyUpdater<>).MakeGenericType(type);
            return Activator.CreateInstance(generic);
        }

        private static object NewPropertyUpdaterValue(Type typeInsidePropertyUpdater, JProperty property, JsonSerializer serializer)
        {
            var value = (typeInsidePropertyUpdater is IDynamicMetaObjectProvider)
                ? serializer.Deserialize(property.Value.CreateReader())
                : serializer.Deserialize(property.Value.CreateReader(), typeInsidePropertyUpdater);
            var generic = typeof(PropertyUpdater<>).MakeGenericType(typeInsidePropertyUpdater);
            return Activator.CreateInstance(generic, value);
        }

        private static object NewValue(Type type, JProperty property, JsonSerializer serializer)
        {
            if(property == null)
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            return (type is IDynamicMetaObjectProvider)
                ? serializer.Deserialize(property.Value.CreateReader())
                : serializer.Deserialize(property.Value.CreateReader(), type);
        }

        private static TUpdateModel Create()
        {
            return new TUpdateModel();
        }

        private static bool IsPropertyUpdater(PropertyInfo propertyInfo)
        {
            return (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(PropertyUpdater<>)));
        }

        private static Type GetTypeInsidePropertyUpdater(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.GetGenericArguments().First();
        }
    }
}
