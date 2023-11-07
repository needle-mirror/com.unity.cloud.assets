# Get started with Asset SDK

Asset Manager is a Unity Cloud service that allows you to manage your assets in the cloud. You can use Assets SDK to:

* Create an Asset Project
* List your Assets from an Organization
* Get the information of an Asset Project
* Create, read, update, and delete:
  * asset collections
  * assets
  * dataset
* Get an asset
* Search for assets
* Aggregate assets based on a set of criteria
* Create, read, update, delete, upload, and download files
* Add and remove assets from collections
* Link and unlink assets from Projects

This section explains how to set up a basic scene and script to initialize and use the Unity Assets package with Asset Manager.
It performs a basic search for all assets of the selected project and displays the results in a simple GUI.

## Requirements

To use Assets SDK, you must have a minimum role of [**Asset Manager Viewer**](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in the Unity Cloud Project you belong to.

## Integrate the package in a Unity project

To integrate the Unity Cloud Assets package in a Unity project, you must do the following:

* Set up a Unity scene
* Create the `PlatformServices`

### Set up a Unity scene

To set up a Unity scene, perform the following steps:

1. In your Unity project window, navigate to **Assets** > **Scenes**.
2. Select and hold the `Assets/Scenes` folder and navigate to **Create** > **Scene**.
3. Name the new scene `AssetManagementExample`.

### Create an AssetManager

To create a `MonoBehaviour`, perform the following steps:

1. In your Unity project window, go to **Assets** > **Scripts**. Create an `Assets/Scripts` folder if the folder doesn't already exist.
2. Select and hold the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `AssetManagementUI`.
3. In the `AssetManagementExample` scene you created earlier, select and hold the hierarchy and select **Create Empty**.
4. Name your new object `AssetManager`.
5. Select the `AssetManager` object and add the `AssetManagementUI` script you created earlier.

### Create the PlatformServices

To instantiate the necessary components, follow these steps:

1. Implement the platform services pattern. See the **Best practices: dependency injection** page of the [Identity package documentation](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest) for more information.
2. Update the `PlatformServices` class in your `PlatformServices.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Management/PlatformServices.cs#PlatformServices)]

What this script accomplishes:

* Initializes an `ICompositeAuthenticator` for logging in and verifying your identity when accessing the HTTP services.
* Initializes an `IOrganizationRepository` to fetch the organizations you belong to.
* Initializes an `IAssetRepository` to interface with the Asset Manager service.

To initialize the `PlatformServices` in your scene, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `PlatformServicesInitialization`.
3. In the `AssetManagementExample` scene you created earlier, select and hold the hierarchy and select **Create Empty**.
4. Name your new object `PlatformServices`.
5. Select the `PlatformServices` object and add the `PlatformServicesInitialization` script you created earlier.
6. Update the `PlatformServicesInitialization` class in your `PlatformServicesInitialization.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Management/PlatformServicesInitialization.cs#PlatformServices_Initialization)]

What this script accomplishes:

* Triggers the creation of the services available in the `PlatformServices`.
* Initializes the `ICompositeAuthenticator`.

To clean up the `PlatformServices` in your scene, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `PlatformServicesShutdown`.
3. Select the `PlatformServices` object you created earlier and add the `PlatformServicesShutdown` script you created earlier.
4. Update the `PlatformServicesShutdown` class in your `PlatformServicesShutdown.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Management/PlatformServicesShutdown.cs#PlatformServices_Shutdown)]

This script cleans up of the services when the scene is closed.

### Create the behaviour for managing assets

To create the behaviour for asset management, perform the following steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `AssetManagementBehaviour`.
3. Open the `AssetManagementBehaviour` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/GetStartedManagementExample.cs#Example)]

The script does the following:

* Provides the functions to list and select Organizations.
* Provides the functions to list and select Projects.
* Performs a basic search to list the assets of the selected Project.
* Provides the functions to create, read, update, delete assets.

### Create the UI for navigating assets

To create a simple UI for navigating assets, do the following:

Open the `AssetManagementUI` script you created earlier and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/GetStartedManagementExampleUI.cs#Example)]

The script does the following:

* Registers to the `ICompositeAuthenticator` to track login changes.
* Creates an instance of an `AssetManagementBehaviour`.
* Creates a simple UI flow for selecting Organizations, Projects and do CRUD operations on assets.

## Going further

For a more information about asset management, see the [Asset Management sample](asset-management-sample.md).

### Creating datasets

By default, each asset contain two datasets:

* `Sources`
* `Previews`

To create additional datasets, see the [Create datasets](use-case-create-datasets.md) use case for more information.

### Uploading files

See the [Upload files](use-case-create-files.md) use case for more information.

### Grouping assets in collections

Collections allow assets to be group together within a project. See the [Manage collections](use-case-manage-collections.md) use case for more information.
