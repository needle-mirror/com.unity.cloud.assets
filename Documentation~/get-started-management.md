# Get started with Asset SDK

Asset Manager is a Unity Cloud service that allows you to manage your assets in the cloud. You can use Assets SDK to:

* Create and read an Asset Project.
* Create, read, and update:
  * assets
  * datasets
  * files
* Download files.
* Create, read, update, and delete collections.
* Search your Assets in an Asset Project or Organization based on a set of criteria.
* Group and count Assets based on a set of criteria.
* Link and unlink Assets from Projects.
* Link and unlink Assets from Collections.
* Start transformations on datasets.
* Create, read, update and delete the Field Definitions of an Organization.
* Add and remove the accepted values of Field Definitions of the `Selection` type.

This section explains how to set up a basic scene and script to initialize and use the Unity Assets package with Asset Manager.
It performs a basic search for all assets of the selected project and displays the results in a simple GUI.

Before you begin, make sure you meet the [prerequisites](prerequisites.md).

## Requirements

To use Assets SDK, you must have a minimum role of [**Asset Manager Viewer**](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) in the Unity Cloud Project you belong to.

## Integrate the package in a Unity project

To integrate the Unity Cloud Assets package in a Unity project, you must do the following:

* Set up a Unity scene
* Create the `PlatformServices`

### Set up a Unity scene

To set up a Unity scene, follow these steps:

1. In your Unity project window, navigate to **Assets** > **Scenes**.
2. Select and hold the `Assets/Scenes` folder and navigate to **Create** > **Scene**.
3. Name the new scene `AssetManagementExample`.

### Create an AssetManager

To create a `MonoBehaviour`, follow these steps:

1. In your Unity project window, go to **Assets** > **Scripts**. Create an `Assets/Scripts` folder if the folder doesn't already exist.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `AssetManagementUI`.
4. In the `AssetManagementExample` scene you created earlier, select and hold the hierarchy and select **Create Empty**.
5. Name your new object `AssetManager`.
6. Select the `AssetManager` object and add the `AssetManagementUI` script you created earlier.

### Create the PlatformServices

To instantiate the necessary components, follow these steps:

1. Implement the platform services pattern. See the **Best practices: dependency injection** page of the [Identity package documentation](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest) for more information.
2. Update the `PlatformServices` class in your `PlatformServices.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/PlatformServices.cs#PlatformServices)]

What this script accomplishes:

* Initializes an `ICompositeAuthenticator` for logging in and verifying your identity when accessing the HTTP services.
* Initializes an `IOrganizationRepository` to fetch the organizations you belong to.
* Initializes an `IAssetRepository` to interface with the Asset Manager service.

To initialize the `PlatformServices` in your scene, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `PlatformServicesInitialization`.
4. In the `AssetManagementExample` scene you created earlier, select and hold the hierarchy and select **Create Empty**.
5. Name your new object `PlatformServices`.
6. Select the `PlatformServices` object and add the `PlatformServicesInitialization` script you created earlier.
7. Update the `PlatformServicesInitialization` class in your `PlatformServicesInitialization.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/PlatformServicesInitialization.cs#PlatformServices_Initialization)]

What this script accomplishes:

* Triggers the creation of the services available in the `PlatformServices`.
* Initializes the `ICompositeAuthenticator`.

To clean up the `PlatformServices` in your scene, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `PlatformServicesShutdown`.
4. Select the `PlatformServices` object you created earlier and add the `PlatformServicesShutdown` script you created earlier.
5. Update the `PlatformServicesShutdown` class in your `PlatformServicesShutdown.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/PlatformServicesShutdown.cs#PlatformServices_Shutdown)]

This script cleans up of the services when the scene is closed.

### Create the behaviour for managing assets

To create the behaviour for asset management, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `AssetManagementBehaviour`.
4. Open the `AssetManagementBehaviour` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedManagementExample.cs#Example)]

The script does the following:

* Provides the functions to list and select Organizations.
* Provides the functions to list and select Projects.
* Performs a basic search to list the assets of the selected Project.
* Provides the functions to create, read, update, delete assets.

### Create an interface for all UI scripts

To create the interface for all UI scripts, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `IAssetManagementUI`.
4. Open the `IAssetManagementUI` script you created earlier and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/IAssetManagementUI.cs#Example)]

### Create the UI for selecting an organization

To create a simple UI for selecting an organization, do the following:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `OrganizationSelectionExampleUI`.
4. Open the `OrganizationSelectionExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/OrganizationSelectionExampleUI.cs#Example)]

The script does the following:

* Provides the UI to list and select organizations.

### Create the UI for selecting a project

To create a simple UI for selecting a project, do the following:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `ProjectSelectionExampleUI`.
4. Open the `ProjectSelectionExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/ProjectSelectionExampleUI.cs#Example)]

The script does the following:

* Provides the UI to list and select projects.

### Create the UI for selecting an asset

To create a simple UI for selecting an asset, do the following:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `AssetSelectionExampleUI`.
4. Open the `AssetSelectionExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/AssetSelectionExampleUI.cs#Example)]

The script does the following:

* Provides the UI to list and select assets.

### Create the UI for CRUD operations on assets

To create a simple UI for CRUD operations on assets, do the following:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseCreateAssetExampleUI`.
4. Open the `UseCaseCreateAssetExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseCreateAssetExample.cs#Example)]

5. In your Unity Project window, go to **Assets** > **Scripts**.
6. Select and hold the `Assets/Scripts` folder.
7. Go to **Create** > **C# Script**. Name your script `UseCaseManageAssetExampleUI`.
8. Open the `UseCaseManageAssetExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageAssetExample.cs#Example)]

The scripts do the following:

* Provide UI buttons to select an asset type and a UI button to create a new asset.
* Provide UI to update the name, type and tags of the selected asset.

### Integrate the UI scripts

To bring all your UI scripts into a single `Monobehaviour`, open the `AssetManagementUI` script you created earlier and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedManagementExampleUI.cs#Example)]

The script does the following:

* Registers to the `ICompositeAuthenticator` to track login changes.
* Creates an instance of an `AssetManagementBehaviour`.
* Creates instances of the UI scripts for selecting an organization, project, and asset.

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
