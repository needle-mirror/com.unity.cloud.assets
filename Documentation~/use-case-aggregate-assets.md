# Aggregate assets

You can use the Unity Asset Manager SDK package to retrieve the number of assets in a project that meet a set of search criteria.

The SDK supports different workflows for users with different roles.

| Asset Manager Project role |Supported workflows |Details|
| :-- | :-- | :--|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) (minimum)| Asset Discovery |  The `CloudAssetDiscovery` implementation of the `IAssetProvider` interface fetches and searches among published assets only.|
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) (minimum)| Asset Management| The `CloudAssetManager` implementation of the `IAssetProvider` interface fetches and searches among all assets regardless of status. |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an organization and project browser. See either [Get started with Asset Discovery](get-started-discovery.md) or [Get started with Asset Management](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Asset Management](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Add aggregation behaviours

To implement aggregation, open the `AssetDiscoveryBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Discovery/UseCaseAggregationExample.cs#Example_Behaviour)]

> **Note**: You can adapt the code to the Asset Management pathway. To do so, add the above code to the `AssetManagementBehaviour` script you created and replace references to `PlatformServices.AssetProvider` to `PlatformServices.AssetManager`.

The script provides several functions which return the aggregation of assets for a given field.

### Add the UI for triggering and displaying the aggregation

To add a UI to the example, open the `AssetDiscoveryUI` script you created and replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Discovery/UseCaseAggregationExample.cs#Example_UI)]

> **Note**: You can adapt the code to the Asset Management pathway. To do so, add the above code to the `AssetManagementUI` script you created.

The script provides UI buttons to trigger the aggregation functions and displays the results of the aggregation functions.
