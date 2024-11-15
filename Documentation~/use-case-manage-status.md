# Use case: Update the status of assets

Use the Unity Cloud Assets package to update the status of assets in the cloud.

>[!NOTE]
>To update assets, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.

## Before you start

Before you start, do the following:

1. Verify you have the required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

   >[!NOTE]
   >Asset Manager roles define the permissions that you have for a single Asset Manager project. Depending on your work, permissions may vary across projects.

2. Set up a Unity scene in the Unity Editor with an Organization and Project browser. Read more about [setting up a Unity scene](get-started-management.md#Set-up-a-Unity-scene).
3. Create assets in Unity Cloud any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [a single asset](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple assets](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) through the dashboard.

## How do I...?

### View reachable statuses for an asset

To view the next available statuses of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_Behaviour_GetReachableStatuses)]

The code snippet sets a collection of the next available statuses for the asset.

### Update the status of an asset

To update the status of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_Behaviour_UpdateStatus)]

### Add a UI for viewing and updating the status of assets

To create a UI for updating the status of assets, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseManageAssetStatusExample`.
5. Open the `UseCaseManageAssetStatusExample` script that you created in the previous step and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseManageAssetStatus)]

The code snippet does the following:
* Displays the current status of the asset.
* Provides UI buttons to update the asset to the next available status.
