# Manage asset collections in a project

You can use the Unity Cloud Assets package to manage your collections of assets.

Collection management is only available through the Asset Management pathway.
> Note: Collection management requires users have the role of `Asset Management Contributor` OR a minimum role of `Manager` in their organization.

## Prerequisites

Before you start, set up a Unity scene in the Unity Editor with an organization and project browser.
See [Get started with Asset Management](get-started-management.md) for more information.

Next, you must have some assets in the cloud. There are several ways to do so:

* Assets can be created through the [Get started with Asset Management](get-started-management.md).
* Assets can be uploaded from existing Unity assets; see the [Asset Uploader sample](./asset-database-uploader-sample.md).
* You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## Methodology

### List the collections in a project

To list the existing collections in a project, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseCollectionManagementExample.cs#Example_Behaviour_RefreshCollections)]

The code snippet does the following:

* Populates a list of the collections in the selected project.
* Holds a reference to the selected collection.

### Create an asset collection

To create an asset collection, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_CreateCollection)]

The code snippet does the following:

* Creates a new asset collection with the specified name and parent collection.
* Updates the list of collections in the selected project.
* Prints a message to the console on success.

### Add the UI for viewing and creating collections

To add UI for the example, open the `AssetManagementUI` script you created and replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_UIProject)]

The code snippet does the following:

* Displays a list of the selected project's collections.
* Displays UI buttons and necessary text fields to create a new collection and to select a collection.

### Update an asset collection

To update an asset collection, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_UpdateCollection)]

The code snippet does the following:

* Updates the selected collection's description by incrementing a counter within the text.
* Prints a message to the console on success.

### Delete an asset collection

To delete an asset collection, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_DeleteCollection)]

The code snippet does the following:

* Deletes the selected collection from the project.
* Refreshes the list of collections in the project.
* Prints a message to the console on success.

### Move an asset collection

To move an asset collection either to nest it under another collection or re-parent at the root of the project, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_MoveCollection)]

The code snippet does the following:

* Moves the selected collection to the specified parent collection.
* Prints a message to the console on success.

### Add an asset to a collection

To add an asset to a collection, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_CollectionInsert)]

The code snippet does the following:

* Adds the selected asset to the selected collection.
* Prints a message to the console on success.

### Add the UI for interacting with a collection

To add UI for the example, open the `AssetManagementUI` script you created and replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_UIActions)]

The code snippet does the following:

* Displays UI buttons to update and delete the selected collection.
* Displays a text field and UI button to re-parent the selected collection to another collection.
* Displays a UI button to add the selected asset to the selected collection.
