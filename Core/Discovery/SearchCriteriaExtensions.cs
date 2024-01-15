using System;
using System.Reflection;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static class SearchCriteriaExtensions
    {
        static readonly Type k_AssetType = typeof(Asset);

        internal static bool TryGetPropertyValue(this IAsset asset, string propertyName, out object value)
        {
            value = asset.GetPropertyValue(propertyName);
            return value != null // works for non-ValueType
                && (value is not string s || !string.IsNullOrWhiteSpace(s)); // special check for strings
        }

        internal static object GetPropertyValue(this IAsset asset, string propertyName)
        {
            return propertyName switch
            {
                nameof(AssetDescriptor.AssetId) => asset.Descriptor.AssetId,
                nameof(AssetDescriptor.AssetVersion) => asset.Descriptor.AssetVersion,
                nameof(IAsset.SourceProject) => asset.SourceProject.ProjectId,
                nameof(IAsset.Metadata) => asset.Metadata is MetadataContainerEntity metadataContainer ? metadataContainer.Properties.ToEnumeration() : null,
                _ => k_AssetType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(asset)
            };
        }

        internal static object GetPropertyValue(this object input, string propertyName)
        {
            return propertyName switch
            {
                nameof(FileDescriptor.Path) when input is IFile file => file.Descriptor.Path,
                nameof(IDataset.Metadata) when input is DatasetEntity datasetEntity => datasetEntity.MetadataEntity.Properties.ToEnumeration(),
                nameof(IFile.Metadata) when input is FileEntity fileEntity => fileEntity.MetadataEntity.Properties.ToEnumeration(),
                _ => input.GetType().GetProperty(propertyName)?.GetValue(input)
            };
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
