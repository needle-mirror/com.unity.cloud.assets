# Use case: Publish and/or withdraw assets

You can use the Unity Cloud Assets package to publish assets so they are available to viewers, or withdraw assets so they are open for modification.

| Asset Manager Project role                                                                             | Publish/withdraw |
|:-------------------------------------------------------------------------------------------------------|:-----------------|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)      | no               |
| [`Asset Management Consumer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)    | no               |
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) | yes              |
| [`Asset Management Owner`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)       | yes              |

>[!NOTE]
>Asset management requires users have the role of [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) OR a minimum role of [`Manager`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in the Organization.

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Publish an asset

To publish an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_Behaviour_PublishAsset)]

The code snippet sets the asset in the cloud as published.

### Withdraw a published asset

To withdraw a published asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_Behaviour_WithdrawAsset)]

The code snippet withdraws the published asset.

### Add the UI for interacting with assets

To add UI for the example, follow these steps:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_UI)]

The code snippet provides UI buttons to trigger the publish or the withdraw of a new asset.
