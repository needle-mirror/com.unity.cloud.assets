# Use case: Download asset files

You can use the Unity Cloud Assets package to download the files of an asset.

| Asset Manager Project role                                                                             | Download files |
|:-------------------------------------------------------------------------------------------------------|:---------------|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)      | no             |
| [`Asset Management Consumer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)    | yes            |
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) | yes            |
| [`Asset Management Owner`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)     | yes            |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Asset Management](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Asset Management](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Download an asset file

To download the associated asset file of an asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseDownloadFileExample.cs#Example_Behaviour_DownloadAssetFile)]

The script does the following:

* Downloads the file to the desktop.
* Prints the a message to the console when the download is complete OR prints an error message if the download fails.

### Add the UI for interacting with asset files

To add UI for the example:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseDownloadFileExample.cs#Example_UI)]

The script does the following:

* Provides UI buttons to trigger the download of an asset file.
* Displays a list of the asset files of an asset and provides a UI button to download the file.
