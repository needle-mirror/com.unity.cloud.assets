# Use case: Update datasets

You can use the Unity Cloud Assets package to view and update existing datasets within an asset.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | View datasets | Update datasets |
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

If you have not already done so, you must update the behaviour to get the list of datasets for an asset.

See the [Use case: Create datasets](use-case-create-datasets.md#list-datasets) documentation for more information.

### Update a dataset

To update an existing dataset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageDatasetExample.cs#Example_Behaviour_UpdateDataset)]

The code snippet updates an existing dataset and refreshes the display.

### Add the UI for listing datasets

To create UI for listing datasets, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseCreateDatasetExampleUI`.
4. Open the `UseCaseCreateDatasetExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageDatasetExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageDatasetExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseUpdateDatasets)]

The code snippet does the following:

* Displays a list of datasets for the selected asset with a UI button to select the dataset for update.
* When a dataset is selected, the UI displays UI input fields and a UI button to update the properties of the dataset.
* When the dataset is updated, a message is logged to the console to confirm the update.
