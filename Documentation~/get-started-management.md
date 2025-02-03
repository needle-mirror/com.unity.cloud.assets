# Get started with Asset SDK

Asset Manager is a Unity Cloud service that allows you to manage your assets in the cloud. Use Assets SDK to perform the following:

* Create and read an asset project.
* Create, read, and update:
  * assets
  * datasets
  * files
* Download files.
* Create, read, update, and delete collections.
* Search your assets in an asset project or organization based on a set of criteria.
* Group and count assets based on a set of criteria.
* Link and unlink assets from projects.
* Link and unlink assets from collections.
* Start transformations on datasets.
* Create, read, update and delete the field definitions of an organization.
* Add and remove the accepted values of field definitions of the `Selection` type.

This section explains how to set up a basic scene and script to initialize and use the Unity Assets package with Asset Manager.
The script performs a search for all assets of the selected project and displays the results in a GUI.

Before you begin, verify you meet the [prerequisites](prerequisites.md).

## Requirements

To use Assets SDK, you must have a minimum role of [**Asset Manager Viewer**](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) in your Unity Cloud project.

## Integrate the package in a Unity project

To integrate the Unity Cloud Assets package in a Unity project, do the following:

* [Set up a Unity scene](#set-up-a-unity-scene)
* [Create an `AssetManager`](#create-an-assetmanager)
* [Create the `PlatformServices`](#create-the-platformservices)
* [Create the behavior for managing assets](#create-the-behavior-for-managing-assets)
* [Create an interface for all UI scripts](#create-an-interface-for-all-ui-scripts)

### Set up a Unity scene

To set up a Unity scene, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scenes**.
2. Select and hold the `Assets/Scenes` folder.
3. Go to **Create** > **Scene**.
4. <a name="a1"></a>Name the new scene `AssetManagementExample`.

### Create an AssetManager

To create an `AssetManager` object, first create a `MonoBehaviour` class to manage the UI and then create the `AssetManager` object in your scene as follows:

1. Create a `MonoBehaviour` class to manage the UI:
   1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
   2. Create an `Assets/Scripts` folder if the folder does not already exist.
   3. Select and hold the `Assets/Scripts` folder.
   4. Go to **Create** > **C# Script**.
   5. <a name="a2"></a>Name your script `AssetManagementUI`.
2. Create the `AssetManager` object in your scene:
   1. In the `AssetManagementExample` scene that you created [here](#a1), select and hold the hierarchy and select **Create Empty**.
   2. Name your new object `AssetManager`.
   3. Select the `AssetManager` object and add the `AssetManagementUI` script that you created [here](#a2).

### Create the PlatformServices

To set up the necessary components, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `PlatformServices`.
3. Open the file and replace its contents with the following code sample:

[!code-cs [platform-services](../Samples/Documentation/Manual/PlatformServices.cs#PlatformServices)]

This script does the following:

* Initializes an `ICompositeAuthenticator` interface for signing in and verifying your identity when you access the HTTP services.
* Initializes an `IOrganizationRepository` interface to fetch your organizations.
* Initializes an `IAssetRepository` interface to interact with the Asset Manager service.

To initialize the `PlatformServices` class in your scene, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. <a name="a3"></a>Name your script `PlatformServicesInitialization`.
5. In the `AssetManagementExample` scene that you created [here](#a1), select and hold the hierarchy and select **Create Empty**.
6. <a name="a4"></a>Name your new object `PlatformServices`.
7. Select the `PlatformServices` object and add the `PlatformServicesInitialization` script that you created [here](#a3).
8. Update the `PlatformServicesInitialization` class in your `PlatformServicesInitialization.cs` file as shown below:

[!code-cs [platform-services](../Samples/Documentation/Manual/PlatformServicesInitialization.cs#PlatformServices_Initialization)]

This script does the following:

* Triggers the creation of the services available in the `PlatformServices` class.
* Initializes the `ICompositeAuthenticator` interface.

To clean up the `PlatformServices` class in your scene, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `PlatformServicesShutdown`.
5. Select the `PlatformServices` object that you created [here](#a4) and add the `PlatformServicesShutdown` script that you created in the previous step.
6. Update the `PlatformServicesShutdown` class in your `PlatformServicesShutdown.cs` file as shown below:

[!code-cs [platform-services](../Samples/Documentation/Manual/PlatformServicesShutdown.cs#PlatformServices_Shutdown)]

This script cleans up of the services when the scene is closed.

> [!NOTE]
> Read more about the [platform services pattern implementation](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest?subfolder=/manual/best-practices-dependency-injection.html) in the [Identity package documentation](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest).

### Create the behavior for managing assets

To create the behavior for asset management, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. <a name="a5"></a>Name your script `AssetManagementBehaviour`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedManagementExample.cs#Example)]

This script does the following:

* Provides the functions to list and select organizations.
* Provides the functions to list and select projects.
* Performs a basic search to list the assets of the selected project.

### Create an interface for all UI scripts

To create the interface for all UI scripts, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. 
4. Name your script `IAssetManagementUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/IAssetManagementUI.cs#Example)]

## How do I...?

### Select an organization

To create a UI for selecting an organization, do the following:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. 
4. Name your script `OrganizationSelectionExampleUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/OrganizationSelectionExampleUI.cs#Example)]

This script generates a UI to list and select organizations.

### Select a project

To create a UI for selecting a project, do the following:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `ProjectSelectionExampleUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/ProjectSelectionExampleUI.cs#Example)]

This script generates a UI to list and select projects.

### Select an asset

To create a UI for selecting an asset, do the following:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `AssetSelectionExampleUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/AssetSelectionExampleUI.cs#Example)]

This script generates a UI to list and select assets.

### Create an asset

To create an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created [here](#a5).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCreationExample.cs#Example_Behaviour_CreateAsset)]

This code snippet does the following:

* Provides a method to fetch the status flows of the selected organization.
* Provides a method to create a new asset with a type and a status flow.

To create a UI for asset creation, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseAssetCreationExampleUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCreationExample.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseAssetCreationExample.cs#Example_UIContent)]

### Integrate the UI scripts

To bring all your UI scripts into a single `MonoBehaviour`, open the `AssetManagementUI` script that you created [here](#a2) and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedManagementExampleUI.cs#Example)]

This script does the following:

* Registers to the `ICompositeAuthenticator` interface to track sign-in changes.
* Creates an instance of the `AssetManagementBehaviour` script.
* Creates instances of the UI scripts for selecting an organization, project, and asset.

## Going further

Read more about [asset management](asset-management-sample.md).

### Updating assets

Read more about [updating assets](use-case-update-assets.md).

### Uploading files

Read more about [uploading files](use-case-create-files.md).

### Grouping assets in collections

Use collections to group assets within a project. Read more about [managing collections](use-case-manage-collections.md).
