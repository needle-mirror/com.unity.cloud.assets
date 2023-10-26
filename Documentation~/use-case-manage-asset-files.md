# Use case: Manage asset files

You can use the Unity Cloud Assets package to add or remove references of files from other datasets.

The SDK supports different workflows for users with different roles.

| Asset Manager Project role                                                                             | Edit files | Add/remove file references |
|:-------------------------------------------------------------------------------------------------------|:-----------|:---------------------------|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)      | no         | no                         |
| [`Asset Management Consumer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)    | no         | no                         |
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) | yes        | yes                        |
| [`Asset Management Owner`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)     | yes        | yes                        |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

You should also have uploaded files to an asset; see the [Create asset files use case](use-case-create-asset-files.md).

## How do I...?

### List the asset files of an asset

By default, when you get an asset, the files associated with are not included in the response.
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
