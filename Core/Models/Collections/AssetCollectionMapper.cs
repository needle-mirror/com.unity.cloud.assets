using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static partial class EntityMapper
    {
        internal static AssetCollection From(this IAssetCollectionData data, IAssetDataSource dataSource, ProjectDescriptor projectDescriptor)
        {
            return data.From(dataSource, new CollectionDescriptor(projectDescriptor, data.GetFullCollectionPath()));
        }

        internal static AssetCollection From(this IAssetCollectionData data, IAssetDataSource dataSource, CollectionDescriptor collectionDescriptor)
        {
            return new AssetCollection(dataSource, collectionDescriptor, data.Name, data.Description, data.ParentPath);
        }

        internal static IAssetCollectionData From(this AssetCollection assetCollection)
        {
            return new AssetCollectionData(assetCollection.Name, assetCollection.ParentPath)
            {
                Description = assetCollection.Description,
            };
        }
    }
}
