using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets.Documentation.Manual
{
    public class UseCaseSearchAssetsExample
    {
        readonly IProject project;

        public UseCaseSearchAssetsExample(IProject project)
        {
            this.project = project;
        }

        public void Example(IProject newProject)
        {
#region Example_Constructor

var assetSearchFilter = new AssetSearchFilter(project);

#endregion

#region Example_Project

assetSearchFilter.Project.Include(newProject);

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

        }

        IAsyncEnumerable<IAsset> SearchAsync(IAssetProvider assetProvider, IAssetSearchFilter assetSearchFilter)
        {
#region Example_Search

Pagination pagination = new Pagination(nameof(IAsset.Name), new Range(0, 10), Pagination.Order.Ascending);
return assetProvider.SearchAsync(assetSearchFilter, pagination, CancellationToken.None);

#endregion
        }

        async Task DisplayResultsIndividually(IAssetProvider assetProvider, IAssetSearchFilter assetSearchFilter)
        {
            var pagination = new Pagination(nameof(IAsset.Name), Range.All);

#region Example_Foreach

var assets = assetProvider.SearchAsync(assetSearchFilter, pagination, CancellationToken.None);
await foreach (var asset in assets)
{
    Console.WriteLine(asset.Name + " is available for use.");

    // Do something with each `asset` as it becomes available.
}

#endregion
        }

        async Task DisplayResults(IAssetProvider assetProvider, IAssetSearchFilter assetSearchFilter)
        {
            var pagination = new Pagination(nameof(IAsset.Name), Range.All);

#region Example_ToList

var assets = assetProvider.SearchAsync(assetSearchFilter, pagination, CancellationToken.None);

var assetList = new List<IAsset>();
await foreach (var asset in assets)
{
    assetList.Add(asset);
}

// Do something with the complete `assetList`

#endregion
        }
    }
}
