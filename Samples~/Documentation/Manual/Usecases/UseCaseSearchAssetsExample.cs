using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets;
using UnityEngine;

namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S1144 // Remove unused private method
#pragma warning disable S1481 // Unused local variables should be removed
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

        void SearchAsync(IAssetSearchFilter assetSearchFilter)
        {
#region Example_Search

var results = project.QueryAssets()
    .SelectWhereMatchesFilter(assetSearchFilter)
    .OrderBy(nameof(IAsset.Name), SortingOrder.Descending)
    .LimitTo(Range.EndAt(10))
    .ExecuteAsync(CancellationToken.None);

#endregion
        }

        async Task DisplayResultsIndividually()
        {
            var results = project.QueryAssets().ExecuteAsync(default);
#region Example_Foreach

await foreach (var asset in results)
{
    Debug.Log(asset.Name + " is available for use.");

    // Do something with each `asset` as it becomes available.
}

#endregion
        }

        async Task<IEnumerable<IAsset>> DisplayResults()
        {
            var results = project.QueryAssets().ExecuteAsync(default);

#region Example_ToList

var assetList = new List<IAsset>();
await foreach (var asset in results)
{
    assetList.Add(asset);
}

return assetList;

#endregion
        }
    }
#pragma warning restore S1144 // Remove unused private method
#pragma warning restore S1481 // Unused local variables should be removed
}
