using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static class SearchCriteriaExtensions
    {
        static readonly Type k_AssetType = typeof(Asset);
        static readonly Dictionary<string, object> k_EmptyValue = new()
        {
            {nameof(AssetDescriptor.AssetVersion), 0u},
        };

        public static object GetPropertyValue(this IAsset asset, string propertyName)
        {
            return propertyName switch
            {
                nameof(AssetDescriptor.AssetId) => asset.Descriptor.AssetId,
                nameof(AssetDescriptor.AssetVersion) => asset.Descriptor.AssetVersion,
                nameof(IAsset.SourceProject) => asset.SourceProject.ProjectId,
                _ => k_AssetType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(asset)
            };
        }

        public static bool TryGetPropertyValue(this IAsset asset, string propertyName, out object value)
        {
            value = asset.GetPropertyValue(propertyName);
            var emptyValueDefined = k_EmptyValue.TryGetValue(propertyName, out var emptyValue);
            return value != null // works for non-ValueType
                && (value is not string s || !string.IsNullOrWhiteSpace(s)) // special check for strings
                && (!emptyValueDefined || !value.Equals(emptyValue)); // check for specific ValueType
        }

        internal static object GetPropertyValue(this object input, string propertyName)
        {
            if (nameof(FileDescriptor.Path) == propertyName && input is IFile file)
            {
                return file.Descriptor.Path;
            }

            return input.GetType().GetProperty(propertyName)?.GetValue(input);
        }

        internal static bool TryGetIncluded(this ISearchCriteria input, out object includedValue)
        {
            return input.TryGetIncluded(out includedValue);
        }

        internal static bool TryGetExcluded(this ISearchCriteria input, out object excludedValue)
        {
            return input.TryGetExcluded(out excludedValue);
        }

        internal static bool TryGetAny(this ISearchCriteria input, out object anyValue)
        {
            return input.TryGetAny(out anyValue);
        }

        internal static bool IsMatch(this ISearchCriteria input, object value)
        {
            return input.IsMatch(value);
        }

        internal static bool IsAny(this ISearchCriteria input, object value)
        {
            return input.IsAny(value);
        }

        internal static string BuildSearchKey(this string searchKey, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return string.IsNullOrEmpty(searchKey) ? "" : $"{searchKey}";
            }
            return string.IsNullOrEmpty(searchKey) ? $"{prefix}" : $"{prefix}.{searchKey}";
        }
    }
}
