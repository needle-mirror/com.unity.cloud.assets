# Manage asset files

You can use the Unity Asset Manager SDK package to manage the files linked to an asset.

File management is only available through the Asset Management pathway.

> **Note**: File management requires users have the role of [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) OR a minimum role of [`Manager`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in the organization.

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an organization and project browser. See either [Get started with Asset Discovery](get-started-discovery.md) or [Get started with Asset Management](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Asset Management](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

You should also have uploaded files to an asset; see the [Create asset files use case](use-case-create-asset-files.md).

## How do I...?

### Getting the asset files of an asset

By default, an asset's files are not included when you get an asset.
To get files associated to an asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_Behaviour_RefreshAssetFiles)]

The script populates the `Files` property of the selected asset.

### Get an asset file's download URL

To get the download URL of an asset file:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_Behaviour_DownloadUrls)]

The script prints the download URL of the specified asset file to the console.

### Update an asset file

To update an asset file:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_Behaviour_UpdateAssetFile)]

The script does the following:

* Increments the index in the name of the asset file.
* Prints a message to the console on success.

### Delete an asset file

To delete an asset file:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_Behaviour_DeleteAssetFile)]

The script does the following:

* Deletes the asset file.
* Refreshes the list of files for the selected asset.
* Prints a message to the console on success.

### Add the UI for interacting with asset files

To add UI for the example:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_UI)]

The script does the following:

* Displays a list of the selected asset's asset files.
* Displays UI buttons to update, delete, and output the download URL of each asset file.

## Going further

For more a more in-depth look at file management, see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
