using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets;
using Unity.Cloud.Common;
using UnityEngine;

namespace Unity.Cloud.Documentation.Assets
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

IAsyncEnumerable<IAsset> SearchAsync(IEnumerable<ProjectDescriptor> projectDescriptors, IAssetSearchFilter assetSearchFilter)
{
    return m_AssetRepository.QueryAssets(projectDescriptors).SelectWhereMatchesFilter(assetSearchFilter).LimitTo(new Range(0, 10)).ExecuteAsync(CancellationToken.None);
}

#endregion

#region Example_Foreach

async Task DisplayResultsIndividually(IEnumerable<ProjectDescriptor> projectDescriptors, IAssetSearchFilter assetSearchFilter)
{
    var assets = m_AssetRepository.QueryAssets(projectDescriptors).SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(CancellationToken.None);

    await foreach (var asset in assets)
    {
        Debug.Log(asset.Name + " is available for use.");

        // Do something with each `asset` as it becomes available.
    }

}

#endregion

#region Example_ToList

async Task<IEnumerable<IAsset>> DisplayResults(IEnumerable<ProjectDescriptor> projectDescriptors, IAssetSearchFilter assetSearchFilter)
{
    var assets = m_AssetRepository.QueryAssets(projectDescriptors).SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(CancellationToken.None);

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
