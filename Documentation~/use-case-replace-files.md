# Use case: Replace uploaded files content

You can use the Unity Cloud Assets package to upload new content for a file.

>[!NOTE] 
>Once a file is uploaded, its content cannot be modified. If you want to change the content of a file, you must:
> 
>1. Remove all references to this file, effectively deleting it.
>2. You may upload a new file with the same name.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | Upload file content |
|:-----------------------------------------------------------------------------------------------------|:--------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no                  |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | no                  |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes                 |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes                 |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can create assets through the dashboard, see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

You should also have uploaded files to an asset, see the [Create files use case](use-case-create-files.md).

## How do I...?

### List the files of an asset

By default, when you get an asset, the files associated with are not included in the response.
To get files associated to an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileManagementExample.cs#Example_Behaviour_RefreshFiles)]

The code snippet populates the `Files` property of the selected asset.

### Replace a file

To replace the content of a file, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileReuploadExample.cs#Example_Behaviour_UploadFile)]

The code snippet does the following:

* Removes the file from each dataset it is referenced in.
* Creates and uploads the file as a new file to one of the datasets.
* Adds a reference to the new file in each dataset the old file was referenced in.

### Add the UI for re-uploading file content

To create UI for re-uploading file content, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseFileReuploadExampleUI`.
4. Open the `UseCaseFileReuploadExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileReuploadExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileReuploadExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseReplaceFiles)]

The script does the following:

* Displays a text field to enter a path to a file to upload.
* Displays a list of the selected asset's files with a button to replace the file.
* Displays a button to cancel the request once started.
