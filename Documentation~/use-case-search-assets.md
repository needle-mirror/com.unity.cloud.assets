# Search Assets

You can use the Unity Cloud Assets package to filter assets in a project based on a set of search criteria.

There are two workflows when searching for assets.
In Asset Discovery, the `CloudAssetDiscovery` implementation of the `IAssetProvider` interface will fetch and search among published assets only, while in AssetManagement, the `CloudAssetManager` implementation of the `IAssetProvider` interface will fetch and search among all assets regardless of status.

The implementation you choose will depend on the project roles of your users.
> Note: The Asset Discovery pathway requires users have the minimum role of `Asset Management Viewer`, while the Asset Management requires higher permissions with a minimum role of `Asset Management Contributor`.

## Methodology

### Built-in Search Filter

The Unity Cloud Assets package provides a built-in search filter that you can use to search for assets; the `AssetSearchFilter` class.
The `AssetSearchFilter` class provides the set of properties that can be used to filter assets.

#### Creating a new search filter

You can create a new search filter by instantiating the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Constructor)]

Searches are scoped to a specific organization and project. However, the instance can be reused to search for assets in different projects and organizations by updating the properties.

To update the search to another organization, you can use the `Organization` property, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Organization)]

To update the search to another project, you can use the `Project` property, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Project)]

Each searchable property provides 3 avenues for searching:

- `Include` - The property must match the value exactly.
- `Exclude` - The property must not match the value.
- `Any` - The property may contain the value. This represents an `OR` operation to be applied on all properties that include the `Any` value.

To compute the search results, you can use the `Search` method of an `IAssetProvider`, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Search)]

The `Pagination` struct is used to control the number of results returned and the page of results to return.
It is also used to define how the results should be sorted.
In this example, we display the first 10 results sorted by the asset name in ascending order.
The `Search` method returns a `Task` that can be awaited to get an `IAssetPage` containing the search results.

#### Search by Name

You can search for assets by name using the `Name` property of the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_NameInclude)]
> [Note] This type of search checks for assets whose entire name exactly matches the parameter.

You can also exclude assets by name, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_NameExclude)]

You can also search for assets whose name contains a specific string, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_NameAny)]

#### Search by Tags

You can search for assets by tag using the `Tags` property of the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_TagsInclude)]
> [Note] This type of search checks for assets whose tag list contains all the included parameters.

### Custom Search Filter

You can also create a custom search filter by implementing the `IAssetSearchFilter` interface.



