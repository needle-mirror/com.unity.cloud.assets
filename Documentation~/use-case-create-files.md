# Use case: Create and upload files to a dataset

Use the Unity Cloud Assets package to perform the following:

* View the files of an asset.
* Upload files to an asset's dataset.
* Reference files between datasets.

>[!NOTE]
>To create and upload assets, as well as add or remove file references, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.

## Before you start

Before you start, do the following:

1. Verify you have the required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

   >[!NOTE]
   >Asset Manager roles specify permissions you have for a single Asset Manager project. Depending on your work, permissions may vary across different projects.

2. Set up a Unity scene in the Unity Editor with an Organization and Project browser. Read more about [setting up a Unity scene](get-started-management.md#Set-up-a-Unity-scene).
3. Create assets in the cloud in any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [a single asset](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple assets](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) through the dashboard.

## How do I...?

### List a dataset's files

If you have not already done so, you must update the behaviour to get the list of datasets for an asset.

See the [Use case: Create datasets](use-case-create-datasets.md#list-datasets) documentation for more information.

To list files, do the following:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateFileExample.cs#Example_Behaviour_RefreshFiles)]

### Upload a file

Upload a file to an asset's dataset as follows:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateFileExample.cs#Example_Behaviour_UploadAssetFile)]

The code snippet does the following:

* Provides a method to upload a new file to a dataset.
* Provides a method to replace the content of an existing file in a dataset.
* Provides a method to replace a file in a dataset with another file.

### Upload a folder

Upload a folder to an asset's dataset as follows:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateFileExample.cs#Example_Behaviour_UploadFolder)]

The code snippet provides a method to upload the entire content of a folder to a dataset.

>[!NOTE]
>The method checks for existing files in the dataset and replaces their content. If an existing file is not found, the method uploads the file as new.

### Reference a file in a different dataset

Reference a file in a different dataset as follows:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateFileExample.cs#Example_Behaviour_AddFileReference)]

The code snippet does the following:

* Links a file to a dataset.
* Displays a success message in the console or an error message if the linking fails.

### Remove a file reference from a dataset

Remove a file reference from a dataset as follows:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateFileExample.cs#Example_Behaviour_RemoveFileReference)]

The code snippet does the following:

* Unlinks a file from a dataset.
* Displays a success message in the console or an error message if the unlinking fails.

### Add a UI to create files

Add a UI to create files as follows:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseCreateFileExampleUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateFileExample.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateFileExample.cs#Example_UIContent)]

7. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseCreateFiles)]

The code snippet does the following:

* Provides a UI button to refresh the list of files within each dataset.
* Provides UI buttons to trigger the creation of a new file within a dataset.
* Provides a UI button to link an existing file to a dataset.
* Provides a UI button for each existing file to unlink it from its dataset.

## Going further

### Update and download files

Read more about [downloading and managing files](use-case-update-files.md).

### Replace uploaded file content

Read more about [replacing uploaded file content](use-case-replace-files.md).
