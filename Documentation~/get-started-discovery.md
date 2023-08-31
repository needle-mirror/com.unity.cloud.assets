# Get started with Asset Discovery

Asset Discovery is a Unity Cloud service that allows you to discover and manage your project's published assets. You can use Asset Discovery to:

* Get a published asset
* Search for published assets based on a set of criteria
* Aggregate published assets based on a set of criteria

This section explains how to set up a basic scene and script to initialize and use the Unity Asset Manager SDK package with Asset Discovery.
It performs a basic search for published assets of the selected project and displays the results in a simple GUI.

## Requirements

To use Asset Discovery, you must have a minimum role of:

* [**Manager**](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in the Unity Cloud Organization you belong to. <br/>
OR
* [**Asset Manager Viewer**](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in the Unity Cloud Project you belong to.

## Configure the package

To configure the Unity Asset Manager SDK package, you must do the following:

* Set up a Unity scene
* Create an `AssetDiscovery`
* Create the `PlatformServices`

### Set up a Unity scene

To set up a Unity scene, perform the following steps:

1. In your Unity project window, navigate to **Assets** > **Scenes**.
2. Right-click the `Assets/Scenes` folder and navigate to **Create** > **Scene**.
3. Name the new scene `AssetDiscoveryExample`.

### Create an AssetDiscovery

To create a `MonoBehaviour`, perform the following steps:

1. In your Unity project window, go to **Assets** > **Scripts**. Create an `Assets/Scripts` folder if the folder doesn't already exist.
2. Right-click the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `AssetDiscoveryUI`.
3. In the `AssetDiscoveryExample` scene you created earlier, right-click the hierarchy and select **Create Empty**.
4. Name your new object `AssetDiscovery`.
5. Select the `AssetDiscovery` object and add the `AssetDiscoveryUI` script you created earlier.

### Create the PlatformServices

To instantiate the necessary providers and managers, follow these steps:

1. Implement the platform services pattern. See the **Best practices: dependency injection** page of the [Identity package documentation](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest) for more information.
2. Update the `PlatformServices` class in your `PlatformServices.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Discovery/PlatformServices.cs#PlatformServices)]

What this script accomplishes:

* Initializes an `IAuthenticator` for logging in and verifying your identity when accessing the HTTP services.
* Initializes an `IOrganizationProvider` to fetch the organizations you belong to.
* Initializes an `IProjectProvider` to fetch the projects you have access to.
* Initializes an `IAssetProvider` to fetch your assets.

To initialize the `PlatformServices` in your scene, follow these steps:

1. In your Unity project window, go to **Assets** > **Scripts**.
2. Right-click the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `PlatformServicesInitialization`.
3. In the `AssetDiscoveryExample` scene you created earlier, right-click the hierarchy and select **Create Empty**.
4. Name your new object `PlatformServices`.
5. Select the `PlatformServices` object and add the `PlatformServicesInitialization` script you created earlier.
6. Update the `PlatformServicesInitialization` class in your `PlatformServicesInitialization.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Discovery/PlatformServicesInitialization.cs#PlatformServices_Initialization)]

What this script accomplishes:

* Triggers the creation of the services available in the `PlatformServices`.
* Initializes the `IAuthenticator`.

To clean up the `PlatformServices` in your scene, follow these steps:

1. In your Unity project window, go to **Assets** > **Scripts**.
2. Right-click the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `PlatformServicesShutdown`.
3. Select the `PlatformServices` object you created earlier and add the `PlatformServicesShutdown` script you created earlier.
4. Update the `PlatformServicesShutdown` class in your `PlatformServicesShutdown.cs` file to look like the following:

[!code-cs [platform-services](../Samples/Documentation/Manual/Discovery/PlatformServicesShutdown.cs#PlatformServices_Shutdown)]

This script cleans up of the services when the scene is closed.

### Create the behaviour for getting assets

To create the behaviour for asset discovery, perform the following steps:

1. In your Unity project window, go to **Assets** > **Scripts**.
2. Right-click the `Assets/Scripts` folder and go to **Create** > **C# Script**. Name your script `AssetDiscoveryBehaviour`.
3. Open the `AssetDiscoveryBehaviour` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Discovery/GetStartedDiscoveryExample.cs#Example)]

The script does the following:

* Provides the functions to list and select organizations.
* Provides the functions to list and select projects.
* Performs a basic search to list the published assets of the selected project.

### Create the UI for navigating assets

To create a simple UI for navigating assets, do the following:
Open the `AssetDiscoveryUI` script you created earlier and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Discovery/GetStartedDiscoveryExampleUI.cs#Example)]

The script does the following:

* Registers to the `IAuthenticator` to track login changes.
* Creates an instance of an `AssetDiscoveryBehaviour`.
* Creates a simple UI flow for selecting organizations, projects and assets.