# Search assets

You can use the Unity Asset Manager SDK package to filter assets in a project based on a set of search criteria.

There are two workflows when searching for assets which can be controlled from the `AssetServiceConfiguration` class.
Setting the `IsDiscovery` to `true` in the `AssetServiceConfiguration` will fetch and search among published assets only.
While setting the value to `false` will fetch and search among all assets regardless of status.

The implementation you choose will depend on the project roles of your users.

> **Note**: The Asset Discovery pathway requires users have the minimum role of [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html), while the Asset Management requires higher permissions with a minimum role of [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html).

## How do I...?

### Built-in Search Filter

The Unity Asset Manager SDK package provides a built-in search filter that you can use to search for assets: the `AssetSearchFilter` class.
The `AssetSearchFilter` class provides the set of properties that can be used to filter assets.

#### Create a new search filter

You can create a new search filter by instantiating the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Constructor)]

Searches are scoped to a specific organization and project. However, the instance can be reused to search for assets in different projects and organizations by updating the properties.

* To update the search to another organization, you can use the `Organization` property, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Organization)]

* To update the search to another project, you can use the `Project` property, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Project)]

Each searchable property provides 3 avenues for searching:

- `Include` - The property must match the value exactly.
- `Exclude` - The property must not match the value.
- `Any` - The property may contain the value. This represents an `OR` operation to be applied on all properties that include the `Any` value.

To compute the search results, you can use the `SearchAsync` method of an `IAssetProvider`, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Search)]

The `Pagination` struct is used to control the range of results to be returned and the ordering of results.

In this example, the first 10 results displayed are sorted by the asset name in ascending order.
The `Search` method returns an awaitable `IAsyncEnumerable` that will return each `IAsset` result.

The results can be iterated over using a `foreach` loop and used as they become available, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_Foreach)]

Alternatively, the results can be iterated over and compiled into a list, so that the complete set of results can be used, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_ToList)]

#### Search by Name

* You can search for assets by name using the `Name` property of the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_NameInclude)]

> **Note**: This type of search checks for assets whose entire name exactly matches the parameter.

* You can also exclude assets by name, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_NameExclude)]

* You can also search for assets whose name contains a specific string, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_NameAny)]

#### Search by Tags

You can search for assets by tag using the `Tags` property of the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/UseCaseSearchAssetsExample.cs#Example_TagsInclude)]

> **Note**: This type of search checks for assets whose tag list contains all the included parameters.

### Custom Search Filter

You can also create a custom search filter by implementing the `IAssetSearchFilter` interface.
