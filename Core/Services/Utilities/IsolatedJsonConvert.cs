using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class for converting JSON.
    /// </summary>
    static class IsolatedJsonConvert
    {
        public static JsonSerializerSettings jsonSerializerSettingsWithoutType = new()
        {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented
        };

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object value)
        {
            var jsonSerializer = JsonSerializer.Create(null);
            return SerializeObjectInternal(value, jsonSerializer);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        static string SerializeObjectInternal(object value, JsonSerializer jsonSerializer)
        {
            var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using (var jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                jsonTextWriter.Formatting = jsonSerializer.Formatting;
                jsonSerializer.Serialize(jsonTextWriter, value, null);
            }

            return stringWriter.ToString();
        }

        /// <summary>
        /// Deserialize the specified string value into an object of the specified <typeparam name="T"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T DeserializeObject<T>(string value, params JsonConverter[] converters)
        {
            return JsonConvert.DeserializeObject<T>(value, converters);
        }

        /// <summary>
        /// Deserialize the specified string value into an object of the specified <typeparam name="T"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T DeserializeObject<T>(string value, JsonSerializerSettings settings) => (T)DeserializeObject(value, typeof(T), settings);

        /// <summary>
        /// Serialize the specified object value to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        internal static string SerializeObject(object value, JsonSerializerSettings settings) => SerializeObjectInternal(value, JsonSerializer.Create(settings));

        /// <summary>
        /// Deserialize the specified string value into an object of the specified type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        static object DeserializeObject(string value, Type type, JsonSerializerSettings settings)
        {
            var jsonSerializer = JsonSerializer.Create(settings);
            using (var reader = new JsonTextReader(new StringReader(value)))
                return jsonSerializer.Deserialize(reader, type);
        }
    }
}
