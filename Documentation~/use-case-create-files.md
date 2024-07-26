# Use case: Create and upload files to a dataset

You can use the Unity Cloud Assets package to:

* view the files of an asset.
* upload files to an asset's dataset.
* reference files between datasets.

The SDK supports several workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | View files | Upload new file | Add/remove file references |
|:-----------------------------------------------------------------------------------------------------|------------|:----------------|----------------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes        | no              | no                         |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes        | no              | no                         |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes        | yes             | yes                        |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes        | yes             | yes                        |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List an asset's datasets and files

To list datasets, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileCreationExample.cs#Example_Behaviour_RefreshDatasets)]

To list files, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileCreationExample.cs#Example_Behaviour_RefreshFiles)]

### Upload a file

To upload a file to an asset's dataset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileCreationExample.cs#Example_Behaviour_UploadAssetFile)]

The code snippet does the following:

* Provides a method to upload a new file to a dataset.
* Provides a method to replace the content of an existing file in a dataset.
* Provides a method to replace a file in a dataset with another file.

### Reference a file in a different dataset

To reference a file in a different dataset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileCreationExample.cs#Example_Behaviour_AddFileReference)]

The code snippet does the following:

* Links a file to a dataset.
* Prints a message to the console on success OR an error message if the linking fails.

### Remove a file reference from a dataset

To remove a file reference from a dataset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileCreationExample.cs#Example_Behaviour_RemoveFileReference)]

The code snippet does the following:

* Unlinks a file from a dataset.
* Prints a message to the console on success OR an error message if the unlinking fails.

### Add the UI for creating files

To create UI for creating files, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseFileCreationExampleUI`.
4. Open the `UseCaseFileCreationExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileCreationExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFileCreationExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseCreateFiles)]

The code snippet does the following:

* Provides a UI button to refresh the list of files within each dataset.
* Provides UI buttons to trigger the creation of a new file within a dataset.
* Provides a UI button to link an existing file to a dataset.
* Provides a UI button for each existing file to unlink it from its dataset.
