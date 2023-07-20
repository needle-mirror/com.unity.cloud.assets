using System;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    class CollectionPathStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case null:
                    writer.WriteValue("");
                    break;
                case CollectionPath path:
                    writer.WriteValue(path.ToString());
                    break;
                default:
                    writer.WriteValue(value);
                    break;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var path = "";

            if (reader.TokenType == JsonToken.String)
            {
                path = reader.Value?.ToString();
            }

            return objectType == typeof(CollectionPath) ? new CollectionPath(path) : path;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string) || objectType == typeof(CollectionPath);
        }
    }
}
