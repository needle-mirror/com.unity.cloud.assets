# Sample: Discover published assets

You can use the Asset Discovery sample to search and download published assets from your organizations and projects.

The sample only displays published assets, using the discovery endpoints that require only user-level permissions.

## Before you start

Before you use the Asset Discovery sample, you must have the following:

* An installed [Assets](installation.md) package
* An installed [Identity](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest) package
* A valid [Unity ID Account](https://dashboard.unity3d.com/)
* An access to [Asset Manager service](https://docs.unity3d.com/docs-asset-manager/manual/get-started.html)
* At least 1 published asset in an Asset Management project (see [Asset Manager documentation](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html))
* A Unity project with the Asset Manager service enabled (see [Asset Manager documentation](https://docs.unity3d.com/docs-asset-manager/manual/modify-project.html))

> **Note**: While the Assets package doesn't depend on the Identity service, it is used in the sample to control the authentication flow.

## Install the sample

To install the sample, follow these steps:

1. In your Unity project window, go to **Package Manager** > **Unity Cloud Assets**.
   
2. Expand the **Samples** section and select **Import** next to the Asset Discovery sample.

   ![Screenshot of the samples import section of the package manager window](images/import-discovery-sample.png)

   After the import process is complete, you can view your imported assets in the `Assets/Samples/Unity Cloud Assets` folder.

   ![Screenshot of the imported sample](images/tac-sample-discovery-scene.png)

## Run the sample

To run the sample, follow these steps:

1. Go to `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Discovery/Scenes/AssetDiscoverySample.unity` and run the scene. If this is your first time launching the sample, make sure to sign in with your Unity Gaming Services account.
2. Select an Organization. The list of projects from that organization appears on the left column.
   </br>
   ![Screenshot of the organization selection](images/organizations.png)
3. Select a project. The list of published assets from that project appears on the right column.
   </br>
   ![Screenshot of the project list](images/project-list.png)
   </br>
   ![Screenshot of the login window](images/assets-list.png)

### Search for specific assets

To search for specific assets by tag or name in this sample, follow these steps:

1. Select a project.
2. In the search bar, type the keywords by which you want to search your assets and select **Search**.
   </br>
   ![Screenshot of the created topic](images/search.png)

> **Note**: While using the SDK, you will be able to build different and more complex queries. For more information on asset search, see the [Search Assets manual](use-case-search-assets.md).

#### Search prompts

As you type in the search bar, the sample will display a list of keyword suggestions based on your search. These keywords are aggregated from the tags and names of your assets.
To select a keyword, click on it. The search bar will add it as a parameter and the sample will refresh the asset list to match the new search.

### See asset details and download files

To see your asset information in this sample, follow these steps:

1. Select one of the assets in the grid. The asset details view appears.
    </br>
    ![Screenshot of the created topic](images/asset-detail.png)

2. To download your asset files, select **Download**.

To edit the download's filepath, update the `path` variable in `AssetInformationPanelController.OnAssetDownloadButtonClicked()`.
```C#
async void OnAssetDownloadButtonClicked()
{
    UpdateDownloadButton(false);

    var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
```

> **Note**: To download assets, you will need the right permissions in your project to **consume** assets.

## Main components

This section describes the scripts that make up the main components of the Asset Discovery sample.

### Platform services script

The `PlatformServices` class initializes and disposes of dependencies required by the `AssetProvider`. You can use this class to manage the Unity Cloud services and dependencies you use in your application.

To open the platform services script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Services/PlatformServices.cs` file.

The `PlatformServices` class has two accompanying classes called `PlatformServicesInitialization` and `PlatformServicesShutdown` that call the initialization and shutdown methods through Unity's standard `Monobehaviour` methods `Awake()`, `Start()` and `OnDestroy()`.

### User Controller script

The `UserController` class lets you sign in and provides the Asset Discovery sample with your Unity Gaming Services ID. For more information on authentication, see the **Get user information** page of the [Identity package documentation](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest).

To open the UserController script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers/UserController.cs` file.

### Asset Discovery sample script

The `AssetDiscoverySample` shows you how to do the following:

* Integrate the login flow with the `UserController` class
* Retrieve organizations and projects from the Asset Manager service
* Retrieve published assets from the Asset Manager service
* Search for assets by tag or name

To open the Asset Discovery sample script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetDiscovery/Scripts/AssetDiscoverySample.cs` file.

### Shared UI scripts

The `UI` sample includes a set of UI scripts and prefabs used by Unity Asset Manager SDK samples. To open shared UI scripts, go to `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers`.

## Go further with your sample

This section describes other actions you can perform with the Asset Discovery sample.

### Search published, unpublished, and draft assets

If you want to search among all assets, not only the published assets, you can replace the Asset provider implementation to Asset Management instead of discovery by switching them in the `AssetsPlatformServices`.
    </br>
    ![Switch asset providers](images/gofurther-managerprovider.png)
    </br>
> **Note**: The Asset Management provider type will use different endpoints that require a 'manager' permission. For more information on permissions, see the [Asset Manager documentation](https://docs.unity3d.com/docs-asset-manager/manual/modify-project.html).

## Troubleshooting

Refer to the [troubleshooting](troubleshooting.md#sample-issues) section for help with sample issues.
