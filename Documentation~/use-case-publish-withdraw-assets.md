# Publish and/or withdraw assets

You can use the Unity Asset Manager SDK package to manage the assets.

Asset management is available through the Asset Management pathway.

> **Note**: Asset management requires users have the role of [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) OR a minimum role of [`Manager`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in the organization.

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an organization and project browser. See either [Get started with Asset Discovery](get-started-discovery.md) or [Get started with Asset Management](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Asset Management](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Publish an asset

To publish an asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_Behaviour_PublishAsset)]

The script sets the asset in the cloud as published.

### Withdraw a published asset

To withdraw a published asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_Behaviour_WithdrawAsset)]

The script withdraws the published asset.

### Add the UI for interacting with assets

To add UI for the example:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_UI)]

The script provides UI buttons to trigger the publish or the withdraw of a new asset.
