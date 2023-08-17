using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets.Documentation.Manual
{
    public class UseCaseSearchAcrossProjectsAssetsExample
    {
        readonly IOrganization organization;

        public UseCaseSearchAcrossProjectsAssetsExample(IOrganization organization)
        {
            this.organization = organization;
        }

        public void Example()
        {
#region Example_Constructor

var assetSearchFilter = new AssetSearchFilter(null);

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

        IAsyncEnumerable<IAsset> SearchAsync(IAssetProvider assetProvider, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter)
        {
#region Example_Search

Pagination pagination = new Pagination(nameof(IAsset.Name), new Range(0, 10), Pagination.Order.Ascending);
return assetProvider.SearchAsync(organization, projects, assetSearchFilter, pagination, CancellationToken.None);

#endregion
        }

        async Task DisplayResultsIndividually(IAssetProvider assetProvider, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter)
        {
            var pagination = new Pagination(nameof(IAsset.Name), Range.All);

#region Example_Foreach

var assets = assetProvider.SearchAsync(organization, projects, assetSearchFilter, pagination, CancellationToken.None);
await foreach (var asset in assets)
{
    Console.WriteLine(asset.Name + " is available for use.");

    // Do something with each `asset` as it becomes available.
}

#endregion
        }

        async Task DisplayResults(IAssetProvider assetProvider, IEnumerable<IProject> projects, IAssetSearchFilter assetSearchFilter)
        {
            var pagination = new Pagination(nameof(IAsset.Name), Range.All);

#region Example_ToList

var assets = assetProvider.SearchAsync(organization, projects, assetSearchFilter, pagination, CancellationToken.None);

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
