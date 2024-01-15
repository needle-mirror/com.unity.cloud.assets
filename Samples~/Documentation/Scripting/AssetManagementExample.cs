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
        var asset = await project.GetAssetAsync(assetId, assetVersion, FieldsFilter.Default, cancellationToken);
        return asset;
    }

    #endregion

    #region SearchForAssets

    IAsyncEnumerable<IAsset> SearchForAssets(IAssetProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter();
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(Range.All);

        var assets = project.SearchAssetsAsync(assetSearchFilter, pagination, cancellationToken);
        return assets;
    }

    IAsyncEnumerable<IAsset> SearchForAssets(IAssetRepository assetRepository, OrganizationId organizationId, IEnumerable<ProjectId> projects, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter();
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(Range.All);

        var assets = assetRepository.SearchAssetsAsync(organizationId, projects, assetSearchFilter, pagination, cancellationToken);
        return assets;
    }

    #endregion

    #region AggregateAssets

    async Task<Aggregation> AggregateAssets(IAssetProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter();
        assetSearchFilter.Name.Include(assetName);

        var aggregationParameters = new AggregationParameters(AssetTypeSearchCriteria.SearchKey, 20);

        var aggregation = await project.CountAssetsAsync(assetSearchFilter, aggregationParameters, cancellationToken);
        return aggregation;
    }

    async Task<Aggregation> AggregateAssets(IAssetRepository assetRepository, OrganizationId organizationId, IEnumerable<ProjectId> projects, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter();
        assetSearchFilter.Name.Include(assetName);

        var aggregationParameters = new AggregationParameters(AssetTypeSearchCriteria.SearchKey, 20);

        var aggregation = await assetRepository.CountAssetsAsync(organizationId, projects, assetSearchFilter, aggregationParameters, cancellationToken);
        return aggregation;
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
    }

    #endregion

    #region GetAssetDownloadUrls

    async Task GetAssetDownloadUrls(IAsset asset, CancellationToken cancellationToken)
    {
        await asset.GetAssetDownloadUrlsAsync(cancellationToken);
    }

    #endregion

    #region GetAssetCollections

    async Task<IEnumerable<CollectionPath>> RefreshAssetCollectionsAsync(IAsset asset, CancellationToken cancellationToken)
    {
        await asset.RefreshAssetCollectionsAsync(cancellationToken);
        return asset.Collections;
    }

    #endregion

    #region LinkAssetToProject

    async Task LinkAssetToProject(IAsset asset, ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
    {
        await asset.LinkToProjectAsync(projectDescriptor, cancellationToken);
    }

    #endregion

    #region UnlinkAssetFromProject

    async Task UnlinkAssetFromProject(IAsset asset, ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
    {
        await asset.UnlinkFromProjectAsync(projectDescriptor, cancellationToken);
    }

    #endregion

    #region PublishApprovedAsset

    async Task PublishApprovedAsset(IAsset asset, CancellationToken cancellationToken)
    {
        await asset.PublishAsync(cancellationToken);
    }

    #endregion

    #region WithdrawPublishedAsset

    async Task WithdrawPublishedAsset(IAsset asset, CancellationToken cancellationToken)
    {
        await asset.WithdrawAsync(cancellationToken);
    }

    #endregion

    #region SendAssetToReview

    async Task SendAssetToReviewAsync(IAsset asset, CancellationToken cancellationToken)
    {
        await asset.SendToReviewAsync(cancellationToken);
    }

    #endregion

    #region ApproveAsset

    async Task ApproveAssetAsync(IAsset asset, CancellationToken cancellationToken)
    {
        await asset.ApproveAsync(cancellationToken);
    }

    #endregion

    #region RejectAsset

    async Task RejectAssetAsync(IAsset asset, CancellationToken cancellationToken)
    {
        await asset.RejectAsync(cancellationToken);
    }

    #endregion
}
#pragma warning restore S1144
}
