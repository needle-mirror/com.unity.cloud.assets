using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    static class AssetExtensions
    {
        static readonly Type k_iAssetType = typeof(IAsset);
        static readonly Dictionary<string, object> k_EmptyValue = new()
        {
            {nameof(IAsset.Version), 0u},
            {nameof(IAsset.Created), default(DateTime)},
            {nameof(IAsset.Updated), default(DateTime)}
        };

        public static object GetPropertyValue(this IAsset asset, string propertyName)
        {
            return k_iAssetType.GetProperty(propertyName)?.GetValue(asset);
        }

        public static bool TryGetPropertyValue(this IAsset asset, string propertyName, out object value)
        {
            value = k_iAssetType.GetProperty(propertyName)?.GetValue(asset);
            var emptyValueDefined = k_EmptyValue.TryGetValue(propertyName, out var emptyValue);
            return value != null // works for non-ValueType
                && (value is not string s || !string.IsNullOrWhiteSpace(s)) // special check for strings
                && (!emptyValueDefined || !value.Equals(emptyValue)); // check for specific ValueType
        }
    }
}
