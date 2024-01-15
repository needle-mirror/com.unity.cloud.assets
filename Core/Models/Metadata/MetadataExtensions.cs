using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public static class MetadataExtensions
    {
        /// <summary>
        /// Returns a dictionary of metadata values.
        /// </summary>
        /// <param name="metadataValues">The <see cref="IMetadataContainer"/> to convert to a dictionary. </param>
        /// <returns>A dictionary of metadata values. </returns>
        public static Dictionary<string, object> ToDictionary(this IReadOnlyDictionary<string, IMetadataValue> metadataValues)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var entry in metadataValues)
            {
                dictionary.Add(entry.Key, entry.Value.ToObject());
            }

            return dictionary;
        }

        internal static IEnumerable<KeyValuePair<string, object>> ToEnumeration(this IDictionary<string, MetadataValue> metadataValues)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var entry in metadataValues)
            {
                dictionary.Add(entry.Key, entry.Value.ToObject());
            }

            return dictionary;
        }

        internal static object ToObject(this IMetadataValue metadataValue)
        {
            return metadataValue.ValueType switch
            {
                MetadataValueType.Boolean => metadataValue.AsBoolean(),
                MetadataValueType.SingleSelection => metadataValue.AsSingleSelection().GetValue(),
                MetadataValueType.MultiSelection => metadataValue.AsMultiSelection().GetValue(),
                MetadataValueType.Number => metadataValue.AsNumber(),
                MetadataValueType.Timestamp => metadataValue.AsTimestamp(),
                MetadataValueType.Url => metadataValue.AsUrl().GetValue(),
                _ => metadataValue.ToString()
            };
        }
    }
}
