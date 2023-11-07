# Use case: Manage an asset's files

You can use the Unity Cloud Assets package to edit file metadata and download file content.

The SDK supports different workflows for users with different roles.

| Asset Manager Project role                                                                             | Download files | Edit files |
|:-------------------------------------------------------------------------------------------------------|----------------|:-----------|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)      | no             | no         |
| [`Asset Management Consumer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)    | yes            | no         |
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) | yes            | yes        |
| [`Asset Management Owner`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)       | yes            | yes        |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

You should also have uploaded files to an asset; see the [Create files use case](use-case-create-asset-files.md).

## How do I...?

### List the files of an asset

By default, when you get an asset, the files associated with are not included in the response.
To get files associated to an asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_Behaviour_RefreshFiles)]

The code snippet populates the `Files` property of the selected asset.

### Download a file

To download the file of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_Behaviour_DownloadAssetFile)]

The code snippet does the following:

* Gets the files of an asset.
* Downloads the selected file to the desktop.
* Prints a message to the console when the download is complete OR prints an error message if the download fails.

### Update a file

To update a file, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_Behaviour_UpdateAssetFile)]

The code snippet does the following:

* Increments the index in the name of the file.
* Prints a message to the console on success.

### Delete a file

Deleting a file involves removing all references to the file from the asset.
For more information see the use case for [Removing a file reference from a dataset](use-case-create-asset-files.md#remove-a-file-reference-from-a-dataset).

### Add the UI for interacting with files

To add UI for the example, follow these steps:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileManagementExample.cs#Example_UI)]

The code snippet does the following:

* Displays a button to force refresh the list of files of the selected asset.
* Displays each file of the selected asset with a UI buttons to update and download.

## Going further

For more a more in-depth look at file management, see the [Asset Database Uploader sample](asset-database-uploader-sample.md).
