# Use case: Update an asset's files

You can use the Unity Cloud Assets package to edit file metadata and download file content.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | Download files | Update files |
|:-----------------------------------------------------------------------------------------------------|----------------|:-------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no             | no           |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes            | no           |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes            | yes          |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes            | yes          |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

You should also have uploaded files to an asset; see the [Create files use case](use-case-create-files.md).

## How do I...?

### List the files of a dataset

If you have not already done so, you must update the behaviour to get the list of files for an asset's dataset.

See the [Use case: Create datasets](use-case-create-files.md#list-a-datasets-files) documentation for more information.

### Download a file

To download the file of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFileExample.cs#Example_Behaviour_DownloadAssetFile)]

The code snippet does the following:

* Gets the files of an asset.
* Downloads the selected file to the desktop.
* Prints a message to the console when the download is complete OR prints an error message if the download fails.

### Update a file

The properties of the file that can be updated are the following:

* Description
* Tags

To update a file, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFileExample.cs#Example_Behaviour_UpdateAssetFile)]

The code snippet does the following:

* Updates the file with new data.
* Prints a message to the console on success.

### Delete a file

Deleting a file involves removing all references to the file from the asset.
For more information see the use case for [Removing a file reference from a dataset](use-case-create-files.md#remove-a-file-reference-from-a-dataset).

### Generating tags for a file

The service can generate a list of suggested tags for any image files in the following supported formats:

* JPEG
* PNG
* GIF
* TIFF
* WebP

The desired tags can then be added to the file through the update method.

To generate tags for a file, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFileExample.cs#Example_Behaviour_GenerateFileTags)]

The code snippet does the following:

* Returns a list of generated tags for the file.
* Prints any errors to the console.

### Add the UI for interacting with files

To create UI for interacting with files, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseManageFileExampleUI`.
4. Open the `UseCaseManageFileExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFileExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFileExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseFileManagement)]

The code snippet does the following:

* Displays a button to force refresh the list of files of the selected asset.
* Displays each file of the selected asset with a UI buttons to select and download.
* When a file is selected, displays a UI to generate tags and update the file's description and tags.
