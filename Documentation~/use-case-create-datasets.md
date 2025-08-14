# Use case: Create datasets

You can use the Unity Cloud Assets package to view and create datasets within an asset.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | View datasets | Create datasets |
|:-----------------------------------------------------------------------------------------------------|:--------------|-----------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes           | no              |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes           | no              |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes           | yes             |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes           | yes             |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Asset Management](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Asset Management](get-started-management.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List datasets

To list the datasets of an asset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateDatasetExample.cs#Example_Behaviour_RefreshDatasets)]

The code snippet populates a list of datasets for the selected asset.

### Create a dataset

To create a new dataset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateDatasetExample.cs#Example_Behaviour_CreateDataset)]

The code snippet creates a new dataset with the given name and the tag `Custom` on the selected asset.

### Add the UI for listing datasets

To create UI for listing datasets, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseCreateDatasetExampleUI`.
4. Open the `UseCaseCreateDatasetExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateDatasetExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateDatasetExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseCreateDatasets)]

The code snippet does the following:

* Displays a list of datasets for the selected asset.
* Provides a text input and UI button to create a new dataset.
* Logs a confirmation message to the console when a dataset is created.
