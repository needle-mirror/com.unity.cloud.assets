# Use case: Search assets across projects

You can use the Unity Cloud Assets package to filter assets across projects based on a set of search criteria.

| Organization or Asset Manager Project role                                                           | Cross-project search |
|:-----------------------------------------------------------------------------------------------------|:---------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes                  |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes                  |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes                  |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes                  |

## Methodology

To search assets across projects, you can use the `QueryAssets` method of an `IAssetRepository`, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAcrossProjectsAssetsExample.cs#Example_Search)]

The `ExecuteAsync` method returns an awaitable `IAsyncEnumerable` that will return each `IAsset` result.

### Built-in Search Filter

For more information on the `IAssetSearchFilter`, see [Use case: Search assets in a project](use-case-search-assets.md).
