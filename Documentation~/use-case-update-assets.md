# Use case: Update an asset's properties

You can use the Unity Cloud Assets package to update the status of assets in the Cloud.

>[!NOTE]
>To update assets, you need an [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at Organization level or an [`Asset Management Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at Project level. Asset Management Contributors can update assets only for the specific projects to which they have access.

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

### Update an asset

Update an asset as follows:

1. Open the `AssetManagementBehaviour` script you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_Behaviour_UpdateAsset)]

The code snippet provides a method to update the selected asset.

### Clear the properties of an asset

Properties of `IAssetUpdate` set to null are not updated. Therefore, set the fields you want to clear explicitly empty.

#### Clear the description

Clear the description of an asset as follows:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_Behaviour_ClearDescription)]

#### Clear the tags

Clear the tags of an asset as follows:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_Behaviour_ClearTags)]

#### Clear the default preview

Clear the default preview of an asset as follows:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_Behaviour_ClearPreviewFile)]

>[!NOTE]
>You cannot clear the name of an asset. The name must always be a non-empty value.

### Add a UI to view and update asset properties

Create a UI to update an asset as follows:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseManageAssetExampleUI`.
5. Open the `UseCaseManageAssetExampleUI` script you created in the previous step and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example_UIContent)]

7. Open the `AssetManagementUI` script you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseAssetUpdate)]

The UI does the following:

* Displays information about the selected asset.
* Provide fields to update the name, type and tags of the selected asset.
