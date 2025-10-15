# Use case: Manage an asset's update history

You can use the Unity Cloud Assets package to view and roll back the update history of assets in the Cloud.

>[!NOTE]
>To manage update history, you need an [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at Organization level or an [`Asset Management Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at Project level. Asset Management Contributors can update assets only for the specific projects to which they have access.

## Before you start

Before you start, do the following:

1. Verify your permissions as described in [Asset Manager user roles](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

>[!NOTE]
>Asset Manager roles specify permissions you have for a single Asset Manager project. Depending on your work, permissions can vary across different projects.

2. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See the [Get started with Asset SDK](get-started-management.md#Set-up-a-Unity-scene) page for more information.
3. Create assets in the Cloud any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [single](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) assets through the dashboard.

## How do I...?

### View the update history of an asset

View update history for an asset as follows:

1. Open the `AssetManagementBehaviour` script you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetChangeHistory.cs#Example_Behaviour_GetHistory)]

The code snippet loads and stores the update history of the selected asset. It includes the option to include update history of its datasets and files.

### View the update history entry of a child entity

Load the update history of a child entity (dataset or file) as follows:

1. Open the `AssetManagementBehaviour` script you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetChangeHistory.cs#Example_Behaviour_GetChildHistory)]

The code snippet does the following:

* Provides a method to load and store the update history entry of a dataset.
* Provides a method to load and store the update history entry of a file.
* Provides a method to get the stored update history of a dataset.
* Provides a method to get the stored update history of a file.

### Update the properties of an asset, dataset, or file to a previous update history state

Update an asset to a previous update history as follows:

1. Open the `AssetManagementBehaviour` script you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetChangeHistory.cs#Example_Behaviour_RollbackHistory)]

The code snippet does the following:

* Provides a method to update an asset to a previous update history state.
* Provides a method to update a dataset to a previous update history state.
* Provides a method to update a file to a previous update history state.

### View the entire update history of a dataset

View the entire update history of a dataset as follows:

1. Open the `AssetManagementBehaviour` script you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetChangeHistory.cs#Example_Behaviour_GetHistory_Dataset)]

The code snippet provides a method which returns an enumerable list of all update history entries of a dataset.

### View the entire update history of a file

View the entire update history of a file as follows:

1. Open the `AssetManagementBehaviour` script you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetChangeHistory.cs#Example_Behaviour_GetHistory_File)]

The code snippet provides a method which returns an enumerable list of all update history entries of a file.

### Add a UI to view the update history of an asset

Create a UI to update an asset as follows:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseManageAssetUpdateHistoryExampleUI`.
5. Open the `UseCaseManageAssetUpdateHistoryExampleUI` script you created in the previous step and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetChangeHistory.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_UIContent)]

7. Open the `AssetManagementUI` script you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseUpdateHistory)]

The UI does the following:

* Displays a list of update history entries for the selected asset. Next to each entry is displayed a **Select** and a **Roll back** UI button.
* Displays a toggle to reload the list with or without child dataset and file entries.
* When you select an entry, additional UI elements display the state of the asset properties.
  * If the entry includes a change to a dataset or file, a button is displayed to load the update history entry of the child entity.
