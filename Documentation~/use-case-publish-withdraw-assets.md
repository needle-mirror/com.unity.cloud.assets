# Publish, Withdraw assets

You can use the Unity Cloud Assets package to manage the assets.

Asset management is available through the Asset Management pathway.
> Note: Asset management requires users have the role of `Asset Management Contributor` OR a minimum role of `Manager` in the organization.

## Prerequisites

Before you start, set up a Unity scene in with a organization and project browser.
See [Get started with Asset Management](get-started-management.md) for more information.

Next, you must have some assets in the cloud. There are several ways to do so:

* Assets can be created through the [Get started with Asset Management](get-started-management.md).
* Assets can be uploaded from existing Unity assets; see the [Asset Uploader sample](./asset-database-uploader-sample.md).
* You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## Methodology

### Publish an asset

To publish an asset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_Behaviour_PublishAsset)]

The script does the following:

* Set the asset in the cloud as published.

### Withdraw a published asset

To withdraw a published asset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_Behaviour_WithdrawAsset)]

The script does the following:

* Withdraw the published asset.

### Add the UI for interacting with assets

To add UI for the example, open the `AssetManagementUI` script you created and replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCasePublishWithdrawAssetExample.cs#Example_UI)]

The script does the following:

* Provides UI buttons to trigger the publish or the withdraw of a new asset.
