# Use case: Manage asset to asset references

You can use the Unity Cloud Assets package to:

* Query the references of an asset.
* Create and delete references between assets.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | List an asset's references | Add/remove asset references |
|:-----------------------------------------------------------------------------------------------------|:---------------------------|:----------------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes                        | no                          |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes                        | no                          |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes                        | yes                         |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes                        | yes                         |

## Before you start

Before you start, you need some assets in the cloud. There are several ways to do so:

* You can create assets through the [Get started with Assets](get-started-management.md).
* You can create assets through the dashboard, see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List the references of an asset

To list the references of an asset, do the following:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_Behaviour_ListReferences)]

The code snippet populates a list of the references of the specified asset.

### Add a reference between assets

To add a reference between assets, do the following:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_Behaviour_CreateReference)]

The code snippet creates a new reference between a source asset and a target asset and prints a message to the console on success.

### Remove a reference between assets

To remove a reference between assets, do the following:

1. Open the `AssetManagementBehaviour` script you created. 
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_Behaviour_RemoveReference)]

The code snippet removes the reference between the source asset and the target asset and prints a message to the console on success.

### Add the UI for viewing and creating references

To create UI for displaying asset references, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseManageAssetReferencesExampleUI`.
4. Open the `UseCaseManageAssetReferencesExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetReferencesExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseManageAssetReferences)]

The code snippet displays the following UI elements:

* A selection grid to specify which entry to update on asset selection; the Source or Target.
* A list of references for the selected Source asset.
* A list of references for the selected Target asset.
* UI buttons beside each reference to remove the reference.
* A UI button to create a reference between the selected Source and Target assets.
