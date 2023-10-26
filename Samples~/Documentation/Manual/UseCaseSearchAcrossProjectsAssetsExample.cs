using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets.Documentation.Manual
{
#pragma warning disable S1144 // Remove unused private method
    public class UseCaseSearchAcrossProjectsAssetsExample
    {
        readonly IAssetRepository m_AssetRepository;

        public UseCaseSearchAcrossProjectsAssetsExample(IAssetRepository assetRepository)
        {
            m_AssetRepository = assetRepository;
        }

#region Example_Search

IAsyncEnumerable<IAsset> SearchAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter)
{
    Pagination pagination = new Pagination(nameof(IAsset.Name), new Range(0, 10), Pagination.Order.Ascending);
    return m_AssetRepository.SearchAssetsAsync(organizationId, projectIds, assetSearchFilter, pagination, CancellationToken.None);
}

#endregion

#region Example_Foreach

async Task DisplayResultsIndividually(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter)
{
    var pagination = new Pagination(nameof(IAsset.Name), Range.All);

    var assets = m_AssetRepository.SearchAssetsAsync(organizationId, projectIds, assetSearchFilter, pagination, CancellationToken.None);

    await foreach (var asset in assets)
    {
        Console.WriteLine(asset.Name + " is available for use.");

        // Do something with each `asset` as it becomes available.
    }

}

#endregion

#region Example_ToList

async Task<IEnumerable<IAsset>> DisplayResults(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter)
{
    var pagination = new Pagination(nameof(IAsset.Name), Range.All);

    var assets = m_AssetRepository.SearchAssetsAsync(organizationId, projectIds, assetSearchFilter, pagination, CancellationToken.None);

    var assetList = new List<IAsset>();
    await foreach (var asset in assets)
    {
        assetList.Add(asset);
    }

    return assetList;
}

#endregion
    }
#pragma warning restore S1144 // Remove unused private method
}
