using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Unity.Cloud.Assets
{
    static class IsolatedSerialization
    {
        public static readonly JsonSerializerSettings defaultSettings = new()
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
            Formatting = Formatting.None
        };

        public static JsonConverter CollectionPathConverter => new CollectionPathStringConverter();
        public static JsonConverter DatasetIdConverter => new DatasetIdConverter();
        public static JsonConverter JsonObjectConverter => new JsonObjectConverter();

        public static readonly JsonConverter[] Converters =
        {
            new OrganizationIdConverter(),
            new ProjectIdConverter(),
            new AssetIdConverter(),
            new AssetVersionIdConverter(),
            DatasetIdConverter,
            CollectionPathConverter,
            JsonObjectConverter
        };

        /// <summary>
        /// Deserialize a JSON string to a specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized type if successful, or null if unsuccessful.</returns>
        public static T DeserializeWithDefaultConverters<T>(string json)
        {
            return DeserializeWithConverters<T>(json, Converters);
        }

        /// <summary>
        /// Deserialize a JSON string to a specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="converters">Custom converters to use during deserialization.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized type if successful, or null if unsuccessful.</returns>
        public static T DeserializeWithConverters<T>(string json, params JsonConverter[] converters)
        {
            var settings = defaultSettings.Clone(converters);
            settings.Error = delegate(object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// Deserialize a JSON string to a specified type.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="settings">Custom settings to use during deserialization.</param>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>The deserialized type if successful, or null if unsuccessful.</returns>
        public static T Deserialize<T>(string json, JsonSerializerSettings settings)
        {
            var settingsCopy = settings.Clone();
            settings.Error = delegate(object sender, ErrorEventArgs args)
            {
                args.ErrorContext.Handled = true;
            };
            return JsonConvert.DeserializeObject<T>(json, settingsCopy);
        }

        /// <summary>
        /// Serialize a JSON string from a specified type.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string SerializeWithDefaultConverters<T>(T value)
        {
            return SerializeWithConverters(value, Converters);
        }

        /// <summary>
        /// Serialize a JSON string from a specified type.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">Custom converters to use during serialization.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string SerializeWithConverters<T>(T value, params JsonConverter[] converters)
        {
            var settings = defaultSettings.Clone(converters);
            return Serialize(value, settings);
        }

        /// <summary>
        /// Serialize a JSON string from a specified type.
        /// </summary>
        /// <typeparam name="T">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">Custom settings to use during serialization.</param>
        /// <returns>The serialized JSON string.</returns>
        public static string Serialize<T>(T value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        public static JsonSerializerSettings Clone(this JsonSerializerSettings settingsSource, params JsonConverter[] converters)
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
