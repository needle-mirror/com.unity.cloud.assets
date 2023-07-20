using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets.Documentation.Manual
{
    public class UseCaseSearchAssetsExample
    {
        readonly IOrganization organization;
        readonly IProject project;

        public UseCaseSearchAssetsExample(IOrganization organization, IProject project)
        {
            this.organization = organization;
            this.project = project;
        }

        public void Example(IOrganization newOrganization, IProject newProject)
        {
#region Example_Constructor

var assetSearchFilter = new AssetSearchFilter(organization, project);

#endregion

#region Example_Organization

assetSearchFilter.Organization.Include(newOrganization);

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

        async Task SearchAsync(IAssetProvider assetProvider, AssetSearchFilter assetSearchFilter)
        {
#region Example_Search

Pagination pagination = new Pagination(nameof(IAsset.Name), 10);
IAssetPage results = await assetProvider.SearchAsync(assetSearchFilter, pagination, CancellationToken.None);

#endregion
        }
    }
}
