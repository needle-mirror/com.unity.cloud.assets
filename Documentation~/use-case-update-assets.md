# Use case: Update an asset's properties

You can use the Unity Cloud Assets package to update the status of assets in the cloud.

| Organization or Asset Manager Project role                                                           | Update assets |
|:-----------------------------------------------------------------------------------------------------|:--------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no            |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | no            |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes           |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes           |

>[!NOTE]
>Asset management requires users have the role of [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) OR a minimum role of [`Manager`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) in the Organization.

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Update an asset

To update an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_Behaviour_UpdateAsset)]

The code snippet does the following:

* Provides a method to update the selected asset.

### Clear the properties of an asset

Because any properties of `IAssetUpdate` which are set to null will not be updated, the fields that you want to clear must be explicitly set to empty.

You can clear the description of an asset, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_Behaviour_ClearDescription)

You can clear the tags of an asset, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_Behaviour_ClearTags)

You can clear the default preview of an asset, like so:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_Behaviour_ClearPreviewFile)

>[!NOTE]
>You cannot clear the name of an asset. The name must always be a non-empty value.

### Add the UI for viewing and updating the properties of assets

To create UI for updating an asset, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseManageAssetExampleUI`.
4. Open the `UseCaseManageAssetExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseAssetUpdate)]

The ui does the following:

* Displays information about the selected asset.
* Provide fields to update the name, type and tags of the selected asset.
