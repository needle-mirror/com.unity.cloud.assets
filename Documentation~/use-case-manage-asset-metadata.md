# Use case: Manage an asset's metadata

Use the Unity Cloud Assets package to perform the following:

* View the metadata of an asset.
* Add or remove a metadata entry from an asset.
* Update the value of the metadata entry of an asset.

>[!NOTE]
>To manage assets, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.

## Before you start

Before you start, do the following:

1. Verify you have the required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

   >[!NOTE]
   >Asset Manager roles define the permissions that you have for a single Asset Manager project. Depending on your work, permissions may vary across projects.

2. Create assets in Unity Cloud in any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [a single asset](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple assets](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) through the dashboard.

## How do I...?

### View the metadata of an asset

To fetch the metadata of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#Example_Behaviour_FetchMetadata)]

The code snippet fills a dictionary with metadata.

### Add or update entries in an asset's metadata

> [!NOTE] Metadata keys must be pre-existing in your organization's library.
> Read more about [adding and removing metadata keys](use-case-manage-fields.md).

To add or update an entry in the metadata of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#Example_Behaviour_UpdateMetadata)]

The code snippet does the following:

* Adds or updates the specified metadata key with the provided value for the selected asset.
* Displays a success message in the console.

### Remove an entry from an asset's metadata

To remove an entry from the metadata:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#Example_Behaviour_RemoveMetadata)]

The code snippet does the following:

* Removes the specified metadata key from the selected asset.
* Displays a success message in the console.

### Add a UI for viewing and modifying the metadata of an asset

To create the UI, start by creating helper classes:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `BooleanMetadataValueDisplayer`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#HelperClass_BooleanDisplay)]

6. Repeat steps 3, 4 and 5 to create subsequent helper classes with the following scripts and code samples:

* For the `NumberMetadataValueDisplayer` script, use the following code sample:
[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#HelperClass_NumberDisplay)]
* For the `UrlMetadataValueDisplayer` script, use the following code sample:
[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#HelperClass_UrlDisplay)]
* For the `SingleSelectionMetadataValueDisplayer` script, use the following code sample:
[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#HelperClass_SingleSelectionDisplay)]
* For the `MultiSelectionMetadataValueDisplayer` script, use the following code sample:
[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#HelperClass_MultiSelectionDisplay)]
* For the `TextMetadataValueDisplayer` script, use the following code sample:
[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#HelperClass_TextDisplay)]

To complete the UI, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseAssetMetadataExampleUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#Example_UIContent)]

7. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the content of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseAssetMetadata)]

The code snippet displays the following UI elements:

* The list of metadata
* For each metadata key, the **Select** and **Delete** buttons
* If you select a metadata key:

   * An editable field that contains the key value
   * The **Update** button to save your modifications

      > [!NOTE] This editable field displays only when you select a metadata key.
