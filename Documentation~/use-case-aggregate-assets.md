# Use case: Aggregate assets

You can use the Unity Cloud Assets package to retrieve the number of assets in a Project that meet a set of search criteria.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | Aggregation search |
|:-----------------------------------------------------------------------------------------------------|:-------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes                |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes                |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes                |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes                |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Add aggregation behaviours

To implement aggregation, open the `AssetDiscoveryBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAggregationExample.cs#Example_Behaviour)]

The code snippet provides a functions which returns the aggregation of assets for a given field.

### Add the UI for displaying aggregation information

To create UI for displaying aggregation information, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseAggregationExampleUI`.
4. Open the `UseCaseAggregationExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAggregationExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAggregationExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

```cs
   m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
   m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
   m_UI.Add(new UseCaseAggregationExampleUI(m_Behaviour));
```

The code snippet provides UI buttons to trigger the aggregation function with different criteria and displays the results of the aggregation.
The UI can aggregate assets by:

* Asset type
* Asset tags
* Asset status

An additional text field allows you to specify a custom aggregation field.

The results of the aggregation are displayed below the buttons.

* `Total` is the number of assets matching the search criteria.
* `Unique` is the number of unique values for the aggregation field.
* `Values` is a list of key-value pairs, where the key is a unique value of the aggregation field and the value is the number of assets that match.
