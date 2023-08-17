# Aggregate Assets

You can use the Unity Cloud Assets package to retrieve the assets in a project that meet a set of search criteria.

There are two workflows when searching for assets which can be controlled from the `AssetServiceConfiguration` class.
Setting the `IsDiscovery` to `true` in the `AssetServiceConfiguration` will fetch and search among published assets only.
While setting the value to `false` will fetch and search among all assets regardless of status.

The flag you choose will depend on the project roles of your users.
> Note: The Asset Discovery pathway requires users to have the minimum role of `Asset Management Viewer`, while the Asset Management requires higher permissions with a minimum role of `Asset Management Contributor`.

## Prerequisites

[!INCLUDE [prerequisites](../includes/prerequisites.md)]

## Methodology

### Add aggregation behaviours

To implement aggregation, open the `AssetDiscoveryBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Discovery/UseCaseAggregationExample.cs#Example_Behaviour)]

> Note: This code can be adapted to the Asset Management pathway. Add the above code to the `AssetManagementBehaviour` script you created and replace references to `PlatformServices.AssetProvider` to `PlatformServices.AssetManager`.

The script does the following:

* Provides several functions which return the aggregation of assets for a given field.

### Add the UI for triggering and displaying the aggregation

To add a UI to the example, open the `AssetDiscoveryUI` script you created and replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Discovery/UseCaseAggregationExample.cs#Example_UI)]

> **Note:** To adapt the code to the Asset Management pathway, add the code above to the `AssetManagementUI` script you created.

The script does the following:

* Provides UI buttons to trigger the aggregation functions.
* Displays the results of the aggregation functions.
