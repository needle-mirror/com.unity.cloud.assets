# Sample: Create and Upload Unity Editor assets

You can use the Asset Database Uploader sample in the Unity Editor to create and upload assets from your Unity project into your Unity Cloud project. 
By creating them and uploading them, your assets will be available in the Unity Cloud Asset Manager dashboard.

The sample use the management endpoints that requires a minimum role of **Manager** in the Unity Cloud Organization you belong to OR a minimum role of **Asset Manager Contributor** in the Unity Cloud Project you belong to.

## Prerequisites

Before you use the Asset Database Uploader sample, you must have the following:

* An installed [Assets](installation.md), and [Identity](https://docs.unity3d.com/Packages/com.unity.cloud.identity@0.16/manual/installation.html) packages
* A valid [Unity ID Account](https://dashboard.unity3d.com/) and [access to the asset manager service](https://docs.unity3d.com/docs-asset-manager/manual/get-started.html)

> **Note**: While the Assets package doesn't depend on the Identity service, It is used in the sample to control the authentication flow.

## Installation

To install the sample, follow these steps:

1. Inside your Unity project window, go to **Package Manager** > **Unity Cloud Assets**.
2. Expand the **Samples** section and select **Import** next to the Asset Database Uploader sample.
   </br>
   ![Screenshot of the samples import section of the package manager window](images/import-uploader-sample.png)

After the import process is complete, you can view your imported assets under the `Assets/Samples/Unity Cloud Assets` folder.
</br>
![Screenshot of the imported sample](images/tac-sample-uploader-scene.png)

## Run the sample

To run the sample, follow these steps:

1. Go to `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Database Uploader/Scenes/AssetDatabaseUploaderSample.unity` and open the scene. If this is your first time launching the sample, make sure to sign in with your Unity Gaming Services account. For more information on creating a Unity project, see the [Asset Manager documentation](https://docs.unity3d.com/docs-asset-manager/manual/index.html).
2. Select the `AssetDatabaseUploader` game object in the Hierarchy window.
3. In the inspector window, click on the `Fetch Organizations and Projects` button.
   </br>
   ![Screenshot of the fetch organizations and projects button](images/uploader-fetch-organizations-projects.png)
4. Select the organization where you want to upload your assets to.
   </br>
   ![Screenshot of the organization dropdown selection](images/uploader-select-organization.png)
5. Select the project where you want to upload your assets to.
   </br>
   ![Screenshot of the project dropdown selection](images/uploader-select-project.png)
6. Set the folder path that contains your assets to upload.
   </br>
   ![Screenshot of the local assets path textfield](images/uploader-set-local-assets-path.png)
7. If you want to check which asset is already known in your Unity Cloud project, click on the `Search Assets` button.
   </br>
   ![Screenshot of the search assets button](images/uploader-search-assets.png)
8. To upload your assets, click on the `Create and Upload Assets` button.
   </br>
   ![Screenshot of the upload assets button](images/uploader-upload-assets.png)

> **Note**: The script is basic and doesn't handle asset that are defined by multiple files. So each file is seen as a new asset.
> Also, all the uploaded assets are set with draft status. The script doesn't provide a way to change the status of an asset.

### Time out settings

The sample provide two time out settings. One for the main queries like Fetch and Search for example. And one for the upload process.

1. To change the main queries time out, select the `AssetDatabaseUploader` game object in the Hierarchy window.
2. In the inspector window, change the `Main Queries Time Out` value.
   </br>
   ![Screenshot of the main queries timeout textfield](images/uploader-set-main-queries-timeout.png)

3. To change the upload time out, select the `AssetDatabaseUploader` game object in the Hierarchy window.
4. In the inspector window, change the `Upload Time Out` value.
   </br>
   ![Screenshot of the upload query timeout textfield](images/uploader-set-upload-timeout.png)

## Main components

This section describes the scripts that make up the main components of the Asset Database Uploader sample.

### AssetsEditor services script

The `AssetsEditorServices` class initializes and disposes of dependencies required by the `AssetManager` and `AssetFileManager`. You can use this class to manage the Unity Cloud services and dependencies you use in your scripts.

To open the AssetsEditor services script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/AssetsEditorServices.cs` file.

### Organization and Project selector script

The `OrgAndProjectSelector` script shows you how to do the following:

* Retrieve organizations and projects from the Asset Manager service.
* Select an organization and project from a list of organizations and projects.

To open the OrgAndProjectSelector script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/OrgAndProjectSelector.cs` file.

### Assets uploader script

The `AssetsUploader` script shows you how to do the following:

* Create an asset into a Unity Cloud project.
* Create an asset file and attach it to the created asset.
* Upload an asset file content.

To open the AssetsUploader script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/AssetsUploader.cs` file.

The `AssetsUploader` class has one required component called `OrgAndProjectSelector`.

### Asset Database Uploader sample script

The `AssetDatabaseUploaderSample` shows you how to do the following:

* Integrate everything together the `AssetsEditorServices`, the `OrgAndProjectSelector` and the `AssetsUploader` scripts.
* Use the `AssetsEditorServices` class to get your authentication token.
* Use the `AssetsEditorServices` class to initialize the `OrganizationProvider`, `ProjectProvider`, `AssetManager`, `AssetFileManager`.
* Initialize the `OrgAndProjectSelector` script.
* Initialize the `AssetsUploader` script.

To open the Asset Database Uploader sample script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/AssetDatabaseUploaderSample.cs` file.

### Inspector UI scripts

The sample includes a set of Editor UI scripts. To open UI scripts, go to `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDatabaseUploader/Scripts/Editor`.

### Limitations

The sample has the following limitations:

* If your asset already exists in your Unity Cloud project the sample will not update your asset nor upload the file content. It will just skip it. 
For now, the API endpoints don't support to get the upload url of an asset if the file and the content has not been created and uploaded as the same time than the Asset's creation. 

* Allows you to create 1 asset per 'file', you can add rules of your own to combine asset based on 'naming convention, or anything else'.
An example would be combine all files that have the same letters before the first _ .
The example of files below would create 2 assets.

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

## Go further with your sample

This section describes other actions you can perform with the Asset Database Uploader sample.

### Step by step mode

The sample provide a step by step mode to help you understand how the script works. If you want to follow each step instead of doing everything in one click, you can enable the step by step mode.

1. To enable it, select the `AssetDatabaseUploader` game object in the Hierarchy window.
2. In the inspector window, check the `Step By Step` checkbox.
   </br>
   ![Screenshot of the step by step mode section](images/uploader-step-by-step-mode.png)
3. Then you can do like the `Create and Upload Assets` button by clicking on each action button one by one following this order :
   `Create Assets` (1), `Create Asset Files` (2), `Upload Created Assets` (3).

## Troubleshoot

This section describes issues you might have while using the Asset Database Uploader sample.

### Missing dependency

If you get a missing dependency error about a specific package, ensure you have installed all the packages listed in the [Prerequisites](#prerequisites).

### I can't create and upload my assets

If you can't create and upload any assets, it might be that your organization doesn't have the asset management feature flag enabled. You'll need to [request access to the beta](https://docs.unity3d.com/docs-asset-manager/manual/request-access.html).
Or you don't have the right permissions to create and upload assets. You'll need to contact your Unity Organization/Project Manager to get the right permissions. 

