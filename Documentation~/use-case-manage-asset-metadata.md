# Use case: Manage an asset's metadata

You can use the Unity Cloud Assets package to:

* View the metadata of an asset.
* Add or remove a metadata entry from an asset.
* Update the value of the metadata entry of an asset.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role  | View metadata | Update metadata |
|:--------------------------------------------|:--------------|:----------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes           | no              |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes           | no              |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes           | yes             |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes           | yes             |

## Before you start

Before you start, you need assets in the cloud. There are several ways to do so:

* You can create assets through the [Get started with Assets](get-started-management.md).
* You can upload assets from existing Unity assets, see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
* You can create assets through the dashboard, see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### View the metadata of an asset

To fetch the metadata of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#Example_Behaviour_FetchMetadata)]

The code snippet populates a dictionary of metadata.

### Add or update entries in an asset's metadata

> [!NOTE] The keys for metadata must already exist in your organization's library.
> To add and remove metadata keys, see the [Manage the field definitions in an organization](use-case-manage-fields.md).

To add or update an entry in the metadata of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#Example_Behaviour_UpdateMetadata)]

The code snippet does the following:

* Adds or updates the specified metadata key with the provided value for the selected asset.
* Prints a message to the console on success.

### Remove an entry from an asset's metadata

To remove an entry from the metadata:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#Example_Behaviour_RemoveMetadata)]

The code snippet does the following:

* Removes the specified metadata key from the selected asset.
* Prints a message to the console on success.

### Add the UI for viewing and modifying the metadata of an asset

To create the UI, begin by creating helper classes:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `BooleanMetadataValueDisplayer`.
4. Open the `BooleanMetadataValueDisplayer` script you created and replace the content of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetMetadataExample.cs#HelperClass_BooleanDisplay)]

5. Repeat steps 3 and 4 to create the subsequent helper classes with the following scripts and code samples:

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

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseAssetMetadataExampleUI`.
4. Open the `UseCaseAssetMetadataExampleUI` script you created and replace the content of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the content of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseAssetMetadata)

The code snippet displays the following UI elements:

* A list of metadata with **Select** and **Delete** UI buttons for each metadata key.
* When you select a metadata key, the UI displays an editable field containing the value and a UI button to **Update** the metadata value.
