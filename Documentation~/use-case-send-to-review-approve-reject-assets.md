# Manage assets review

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

### Send an asset to review

To send an asset to review:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_SendAssetToReview)]

The script sends an asset in the cloud to review.

### Approve an in-review asset

To approve an in-review asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_ApproveInReviewAsset)]

The script approves an in-review asset.

### Reject an in-review asset

To reject an in-review asset:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_RejectInReviewAsset)]

The script rejects an in-review asset.

### Add the UI for interacting with assets

To add UI for the example:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AdditionalActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_UI)]

The script provides UI buttons to trigger the send to review or approve in-review or reject in-review an asset.
