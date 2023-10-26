# Use case: Create and upload asset files

You can use the Unity Cloud Assets package to view the files of an asset and to upload files to an asset's dataset.

The SDK supports different workflows for users with different roles.

| Asset Manager Project role                                                                             | View files | Upload new file |
|:-------------------------------------------------------------------------------------------------------|------------|:----------------|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)      | yes        | no              |
| [`Asset Management Consumer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)    | yes        | no              |
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) | yes        | yes             |
| [`Asset Management Owner`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)       | yes        | yes             |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Create an asset file

To create a new asset file, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileCreationExample.cs#Example_Behaviour_CreateAssetFile)]

The script creates a new asset file on the selected asset from a default Unity texture.

### Upload an asset file

To upload the associated asset of an asset file:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileCreationExample.cs#Example_Behaviour_UploadAssetFile)]

The script does the following:

* Uploads the file content to the cloud.
* Prints the a message to the console when the upload is complete OR prints an error message if the upload fails.

### Add the UI for interacting with asset files

To add UI for the example:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileCreationExample.cs#Example_UI)]

The script does the following:

* Provides UI buttons to trigger the creation of a new asset file.
* Displays a list of created asset files and provides a UI button to upload the file content.

## Going further

For more a more in-depth look at file management, see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
