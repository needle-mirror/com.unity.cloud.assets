using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets;
using UnityEngine;

namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S1144 // Remove unused private method
    public class UseCaseSearchAssetsExample
    {
        readonly IAssetProject project;

        public UseCaseSearchAssetsExample(IAssetProject project)
        {
            this.project = project;
        }

        public void Example()
        {
#region Example_Constructor

var assetSearchFilter = new AssetSearchFilter();

#endregion

#region Example_NameInclude

assetSearchFilter.Include().Name.WithValue("my cool asset");

#endregion

#region Example_NameExclude

assetSearchFilter.Exclude().Name.WithValue("my mediocre asset");

#endregion

#region Example_NameAny

assetSearchFilter.Any().Name.WithValue("cool");

#endregion

#region Example_TagsInclude

assetSearchFilter.Include().Tags.WithValue("tag1", "tag2", "tag3");

#endregion

#region Example_Collections

assetSearchFilter.Collections.WhereContains("my awesome collection", "my other awesome collection");

#endregion

        }

        IAsyncEnumerable<IAsset> SearchAsync(IAssetSearchFilter assetSearchFilter)
        {
#region Example_Search

return project.QueryAssets().SelectWhereMatchesFilter(assetSearchFilter).LimitTo(new Range(0, 10)).ExecuteAsync(CancellationToken.None);

#endregion
        }

        async Task DisplayResultsIndividually(IAssetSearchFilter assetSearchFilter)
        {
#region Example_Foreach

var assets = project.QueryAssets().SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(CancellationToken.None);
await foreach (var asset in assets)
{
    Debug.Log(asset.Name + " is available for use.");

    // Do something with each `asset` as it becomes available.
}

#endregion
        }

        async Task DisplayResults(IAssetSearchFilter assetSearchFilter)
        {
#region Example_ToList

var assets = project.QueryAssets().SelectWhereMatchesFilter(assetSearchFilter).ExecuteAsync(CancellationToken.None);

var assetList = new List<IAsset>();
await foreach (var asset in assets)
{
    assetList.Add(asset);
}

// Do something with the complete `assetList`

#endregion
        }
    }
#pragma warning restore S1144 // Remove unused private method
}
