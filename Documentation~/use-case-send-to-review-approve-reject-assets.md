# Use case: Manage assets review

You can use the Unity Cloud Assets package to send assets to review and approve or reject assets in review.

| Asset Manager Project role                                                                           | Send to review | Approve/reject |
|:-----------------------------------------------------------------------------------------------------|:---------------|----------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no             | no             |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | no             | no             |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes            | yes            |
| [`Asset Management Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)        | yes            | yes            |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Send an asset to review

To send an asset to review, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_SendAssetToReview)]

The code snippet sends an asset in the cloud to review.

### Approve an in-review asset

To approve an in-review asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_ApproveInReviewAsset)]

The code snippet approves an in-review asset.

### Reject an in-review asset

To reject an in-review asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_Behaviour_RejectInReviewAsset)]

The code snippet rejects an in-review asset.

### Add the UI for updating the status of assets

To create UI for updating the status of assets, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseSendToReviewApproveRejectAssetExampleUI`.
4. Open the `UseCaseSendToReviewApproveRejectAssetExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSendToReviewApproveRejectAssetExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

```cs
   m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
   m_UI.Add(new ProjectSelectionExampleUI(m_Behaviour));
   m_UI.Add(new AssetSelectionExampleUI(m_Behaviour));
   m_UI.Add(new UseCaseSendToReviewApproveRejectAssetExampleUI(m_Behaviour));
```

The code snippet provides UI buttons to trigger the send to review or approve in-review or reject in-review an asset.
