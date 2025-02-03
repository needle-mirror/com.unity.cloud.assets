# Use case: Manage references between assets

Use the Unity Cloud Assets package to perform the following:

* Query the references of an asset.
* Create or delete references between assets.

>[!NOTE]
>To manage assets, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.

## Before you start

Before you start, do the following:

1. Verify you have the required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

   >[!NOTE]
   >Asset Manager roles define permissions that you have for a single Asset Manager project. Depending on your work, permissions might vary across different projects.

2. Create assets in Unity Cloud in any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [a single asset](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple assets](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) through the dashboard.


## How do I...?

### List the references of an asset

To list the references of an asset, do the following:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_Behaviour_ListReferences)]

The code snippet generates the selected asset's reference list.

### Add a reference between assets

To add a reference between assets, do the following:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_Behaviour_CreateReference)]

The code snippet creates a new reference between a source asset and a target asset. A message in the console confirms the creation.

### Remove a reference between assets

To remove a reference between assets, do the following:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets). 
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_Behaviour_RemoveReference)]

The code snippet removes the reference between the source asset and the target asset. A message in the console confirms the deletion.

### Add a UI for viewing and creating references

To create a UI for displaying asset references, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseManageAssetReferencesExampleUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_UIContent)]

7. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseManageAssetReferences)]

The code snippet displays the following UI elements:

* A selection grid to specify which entry to update on asset selection: the Source or Target.
* A list of references for the selected Source asset.
* A list of references for the selected Target asset.
* A UI button next to each reference to remove the reference.
* A UI button to create a reference between the selected Source and Target assets.
