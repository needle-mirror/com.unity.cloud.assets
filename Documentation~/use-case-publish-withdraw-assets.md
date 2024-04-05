# Use case: Publish and/or withdraw assets

You can use the Unity Cloud Assets package to publish assets so they are available to viewers, or withdraw assets so they are open for modification.

| Organization or Asset Manager Project role                                                           | Publish/withdraw |
|:-----------------------------------------------------------------------------------------------------|:-----------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no               |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | no               |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes              |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes              |

>[!NOTE]
>Asset management requires users have the role of [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) OR a minimum role of [`Manager`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) in the Organization.

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

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCasePublishWithdrawAssetExample.cs#Example_Behaviour_PublishAsset)]

The code snippet sets the asset in the cloud as published.

### Withdraw a published asset

To withdraw a published asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCasePublishWithdrawAssetExample.cs#Example_Behaviour_WithdrawAsset)]

The code snippet withdraws the published asset.

### Add the UI for updating the status of assets

To create UI for updating the status of assets, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCasePublishWithdrawAssetExampleUI`.
4. Open the `UseCasePublishWithdrawAssetExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCasePublishWithdrawAssetExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCasePublishWithdrawAssetExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCasePublishWithdrawAsset)

The code snippet provides UI buttons to publish or withdraw an asset.
