# Use case: Manage asset collections in a project

You can use the Unity Cloud Assets package to create, delete, and edit an asset collection in a project.


| Asset Manager Project role                                                                             | Getting collections | Create/delete/edit collections | Add/remove assets in collections |
|:-------------------------------------------------------------------------------------------------------|:--------------------|--------------------------------|:---------------------------------|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)      | yes                 | no                             | no                               |
| [`Asset Management Consumer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)    | yes                 | no                             | no                               |
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) | yes                 | no                             | no                               |
| [`Asset Management Owner`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)       | yes                 | yes                            | yes                              |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List the collections in a project

To list the existing collections in a Project, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_RefreshCollections)]

The code snippet does the following:

* Populates a list of the collections in the selected Project.
* Holds a reference to the selected collection.

### Create an asset collection

To create an asset collection, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_CreateCollection)]

The code snippet does the following:

* Creates a new asset collection with the specified name and parent collection.
* Updates the list of collections in the selected Project.
* Prints a message to the console on success.

### Add the UI for viewing and creating collections

To add UI for the example, follow these steps:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_UIProject)]

The code snippet does the following:

* Displays a list of the selected Project's collections.
* Displays UI buttons and necessary text fields to create a new collection and to select a collection.

### Update an asset collection

To update an asset collection, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_UpdateCollection)]

The code snippet does the following:

* Updates the selected collection's description by incrementing a counter within the text.
* Prints a message to the console on success.

### Delete an asset collection

To delete an asset collection, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_DeleteCollection)]

The code snippet does the following:

* Deletes the selected collection from the Project.
* Refreshes the list of collections in the Project.
* Prints a message to the console on success.

### Move an asset collection

To move an asset collection either to nest it under another collection or re-parent at the root of the project, follow these steps: 

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_MoveCollection)]

The code snippet does the following:

   * Moves the selected collection to the specified parent collection.
   * Prints a message to the console on success.

### Add an asset to a collection

To add an asset to a collection, follow these steps: 

1. Open the `AssetManagementBehaviour` script you created. 
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_CollectionInsert)]

The code snippet does the following:

   * Adds the selected asset to the selected collection.
   * Prints a message to the console on success.

### Remove an asset from a collection

To remove an asset from a collection, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_Behaviour_CollectionRemove)]

The code snippet does the following:

* Removes the target asset from the selected collection.
* Prints a message to the console on success.

### Add the UI for interacting with a collection

To add UI for the example, follow these steps:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseManageCollectionsExample.cs#Example_UIActions)]

The code snippet does the following:

   * Displays UI buttons to update and delete the selected collection.
   * Displays a text field and UI button to re-parent the selected collection to another collection.
   * Displays a UI button to add the selected asset to the selected collection.
   * Displays the list of assets in the selected collection. Each asset has a UI button to remove it from the selected collection.
