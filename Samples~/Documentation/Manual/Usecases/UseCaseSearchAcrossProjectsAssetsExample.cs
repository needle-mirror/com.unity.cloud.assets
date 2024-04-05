using System;
using System.Collections.Generic;
using System.Threading;
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

IAsyncEnumerable<IAsset> SearchAsync(IEnumerable<ProjectDescriptor> projectDescriptors, IAssetSearchFilter searchFilter)
{
    return m_AssetRepository.QueryAssets(projectDescriptors)
        .SelectWhereMatchesFilter(searchFilter)
        .ExecuteAsync(CancellationToken.None);
}

#endregion
    }
#pragma warning restore S1144 // Remove unused private method
}
