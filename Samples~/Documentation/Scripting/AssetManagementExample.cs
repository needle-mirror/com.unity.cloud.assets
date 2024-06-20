using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Documentation.Assets.Scripting
{
#pragma warning disable S1144 // Remove unused private method
public class AssetManagementExample
{
    #region GetAsset

    async Task<IAsset> GetAsset(IAssetProject project, AssetId assetId, AssetVersion assetVersion, CancellationToken cancellationToken)
    {
        var asset = await project.GetAssetAsync(assetId, assetVersion, cancellationToken);
        return asset;
    }

    #endregion

    #region SearchForAssets

    IAsyncEnumerable<IAsset> SearchForAssets(IAssetProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter();
        assetSearchFilter.Include().Name.WithValue(assetName);

        var assets = project.QueryAssets().SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(cancellationToken);
        return assets;
    }

    IAsyncEnumerable<IAsset> SearchForAssets(IAssetRepository assetRepository, IEnumerable<ProjectDescriptor> projectDescriptors, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter();
        assetSearchFilter.Include().Name.WithValue(assetName);

        var assets = assetRepository.QueryAssets(projectDescriptors).SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(cancellationToken);
        return assets;
    }

    #endregion

    #region AggregateAssets

    async Task<IReadOnlyDictionary<string, int>> AggregateAssets(IAssetProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter();
        assetSearchFilter.Include().Name.WithValue(assetName);

        return await project.GroupAndCountAssets().SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(GroupableField.Type, cancellationToken);
    }

    async Task<IReadOnlyDictionary<string, int>> AggregateAssets(IAssetRepository assetRepository, IEnumerable<ProjectDescriptor> projectDescriptors, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter();
        assetSearchFilter.Include().Name.WithValue(assetName);

        return await assetRepository.GroupAndCountAssets(projectDescriptors).SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(GroupableField.Type, cancellationToken);
    }

    #endregion

    #region CreateAsset

    async Task<IAsset> CreateAsset(IAssetProject project, IAssetCreation assetCreation, CancellationToken cancellationToken)
    {
        var asset = await project.CreateAssetAsync(assetCreation, cancellationToken);
        return asset;
    }

    #endregion

    #region UpdateAsset

    async Task UpdateAsset(IAsset asset, IAssetUpdate assetUpdate, CancellationToken cancellationToken)
    {
        await asset.UpdateAsync(assetUpdate, cancellationToken);
        await asset.RefreshAsync(cancellationToken);
    }

    #endregion

    #region GetAssetDownloadUrls

    async Task GetAssetDownloadUrls(IAsset asset, CancellationToken cancellationToken)
    {
        await asset.GetAssetDownloadUrlsAsync(cancellationToken);
    }

    #endregion

    #region GetAssetCollections

    IAsyncEnumerable<CollectionDescriptor> GetAssetCollectionsAsync(IAsset asset, CancellationToken cancellationToken)
    {
        return asset.ListLinkedAssetCollectionsAsync(Range.All, cancellationToken);
    }

    #endregion
}
#pragma warning restore S1144
}
