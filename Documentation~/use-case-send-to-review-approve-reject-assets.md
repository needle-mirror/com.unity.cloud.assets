# Send to Review, Approve, Reject assets

You can use the Unity Cloud Assets package to manage the assets.

Asset management is available through the Asset Management pathway.
> Note: Asset management requires users have the role of `Asset Management Contributor` OR a minimum role of `Manager` in the organization.

## Prerequisites

Before you start, set up a Unity scene in with a organization and project browser.
See [Get started with Asset Management](get-started-management.md) for more information.

Next, you must have some assets in the cloud.
You can either create assets through the dashboard; see the [Managing assets on the dashboard]() documentation.

## Methodology

### Send an asset to review

To send an asset to review, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_SendAssetToReview)]

The script does the following:

* Send an asset in the cloud to review.

### Approve an in-review asset

To approve an in-review asset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_ApproveInReviewAsset)]

The script does the following:

* Approve an in-review asset.

### Reject an in-review asset

To reject an in-review asset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_RejectInReviewAsset)]

The script does the following:

* Reject an in-review asset.

### Add the UI for interacting with assets

To add UI for the example, open the `AssetManagementUI` script you created and replace the `AdditionalActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_UI)]

The script does the following:

* Provides UI buttons to trigger the send to review or approve in-review or reject in-review an asset.
