# Get started with Asset Management

Asset Management is a Unity Cloud service that allows you to manage your project's assets. You can use Asset Management to:

* Get an asset
* Search for assets
* Aggregate assets based on a set of criteria
* Create, read, update, and delete assets

This section explains how to set up a basic scene and script to initialize and use the Unity Cloud Assets package with Asset Management. The script performs a basic search for all assets of the selected project and displays the results in a simple GUI.

## Requirements

To use Asset Management, you must have the following:

For downloading assets:
* A minimum role of **Manager** in the Unity Cloud Organization you belong to OR a minimum role of **Asset Manager Contributor** in the Unity Cloud Project you belong to.

For managing assets:

## Configure the package

To configure the Unity Cloud Assets package, you must do the following:
* Set up a Unity scene
* Create an AssetManager
* Create the PlatformServices

### Set up a Unity scene

To set up a Unity scene, perform the the following steps:
1. In your Unity project window, navigate to **Assets** > **Scenes**.
2. Right-click the `Assets/Scenes` folder and navigate to **Create** > **Scene**.
3. Name the new scene `AssetManagementExample`.

### Create an AssetManager

To create a `MonoBehaviour`, perform the following steps:
1. In your Unity project window, go to **Assets** > **Scripts**. Create an `Assets/Scripts` folder if the folder doesn't already exist.
2. Right-click the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `AssetManagementUI`.
3. In the `AssetManagementExample` scene you created earlier, right-click the hierarchy and select **Create Empty**.
4. Name your new object `AssetManager`.
5. Select the `AssetManager` object and add the `AssetManagementUI` script you created earlier.

### Create the PlatformServices

To instantiate the necessary providers and managers, follow these steps:

1. Implement the platform services pattern. See [Best practices: dependency injection](https://docs.unity3d.com/Packages/com.unity.cloud.identity@0.16/manual/best-practices-dependency-injection.html) for more information.
2. Update the `PlatformServices` class in your `PlatformServices.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Management/PlatformServices.cs#PlatformServices)]

What this script accomplishes:
* Initializes an `IAuthenticator` for logging in and verifying your identity when accessing the http services.
* Initializes an `IOrganizationProvider` to fetch the organizations you belong to.
* Initializes an `IProjectProvider` to fetch the projects you have access to.
* Initializes an `IAssetManager` to fetch and manage your assets; also acts as an `IAssetProvider`.

To properly initialize the `PlatformServices` in your scene, follow these steps:

1. In your Unity project window, go to **Assets** > **Scripts**.
2. Right-click the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `PlatformServicesInitialization`.
3. In the `AssetManagementExample` scene you created earlier, right-click the hierarchy and select **Create Empty**.
4. Name your new object `PlatformServices`.
5. Select the `PlatformServices` object and add the `PlatformServicesInitialization` script you created earlier.
6. Update the `PlatformServicesInitialization` class in your `PlatformServicesInitialization.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Management/PlatformServicesInitialization.cs#PlatformServices_Initialization)]

What this script accomplishes:
* Triggers the creation of the services available in the `PlatformServices`.
* Initializes the `IAuthenticator`.

To properly clean up the `PlatformServices` in your scene, follow these steps:

1. In your Unity project window, go to **Assets** > **Scripts**.
2. Right-click the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `PlatformServicesShutdown`.
3. Select the `PlatformServices` object you created earlier and add the `PlatformServicesShutdown` script you created earlier.
4. Update the `PlatformServicesShutdown` class in your `PlatformServicesShutdown.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Management/PlatformServicesShutdown.cs#PlatformServices_Shutdown)]

What this script accomplishes:
* Clean up of the services when the scene is closed.

### Create the behaviour for managing assets

To create the behaviour for asset management, perform the following steps:
1. In your Unity project window, go to **Assets** > **Scripts**.
2. Right-click the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `AssetManagementBehaviour`.
3. Open the `AssetManagementBehaviour` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/GetStartedManagementExample.cs#Example)]

The script does the following:

* Provides the functions to list and select organizations.
* Provides the functions to list and select projects.
* Performs a basic search to list the assets of the selected project.
* Provides the functions to create, read, update, delete assets.

### Create the UI for navigating assets

To create a simple UI for navigating assets, do the following:
Open the `AssetManagementUI` script you created earlier and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/GetStartedManagementExampleUI.cs#Example)]

The script does the following:

* Registers to the `IAuthenticator` to track login changes.
* Creates an instance of an `AssetManagementBehaviour`.
* Creates a simple UI flow for selecting organizations, projects and do CRUD operations on assets.

