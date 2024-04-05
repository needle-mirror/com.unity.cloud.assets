# Use case: Search assets in a project

You can use the Unity Cloud Assets package to filter assets in a Project based on a set of search criteria.

| Organization or Asset Manager Project role                                                           | Search |
|:-----------------------------------------------------------------------------------------------------|:-------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes    |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes    |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes    |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes    |

## How do I...?

### Built-in Search Filter

The Unity Assets package provides a built-in search filter that you can use to search for assets: the `AssetSearchFilter` class.
The `AssetSearchFilter` class provides the set of properties that can be used to filter assets.

#### Create a new search filter

You can create a new search filter by instantiating the `AssetSearchFilter` class, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_Constructor)]

The filter provides 3 methods for searching assets:

- `Include()` - The properties added here must match the value.
- `Exclude()` - The properties added here must not match the value.
- `Any()` - The properties added here may match the value. This represents an `OR` operation to be applied on all properties added via the `Any()` method.

To compute the search results, you can use the `QueryAssets` method of an `IAssetProject`, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_Search)]

In this example, only the first 10 results will be returned and are sorted by the asset name in ascending order.
The execution of the query returns an awaitable `IAsyncEnumerable` that will return each `IAsset` result.

The results can be iterated over using a `foreach` loop and used as they become available, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_Foreach)]

Alternatively, the results can be iterated over and compiled into a list, so that the complete set of results can be used, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_ToList)]

#### Search by Name

* You can search for assets by name using the `Name` property of the `AssetSearchFilter.Include()`, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_NameInclude)]

>[!NOTE]
>This type of search checks for assets whose entire name exactly matches the parameter.

* You can also exclude assets by name, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSearchAssetsExample.cs#Example_NameExclude)]

* You can also search for assets whose name contains a specific string, like so:

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

You can also create a custom search filter by implementing the `IAssetSearchFilter` interface.
