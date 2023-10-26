# Use case: View the collections of an asset and manage its associations to these collections

You can use the Unity Cloud Assets package to:

  * List the collections an asset belongs to.
  * Add and remove an asset from a collection.

The SDK supports different workflows for users with different roles.

| Asset Manager Project role                                                                             | List an asset's collections | Add/remove assets in collections |
|:-------------------------------------------------------------------------------------------------------|:-------------------------------|:---------------------------------|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)      | yes                            | no                               |
| [`Asset Management Consumer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)    | yes                            | no                               |
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) | yes                            | yes                              |
| [`Asset Management Owner`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)     | yes                            | yes                              |

## Before you start

Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List the collections of an asset

By default, an asset's collections are not included when you get an asset. To fetch the collections of an asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseAssetCollectionExample.cs#Example_Behaviour_RefreshCollections)]

The code snippet populates a list of the collections of the selected asset.

### Remove an asset from a collection

To remove an asset from a collection:

1. Open the `AssetManagementBehaviour` script you created. 
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseAssetCollectionExample.cs#Example_Behaviour_RemoveFromCollection)]

The code snippet does the following:

  * Removes the selected asset from the specified collection.
  * Updates the list of collections of the selected asset.
  * Prints a message to the console on success.

### Add the UI for viewing and creating collections

To add UI for the example:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseAssetCollectionExample.cs#Example_UI)]

The code snippet displays:

  * A list of the selected asset's collections.
  * A UI button beside each collection to remove the selected asset from it.
