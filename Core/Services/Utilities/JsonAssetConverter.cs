using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unity.Cloud.Assets
{
    class JsonAssetConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var jObject = JObject.Load(reader);
            var asset = Activator.CreateInstance(objectType);
            serializer.Populate(jObject.CreateReader(), asset);

            return asset;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IAsset).IsAssignableFrom(objectType);
        }
    }
}
