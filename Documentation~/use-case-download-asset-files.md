# Download asset files

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
