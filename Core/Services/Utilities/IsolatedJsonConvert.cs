using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class for converting JSON.
    /// </summary>
    static class IsolatedJsonConvert
    {
        public static readonly JsonSerializerSettings jsonSerializerSettingsWithoutType = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Populate,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            TypeNameHandling = TypeNameHandling.None,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            Formatting = Formatting.None,
        };

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object value) => JsonConvert.SerializeObject(value, jsonSerializerSettingsWithoutType);

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object value, JsonSerializerSettings settings) => JsonConvert.SerializeObject(value, settings);

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object value, params JsonConverter[] converters) => SerializeObject(value, jsonSerializerSettingsWithoutType, converters);

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object value, JsonSerializerSettings settings, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(value, CloneSettings(settings, converters));
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
        /// <param name="converters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T DeserializeObject<T>(string value, JsonSerializerSettings settings, params JsonConverter[] converters)
        {
            return JsonConvert.DeserializeObject<T>(value, CloneSettings(settings, converters));
        }

        /// <summary>
        /// Deserialize the specified string value into an object of the specified <typeparam name="T"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="settings"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static T DeserializeObject<T>(string value, JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<T>(value, settings);

        static JsonSerializerSettings CloneSettings(JsonSerializerSettings settingsSource, params JsonConverter[] converters)
        {
            var mergedConverters = settingsSource.Converters.ToList();
            mergedConverters.AddRange(converters);

#if UC_NUGET
            var cloneSettings = new JsonSerializerSettings
            {
                FloatParseHandling = settingsSource.FloatParseHandling,
                FloatFormatHandling = settingsSource.FloatFormatHandling,
                DateParseHandling = settingsSource.DateParseHandling,
                DateTimeZoneHandling = settingsSource.DateTimeZoneHandling,
                DateFormatHandling = settingsSource.DateFormatHandling,
                Formatting = settingsSource.Formatting,
                MaxDepth = settingsSource.MaxDepth,
                DateFormatString = settingsSource.DateFormatString,
                Context = settingsSource.Context,
                Error = settingsSource.Error,
                SerializationBinder = settingsSource.SerializationBinder,
                TraceWriter = settingsSource.TraceWriter,
                Culture = settingsSource.Culture,
                ReferenceResolverProvider = settingsSource.ReferenceResolverProvider,
                EqualityComparer = settingsSource.EqualityComparer,
                ContractResolver = settingsSource.ContractResolver,
                ConstructorHandling = settingsSource.ConstructorHandling,
                TypeNameAssemblyFormatHandling = settingsSource.TypeNameAssemblyFormatHandling,
                MetadataPropertyHandling = settingsSource.MetadataPropertyHandling,
                TypeNameHandling = settingsSource.TypeNameHandling,
                PreserveReferencesHandling = settingsSource.PreserveReferencesHandling,
                Converters = mergedConverters,
                DefaultValueHandling = settingsSource.DefaultValueHandling,
                NullValueHandling = settingsSource.NullValueHandling,
                ObjectCreationHandling = settingsSource.ObjectCreationHandling,
                MissingMemberHandling = settingsSource.MissingMemberHandling,
                ReferenceLoopHandling = settingsSource.ReferenceLoopHandling,
                CheckAdditionalContent = settingsSource.CheckAdditionalContent,
                StringEscapeHandling = settingsSource.StringEscapeHandling
            };
#else
            var cloneSettings = new JsonSerializerSettings(settingsSource)
            {
                Converters = mergedConverters
            };
#endif

            return cloneSettings;
        }
    }
}
