# Use case: Group and count assets

Use the Unity Cloud Assets package to count the number of assets in a project that meet a set of search criteria.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | Group and count search |
|:-----------------------------------------------------------------------------------------------------|:-----------------------|
| [`Asset Manager Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles)      | yes                    |
| [`Asset Manager Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles)    | yes                    |
| [`Asset Manager Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) | yes                    |
| [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles)            | yes                    |

## Before you start

Before you start, do the following:

1. Verify you have the required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

   >[!NOTE]
   >Asset Manager roles define the permissions that you have for a single Asset Manager project. Depending on your work, permissions may vary across projects.

2. Set up a Unity scene in the Unity Editor with an Organization and Project browser. Read more about [setting up a Unity scene](get-started-management.md#Set-up-a-Unity-scene).
3. Create assets in Unity Cloud any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [a single asset](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple assets](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) through the dashboard.

## How do I...?

### Add aggregation behaviors

To implement aggregation, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAggregationExample.cs#Example_Behaviour)]

The code snippet provides a function that aggregates assets based on a specific field.

### Add a UI for displaying aggregation information

To create a UI for displaying aggregation information, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseAggregationExampleUI`.
5. Open the `UseCaseAggregationExampleUI` script that you created in the previous step and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAggregationExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAggregationExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseAggregateAssets)]

The code snippet does the following:
* Provides UI buttons that trigger the aggregation function with different criteria.
* Displays the results of the aggregation.

The UI can aggregate assets by the following criteria:

* Name
* Version
* Type
* Status
* Tags and system tags
* Preview file
* Created by
* Updated by
* Collections

Results of the aggregation are displayed below the following buttons:

* `Total` is the number of assets that match the search criteria.
* `Unique` is the number of unique values for the aggregation field.
* `Values` is a list of key-value pairs, where the key is a unique value of the aggregation field, and the value is the number of assets that match the search criteria.
