# View the collections of an asset and manage its associations to these collections.

You can use the Unity Cloud Assets package to manage the collections of an asset.

Collection management is only available through the Asset Management pathway.
> Note: Collection management requires users have the role of `Asset Management Contributor` OR a minimum role of `Manager` in their organization.

## Prerequisites

Before you start, set up a Unity scene in with an organization and project browser.
See [Get started with Asset Management](get-started-management.md) for more information.

Next, you must have some assets in the cloud. There are several ways to do so:

* Assets can be created through the [Get started with Asset Management](get-started-management.md).
* Assets can be uploaded from existing Unity assets; see the [Asset Uploader sample](./asset-database-uploader-sample.md).
* You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## Methodology

### List the collections of an asset

By default, an asset's collections are not included when you get an asset.
To fetch the collections of an asset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseAssetCollectionExample.cs#Example_Behaviour_RefreshCollections)]

The code snippet does the following:

* Populates a list of the collections of the selected asset.

### Remove an asset from a collection

To remove an asset from a collection, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseAssetCollectionExample.cs#Example_Behaviour_RemoveFromCollection)]

The code snippet does the following:

* Removes the selected asset from the specified collection.
* Updates the list of collections of the selected asset.
* Prints a message to the console on success.

### Add the UI for viewing and creating collections

To add UI for the example, open the `AssetManagementUI` script you created and replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseAssetCollectionExample.cs#Example_UI)]

The code snippet does the following:

* Displays a list of the selected asset's collections.
* Displays a UI button beside each collection to remove the selected asset from it.
