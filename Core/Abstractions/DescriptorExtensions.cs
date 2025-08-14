using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static class DescriptorExtensions
    {
        public static bool IsPathToAssetLibrary(this AssetDescriptor descriptor) => IsPathToAssetLibraryValid(descriptor.AssetLibraryId);
        public static bool IsPathToAssetLibrary(this DatasetDescriptor descriptor) => IsPathToAssetLibraryValid(descriptor.AssetLibraryId);
        public static bool IsPathToAssetLibrary(this FileDescriptor descriptor) => IsPathToAssetLibraryValid(descriptor.AssetLibraryId);
        public static bool IsPathToAssetLibrary(this CollectionDescriptor descriptor) => IsPathToAssetLibraryValid(descriptor.AssetLibraryId);
        public static bool IsPathToAssetLibrary(this FieldDefinitionDescriptor descriptor) => IsPathToAssetLibraryValid(descriptor.AssetLibraryId);
        public static bool IsPathToAssetLibrary(this LabelDescriptor descriptor) => IsPathToAssetLibraryValid(descriptor.AssetLibraryId);
        public static bool IsPathToAssetLibraryValid(this AssetLibraryId assetLibraryId) => assetLibraryId != AssetLibraryId.None;
    }
}
