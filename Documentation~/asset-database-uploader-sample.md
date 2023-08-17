# Sample: Create and upload Unity Editor assets

You can use the Asset Database Uploader sample in the Unity Editor to create and upload assets from your Unity project into your Unity Cloud project. After you create and upload an asset, it becomes available in the Unity Cloud Asset Manager dashboard.

The sample uses the management endpoints and requires one of the following:

* The role of Manager or above in the Unity Cloud Organization
* The role of Asset Manager Contributor or above in the Unity Cloud Project

## Prerequisites

Before you use the Asset Database Uploader sample, you must have the following:

* The [Assets](installation.md) and [Identity](https://docs.unity3d.com/Packages/com.unity.cloud.identity@0.16/manual/installation.html) packages installed
* A valid [Unity ID Account](https://dashboard.unity3d.com/) and [access to the asset manager service](https://docs.unity3d.com/docs-asset-manager/manual/get-started.html)

> **Note**: While the Assets package doesn't depend on the Identity service, it is used in the sample to control the authentication flow.

## Installation

To install the sample, follow these steps:

1. In your Unity project window, go to **Package Manager** > **Unity Cloud Assets**.

2. Expand the **Samples** section and select **Import** next to the Asset Database Uploader sample.
   
   ![Screenshot of the samples import section of the package manager window](images/import-uploader-sample.png)

   After the import process is complete, you can view your imported assets in the `Assets/Samples/Unity Cloud Assets` folder.

   ![Screenshot of the imported sample](images/tac-sample-uploader-scene.png)

## Run the sample

To run the sample, follow these steps:

1. Go to `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Database Uploader/Scenes/AssetDatabaseUploaderSample.unity` and open the scene. If this is your first time launching the sample, sign in with your Unity Gaming Services account.

   > **Note:** For more information about creating a project, see the [Asset Manager documentation](https://docs.unity3d.com/docs-asset-manager/manual/index.html).

2. Select the `AssetDatabaseUploader` game object in the Hierarchy window.

3. In the Inspector window, click **Fetch Organizations and Projects**.

   ![Screenshot of the Fetch Organizations and Projects button](images/uploader-fetch-organizations-projects.png)

4. Select the organization where you want to upload your assets.

   ![Screenshot of the organization drop-down menu](images/uploader-select-organization.png)

5. Select the project where you want to upload your assets.

   ![Screenshot of the project drop-down menu](images/uploader-select-project.png)

6. Enter the path to the assets you want to upload.

   ![Screenshot of the Local Assets path textfield](images/uploader-set-local-assets-path.png)

7. If you want to check for assets that already exist in your Unity Cloud project, click **Search Assets**.

   ![Screenshot of the Search Assets button](images/uploader-search-assets.png)

8. To upload your assets, click **Create and Upload Assets** button.
   
   ![Screenshot of the Upload Assets button](images/uploader-upload-assets.png)

> **Note**: The script doesn't identify assets that are made up of multiple files, so each file is processed as a new asset. All assets are uploaded in Draft status.

### Timeout settings

The sample provides two timeout settings: one for the main queries (like Fetch and Search) and one for the upload process.

To change the timeout for the main queries, select the `AssetDatabaseUploader` game object in the Hierarchy window. In the Inspector window, change the `Main Queries Time Out` value.

   ![Screenshot of the main queries timeout textfield](images/uploader-set-main-queries-timeout.png)

To change the upload timeout, select the `AssetDatabaseUploader` game object in the Hierarchy window. In the Inspector window, change the `Upload Time Out` value.

   ![Screenshot of the upload query timeout textfield](images/uploader-set-upload-timeout.png)

## Main components

This section describes the scripts that make up the main components of the Asset Database Uploader sample.

### AssetsEditor services script

The `AssetsEditorServices` class initializes and disposes of dependencies required by `AssetManager` and `AssetFileManager`. You can use this class to manage the Unity Cloud services and dependencies you use in your scripts.

To open the AssetsEditor services script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/AssetsEditorServices.cs` file.

### Organization and Project selector script

The `OrgAndProjectSelector` script shows you how to do the following:

* Retrieve organizations and projects from the Asset Manager service
* Select an organization and project from a list of organizations and projects

To open the OrgAndProjectSelector script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/OrgAndProjectSelector.cs` file.

### Assets uploader script

The `AssetsUploader` script shows you how to do the following:

* Create an asset in a Unity Cloud project
* Create an asset file and attach it to the created asset
* Upload asset file contents

To open the AssetsUploader script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/AssetsUploader.cs` file.

The `AssetsUploader` class has one required component called `OrgAndProjectSelector`.

### Asset Database Uploader sample script

The `AssetDatabaseUploaderSample` shows you how to do the following:

* Integrate everything together the `AssetsEditorServices`, the `OrgAndProjectSelector` and the `AssetsUploader` scripts
* Use the `AssetsEditorServices` class to get your authentication token
* Use the `AssetsEditorServices` class to initialize the `OrganizationProvider`, `ProjectProvider`, `AssetManager`, `AssetFileManager`
* Initialize the `OrgAndProjectSelector` script
* Initialize the `AssetsUploader` script

To open the Asset Database Uploader sample script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/AssetDatabaseUploaderSample.cs` file.

### Inspector UI scripts

The sample includes a set of Editor UI scripts. To open the scripts, go to `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/Editor`.

### Limitations

The sample has the following limitations:

* If an asset already exists in your Unity Cloud project, the sample skips over it. It does not update the asset or upload new files.
* Currently, the API endpoints don't support getting the upload URL of an asset if the file and its contents were not created and uploaded at the same time as the asset.
* The script creates one asset per uploaded file, but you can add rules of your own to combine assets based on a naming convention or other parameters of your choice. For example, you could combine all files starting with the same letters before the first underscore. This rule would create two assets from the following:
  
   ```
   Marble014_8K_Roughness.png
   Marble014_8K_NormalGL.png
   Marble014_8K_NormalDX.png
   Marble014_8K_Displacement.png
   Marble014_8K_Color.png
   Marble014_PREVIEW.jpg
   Marble014_8K-PNG.usdc
   Rock030_PREVIEW.jpg
   Rock030_8K_Roughness.png
   Rock030_8K_NormalGL.png
   Rock030_8K_NormalDX.png
   Rock030_8K_Displacement.png
   Rock030_8K_Color.png
   Rock030_8K_AmbientOcclusion.png
   ```

## Go further with your sample

This section describes other things you can do with the Asset Database Uploader sample.

### Step-by-step mode

The sample provide a step-by-step mode to help you understand how the script works. To enable this mode, do the following:

1. Select the `AssetDatabaseUploader` game object in the Hierarchy window.
2. In the Inspector window, enable the `Step By Step` checkbox.

   ![Screenshot of the step-by-step mode section](images/uploader-step-by-step-mode.png)

With this mode enabled, the Create and Upload Assets button is replaced by **Create Assets**, **Create Asset Files**, and **Upload Created Assets**. 

## Troubleshoot

This section describes issues you might have while using the Asset Database Uploader sample.

### Missing dependency error

If you get a missing dependency error about a specific package, ensure you have installed all the packages listed in the [Prerequisites](#prerequisites).

### Can't create and upload assets

If you can't create or upload any assets, your organization may not have the Asset Management feature enabled. You'll need to [request access to the beta](https://docs.unity3d.com/docs-asset-manager/manual/request-access.html).

If your organization has Asset Management enabled, you may not have permission to create and upload assets. Contact your Unity Organization/Project Manager to request the right permissions.