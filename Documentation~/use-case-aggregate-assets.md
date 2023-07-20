# Aggregate Assets

You can use the Unity Cloud Assets package to retrieve the number of assets in a project that meet a set of search criteria.

There are two workflows when searching for assets.
In Asset Discovery, the `CloudAssetDiscovery` implementation of the `IAssetProvider` interface will fetch and search among published assets only, while in AssetManagement, the `CloudAssetManager` implementation of the `IAssetProvider` interface will fetch and search among all assets regardless of status.

The implementation you choose will depend on the project roles of your users.
> Note: The Asset Discovery pathway requires users have the minimum role of `Asset Management Viewer`, while the Asset Management requires higher permissions with a minimum role of `Asset Management Contributor`.

## Prerequisites

Before you start, set up a Unity scene in with an organization and project browser.
See either [Get started with Asset Discovery](get-started-discovery.md) or [Get started with Asset Management](get-started-management.md) for more information.

Next, you must have some assets in the cloud. There are several ways to do so:

* Assets can be created through the [Get started with Asset Management](get-started-management.md).
* Assets can be uploaded from existing Unity assets; see the [Asset Uploader sample](./asset-database-uploader-sample.md).
* You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## Methodology

### Add aggregation behaviours

To implement aggregation, open the `AssetDiscoveryBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Discovery/UseCaseAggregationExample.cs#Example_Behaviour)]

> Note: This code can be adapted to the Asset Management pathway. Add the above code to the `AssetManagementBehaviour` script you created and replace references to `PlatformServices.AssetProvider` to `PlatformServices.AssetManager`.

The script does the following:

* Provides several functions which return the aggregation of assets for a given field.

### Add the UI for triggering and displaying the aggregation

To add UI for the example, open the `AssetDiscoveryUI` script you created and replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Discovery/UseCaseAggregationExample.cs#Example_UI)]

> Note: To adapt the code to the Asset Management pathway. Add the above code to the `AssetManagementUI` script you created.

The script does the following:

* Provides UI buttons to trigger the aggregation functions.
* Displays the results of the aggregation functions.
