# Asset Management Sample

The sample available as part of the Unity Cloud Platform Assets SDK demonstrates how to create, upload and update assets.
A typical example of audience for this guide is the developers who want to integrate asset management features in their app. 

## Prerequisites

To use the Asset Management sample, you need the following:

* An installed [Assets](installation.md), and [Identity](https://docs.unity3d.com/Packages/com.unity.cloud.identity@0.16/manual/installation.html) packages
* A valid [Unity ID Account](https://dashboard.unity3d.com/) and [access to the asset manager service](https://docs.unity3d.com/docs-asset-manager/manual/get-started.html)

## Installation

To install the sample, follow these steps:

1. In your Unity project, go to **Window > Package Manager > Assets**.
2. Expand **Samples** and select **Import** beside the Asset Management sample.

After the import process completes, you can view the imported assets under the `Unity Cloud Assets/Samples/Samples/Asset Management` folder.

## Run the sample

To run the sample, follow these steps:

1. In your Unity project, go to **File** > **Open Scene**.
2. Go to `Packages/Unity Cloud Assets/Samples/Asset DiscoverySample.Unity` and run the scene.
3. In the Game view, select **Login** if you are logged out.
    > **Note**: If you've previously logged in, the sample automatically logs you in so you can proceed to step 6.
4. Log into the browser window that launches with your Unity ID account.
5. Return to the sample scene to confirm that you are logged in. The organization field lists the organization associated with your account.
6. Select an organization from the **Organization** field.
7. Select a project from the **Project** field.

### Create an asset

### Update/edit an asset

## Main components

TODO

## Troubleshooting

### Missing dependency

If you get a missing dependency error about a specific package, ensure you have installed all the packages listed in the [Prerequisites](#prerequisites).

### The automatic browser redirection doesn't work

If you run the sample in the Unity Editor, you should see the following page after you successfully login through your browser.

![Login Successful](images/login-redirect.png)

If you aren't automatically redirected to the Editor and nothing happens when you select **Launch Application**, return to the Editor. This should continue the authentication process.

### I can't see my assets

If you can't see any assets, if might be that your organization doesn't have the asset management feature flag enabled. You'll need to [request access to the beta](https://docs.unity3d.com/docs-asset-manager/manual/request-access.html).
