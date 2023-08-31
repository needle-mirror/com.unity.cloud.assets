# Manage asset collections in a project

You can use the Unity Asset Manager SDK package to manage your collections of assets.

Collection management is only available through the Asset Management pathway.

> **Note**: Collection management requires users have the role of [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) OR a minimum role of [`Manager`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in their organization.

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an organization and project browser. See either [Get started with Asset Discovery](get-started-discovery.md) or [Get started with Asset Management](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Asset Management](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List the collections in a project

To list the existing collections in a project:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseCollectionManagementExample.cs#Example_Behaviour_RefreshCollections)]

The code snippet does the following:

* Populates a list of the collections in the selected project.
* Holds a reference to the selected collection.

### Create an asset collection

To create an asset collection:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_CreateCollection)]

The code snippet does the following:

* Creates a new asset collection with the specified name and parent collection.
* Updates the list of collections in the selected project.
* Prints a message to the console on success.

### Add the UI for viewing and creating collections

To add UI for the example:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_UIProject)]

The code snippet does the following:

* Displays a list of the selected project's collections.
* Displays UI buttons and necessary text fields to create a new collection and to select a collection.

### Update an asset collection

To update an asset collection:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_UpdateCollection)]

The code snippet does the following:

* Updates the selected collection's description by incrementing a counter within the text.
* Prints a message to the console on success.

### Delete an asset collection

To delete an asset collection:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_DeleteCollection)]

The code snippet does the following:

* Deletes the selected collection from the project.
* Refreshes the list of collections in the project.
* Prints a message to the console on success.

### Move an asset collection

To move an asset collection either to nest it under another collection or re-parent at the root of the project: 

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_MoveCollection)]

The code snippet does the following:

* Moves the selected collection to the specified parent collection.
* Prints a message to the console on success.

#### Known issues

* Nesting a collection on creation or by moving it results in the collection being unusable. It cannot be moved again or deleted and assets cannot be added or removed from it. This is a known issue and will be fixed in a future release.

### Add an asset to a collection

To add an asset to a collection: 

1. Open the `AssetManagementBehaviour` script you created. 
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_CollectionInsert)]

The code snippet does the following:

* Adds the selected asset to the selected collection.
* Prints a message to the console on success.

### Add the UI for interacting with a collection

To add UI for the example: 

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_UIActions)]

The code snippet does the following:

* Displays UI buttons to update and delete the selected collection.
* Displays a text field and UI button to re-parent the selected collection to another collection.
* Displays a UI button to add the selected asset to the selected collection.
