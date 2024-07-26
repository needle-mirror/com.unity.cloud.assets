# Use case: Update the status of assets

You can use the Unity Cloud Assets package to update the status of assets in the cloud.

| Organization or Asset Manager Project role                                                           | View status | Update status |
|:-----------------------------------------------------------------------------------------------------|:------------|:--------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no          | no            |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes         | no            |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes         | yes           |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes         | yes           |

>[!NOTE]
>Asset management requires users have the role of [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) OR a minimum role of [`Manager`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) in the Organization.

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### View the status of an asset

To get the current status of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_Behaviour_GetCurrentStatus)]

The code snippet sets the current status of the asset.

### View reachable statuses for an asset

To view the next available statuses of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_Behaviour_GetReachableStatuses)]

The code snippet sets the collection of next available statuses for the asset.

### Update the status of an asset

To update the status of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_Behaviour_UpdateStatus)]

### Add the UI for viewing and updating the status of assets

To create UI for updating the status of assets, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseManageAssetStatusExample`.
4. Open the `UseCaseManageAssetStatusExample` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetStatusExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseManageAssetStatus)]

The code snippet displays the current status of the asset and provides UI buttons to update the asset to a next available status.
