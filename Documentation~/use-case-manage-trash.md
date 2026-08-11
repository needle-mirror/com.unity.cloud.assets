# Use case: Manage trash in a project

You can use the Unity Cloud Assets package to manage assets in the trash of a project. This includes querying assets in trash, retrieving specific assets, restoring assets back to the project, permanently deleting assets, and emptying the entire trash.

| Organization or Asset Manager Project role                                                           | Query trash | Restore assets | Delete assets | Empty trash |
|:-----------------------------------------------------------------------------------------------------|:------------|:---------------|:--------------|:------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes         | no             | no            | no          |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes         | no             | no            | no          |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes         | yes            | yes           | yes         |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes         | yes            | yes           | yes         |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

All trash operations are available directly on an `IAssetProject` instance.

### Query assets in trash

To search for assets in a project's trash, use the `QueryTrashedAssets` method of an `IAssetProject`. This method returns a `TrashedAssetQueryBuilder` that you can use to filter and search assets, similar to querying regular assets.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetProjectsBehaviour.cs#Example_QueryTrashAssets)]

The `QueryTrashedAssets` method supports the same filtering and search capabilities as regular asset queries. You can use `AssetSearchFilter` to filter by name, tags, collections, and other properties.

#### Query assets across multiple project trashes

To search for assets across multiple project trashes, you can use the `QueryAssetsInTrash` method of an `IAssetRepository`. This method takes a collection of `ProjectDescriptor` objects and returns a `TrashedAssetQueryBuilder` that searches across all specified project trashes.

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetProjectsBehaviour.cs#Example_QueryTrashAcrossProjects)]

>[!NOTE]
>All projects must belong to the same organization when querying across multiple project trashes.

### Get an asset from trash

To retrieve a specific asset from trash by its `AssetId` and version, use the `GetTrashedAssetAsync` method:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetProjectsBehaviour.cs#Example_GetAssetFromTrash)]

The method returns an `ITrashedAsset` object representing the asset in trash. This trashed asset includes only the basic properties of the assets.

### Restore assets from trash

To restore assets from trash back to the project, use the `RestoreTrashedAssetsAsync` method. You can restore assets by providing their `AssetId` values:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetProjectsBehaviour.cs#Example_RestoreAssetsById)]

Alternatively, you can use the extension method that accepts `ITrashedAsset` objects directly:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetProjectsBehaviour.cs#Example_RestoreAssetsByAsset)]

When assets are restored, they are moved back to the project and are no longer in the trash.

>[!NOTE]
>Restoring assets requires appropriate permissions. Only users with [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions) role can restore assets from trash.

### Permanently delete assets from trash

To permanently delete assets from trash, use the `DeleteAssetsFromTrashAsync` method. You can delete assets by providing their `AssetId` values:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetProjectsBehaviour.cs#Example_DeleteAssetsById)]

Alternatively, you can use the extension method that accepts `ITrashedAsset` objects directly:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetProjectsBehaviour.cs#Example_DeleteAssetsByAsset)]

>[!WARNING]
>Permanently deleting assets from trash is irreversible. Once deleted, assets cannot be recovered.

>[!NOTE]
>Permanently deleting assets requires appropriate permissions. Only users with [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions) role can permanently delete assets from trash.

### Empty the trash

To permanently delete all assets in a project's trash, use the `EmptyTrashAsync` method:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetProjectsBehaviour.cs#Example_EmptyTrash)]

This method removes all assets from the trash permanently.

>[!WARNING]
>Emptying the trash is irreversible. All assets in the trash will be permanently deleted and cannot be recovered.

>[!NOTE]
>Emptying trash requires appropriate permissions. Only users with [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions) role can empty the trash.
