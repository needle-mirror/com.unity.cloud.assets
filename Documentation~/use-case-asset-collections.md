# Use case: View the collections of an asset and manage its associations to these collections

You can use the Unity Cloud Assets package to:

* List the collections an asset belongs to.
* Add and remove an asset from a collection.

The SDK supports different workflows for users with different roles.

| Asset Manager Project role                                                                           | List an asset's collections | Add/remove assets in collections |
|:-----------------------------------------------------------------------------------------------------|:----------------------------|:---------------------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes                         | no                               |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes                         | no                               |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes                         | yes                              |
| [`Asset Management Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)        | yes                         | yes                              |

## Before you start

Before you start, you need some assets in the cloud. There are several ways to do so:

* You can create assets through the [Get started with Assets](get-started-management.md).
* You can upload assets from existing Unity assets, see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
* You can create assets through the dashboard, see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List the collections of an asset

By default, an asset's collections are not included when you get an asset. To fetch the collections of an asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_Behaviour_RefreshCollections)]

The code snippet populates a list of the collections of the selected asset.

### Remove an asset from a collection

To remove an asset from a collection:

1. Open the `AssetManagementBehaviour` script you created. 
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_Behaviour_RemoveFromCollection)]

The code snippet does the following:

* Removes the selected asset from the specified collection.
* Updates the list of collections of the selected asset.
v* Prints a message to the console on success.

### Add the UI for viewing and creating collections

To create UI for displaying aggregation information, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseAssetCollectionExampleUI`.
4. Open the `UseCaseAggregationExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCollectionExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

```cs
   m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
   m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
   m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
   m_UI.Add(new UseCaseAssetCollectionExampleUI(m_Behaviour));
```

The code snippet displays the following UI elements:

* A list of the selected asset's collections.
* A UI button beside each collection to remove the selected asset from it.
