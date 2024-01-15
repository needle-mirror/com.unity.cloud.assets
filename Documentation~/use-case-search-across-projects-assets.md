# Use case: Search assets across projects

You can use the Unity Cloud Assets package to filter assets across projects based on a set of search criteria.

| Organization or Asset Manager Project role                                                           | Cross-project search |
|:-----------------------------------------------------------------------------------------------------|:---------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes                  |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes                  |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes                  |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes                  |

## Methodology

### Built-in Search Filter

The Unity Cloud Assets package provides a built-in search filter that you can use to search for assets; the `AssetSearchFilter` class.
The `AssetSearchFilter` class provides the set of properties that can be used to filter assets.

#### Search filter properties

Searches are done across specified projects.

Each searchable property provides 3 avenues for searching:

- `Include` - The property must match the value exactly.
- `Exclude` - The property must not match the value.
- `Any` - The property may contain the value. This represents an `OR` operation to be applied on all properties that include the `Any` value.

To compute the search results, you can use the `SearchAssetsAsync` method of an `IAssetProject`, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAcrossProjectsAssetsExample.cs#Example_Search)]

The `Pagination` struct is used to control the range of results to be returned and the ordering of results.
In this example, we display the first 10 results sorted by the asset name in ascending order.
The `SearchAsync` method returns an awaitable `IAsyncEnumerable` that will return each `IAsset` result.

The results can be iterated over using a `foreach` loop and used as they become available, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAcrossProjectsAssetsExample.cs#Example_Foreach)]

Alternatively, the results can be iterated over and compiled into a list, so that the complete set of results can be used, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAcrossProjectsAssetsExample.cs#Example_ToList)]

#### Search by Name

You can search for assets by name using the `Name` property of the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_NameInclude)]

>[!NOTE]
>This type of search checks for assets whose entire name exactly matches the parameter.

You can also exclude assets by name, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_NameExclude)]

You can also search for assets whose name contains a specific string, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_NameAny)]

#### Search by Tags

You can search for assets by tag using the `Tags` property of the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_TagsInclude)]

>[!NOTE]
>This type of search checks for assets whose tag list contains all the included parameters.

#### Filter by Collections

You can search for assets in specific collections by adding them to the search filter's list of collections, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_Collections)]

### Custom Search Filter

To create a custom search filter, you can implement the `IAssetSearchFilter` interface.
