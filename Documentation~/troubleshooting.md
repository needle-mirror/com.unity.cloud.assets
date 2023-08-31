# Troubleshooting

This section describes issues you might have while using the Unity Asset Manager SDK package.

## Sample issues

### General sample issues

**I have dependency issues with the samples.**

If you have dependency issues with the samples, refer to the **Before you start** section of the impacted sample:

* [Asset Database Uploader sample](asset-database-uploader-sample.md#before-you-start)
* [Asset Discovery sample](asset-discovery-sample.md#before-you-start)
<!-- * [Asset Management sample](asset-management-sample.md#before-you-start) -->

**The automatic browser redirection doesn't work**

If you run the sample in the Unity Editor, you should see the following page after you successfully login through your browser.

![Login Successful](images/login-redirect.png)

If you aren't automatically redirected to the Editor and nothing happens when you select **Launch Application**, return to the Editor. This should continue the authentication process.

**I can't see my assets**

If you can't see any assets, your organization might not have the asset management feature flag enabled. You'll need to [request access to the beta](https://docs.unity3d.com/docs-asset-manager/manual/request-access.html).

### Asset Database Uploader sample

**I can't create and upload my assets**

If you can't create and upload any assets:

* Your organization might not have the asset management feature flag enabled. You'll need to [request access to the beta](https://docs.unity3d.com/docs-asset-manager/manual/request-access.html).
* You might not have the right permissions to create and upload assets. You'll need to contact your Unity Organization/Project Manager to get the right permissions.
