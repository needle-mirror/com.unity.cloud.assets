# Create and upload asset files

You can use the Unity Cloud Assets package to manage the files linked to an asset.

File management is only available through the Asset Management pathway.
> Note: File management requires users have the role of `Asset Management Contributor` OR a minimum role of `Manager` in the organization.

## Prerequisites

Before you start, set up a Unity scene in with an organization and project browser.
See [Get started with Asset Management](get-started-management.md) for more information.

Next, you must have some assets uploaded to the cloud.
You can either create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## Methodology

### Create an asset file

To create a new asset file, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileCreationExample.cs#Example_Behaviour_CreateAssetFile)]

The script does the following:

* Creates a new asset file on the selected asset from a default unity texture.

### Upload an asset file

To upload the associated asset of an asset file, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileCreationExample.cs#Example_Behaviour_UploadAssetFile)]

The script does the following:

* Uploads the file content to the cloud.
* Prints the a message to the console when the upload is complete OR prints an error message if the upload fails.

### Add the UI for interacting with asset files

To add UI for the example, open the `AssetManagementUI` script you created and replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileCreationExample.cs#Example_UI)]

The script does the following:

* Provides UI buttons to trigger the creation of a new asset file.
* Displays a list of created asset files and provides a UI button to upload the file content.

## Going further

For more a more in-depth look at file management, see the [Asset database uploader sample](./asset-database-uploader-sample.md).
