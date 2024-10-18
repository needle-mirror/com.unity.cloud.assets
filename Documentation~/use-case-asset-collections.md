# Use case: Manage an asset's association to collections

You can use the Unity Cloud Assets package to perform the following:

* List the collections to which an asset belongs.
* Add or remove an asset from a collection.

>[!NOTE]
>To manage assets, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.

## Before you start

Before you start, do the following:

1. The required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

>[!NOTE]
>Asset Manager roles define permissions you have for a single Asset Manager project. Depending on your work, permissions can vary across different projects.

2. Create assets in Unity Cloud any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [single](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) assets through the dashboard.

## How do I...?

### List the collections of an asset

By default, an asset's collections are not included when you get an asset. To fetch the collections of an asset, do the following:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_Behaviour_RefreshCollections)]

The code snippet generates the selected asset's collection list.

### Remove an asset from a collection

To remove an asset from a collection, do the following:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets). 
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_Behaviour_RemoveFromCollection)]

The code snippet does the following:

* Removes the selected asset from the specified collection.
* Updates the selected asset's collection list.
* Displays a success message in the console.

### Add a UI for viewing and creating collections

To create a UI for displaying asset collections, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseAssetCollectionExampleUI`.
5. Open the `UseCaseAssetCollectionExampleUI` script that you created in the previous step and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_UIContent)]

7. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseAssetCollections)]

The code snippet displays the following UI elements:

* A UI button to refresh the selected asset's collections.
* A list of the selected asset's collections.
* A UI button next to each collection to remove the selected asset from it.
