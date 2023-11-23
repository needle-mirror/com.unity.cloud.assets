using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets.Documentation.Manual
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

assetSearchFilter.Name.Include("my cool asset");

#endregion

#region Example_NameExclude

assetSearchFilter.Name.Exclude("my mediocre asset");

#endregion

#region Example_NameAny

assetSearchFilter.Name.ForAny("cool");

#endregion

#region Example_TagsInclude

assetSearchFilter.Tags.Include("tag1", "tag2", "tag3");

#endregion

#region Example_Collections

assetSearchFilter.Collections.Add("my awesome collection");
assetSearchFilter.Collections.Add("my other awesome collection");

#endregion

        }

        IAsyncEnumerable<IAsset> SearchAsync(IAssetSearchFilter assetSearchFilter)
        {
#region Example_Search

Pagination pagination = new Pagination(nameof(IAsset.Name), new Range(0, 10), SortingOrder.Ascending);
return project.SearchAssetsAsync(assetSearchFilter, pagination, CancellationToken.None);

#endregion
        }

        async Task DisplayResultsIndividually(IAssetSearchFilter assetSearchFilter)
        {
            var pagination = new Pagination(nameof(IAsset.Name), Range.All);

#region Example_Foreach

var assets = project.SearchAssetsAsync(assetSearchFilter, pagination, CancellationToken.None);
await foreach (var asset in assets)
{
    Console.WriteLine(asset.Name + " is available for use.");

    // Do something with each `asset` as it becomes available.
}

#endregion
        }

        async Task DisplayResults(IAssetSearchFilter assetSearchFilter)
        {
            var pagination = new Pagination(nameof(IAsset.Name), Range.All);

#region Example_ToList

var assets = project.SearchAssetsAsync(assetSearchFilter, pagination, CancellationToken.None);

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
