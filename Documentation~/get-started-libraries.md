# Get started with Public Asset Libraries

Asset Manager is a Unity Cloud service that allows you to access public libraries of assets in the cloud. Use Assets SDK to perform the following:

* Read an asset library, its assets, datasets, and files.
* Download files.
* Search your assets in a library based on a set of criteria.
* Group and count assets based on a set of criteria.
* Copy assets from a public library to your asset project.
* Read the collections, field definitions, and labels available in a library.

This section explains how to set up a basic scene and script to initialize and use the Unity Assets package with Asset Manager.
The script performs a search for all assets of the selected library and displays the results in a GUI.

Before you begin, verify you meet the [prerequisites](prerequisites.md).

## Requirements

To use Assets SDK, you must have a minimum role of [**Asset Manager Viewer**](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) in your Unity Cloud project.

## Integrate the package in a Unity project

To integrate the Unity Cloud Assets package in a Unity project, do the following:

* [Set up a Unity scene](#set-up-a-unity-scene)
* [Create an `AssetLibraryManager`](#create-an-assetlibrarymanager)
* [Create the `PlatformServices`](#create-the-platformservices)
* [Create the base behavior for managing assets](#create-the-base-behavior-for-managing-assets)
* [Create the behavior for managing library assets](#create-the-behavior-for-managing-library-assets)
* [Create the base MonoBehaviour for the UI](#create-the-base-monobehaviour-for-the-ui)
* [Create an interface for all UI scripts](#create-an-interface-for-all-ui-scripts)

### Set up a Unity scene

To set up a Unity scene, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scenes**.
2. Select and hold the `Assets/Scenes` folder.
3. Go to **Create** > **Scene**.
4. <a name="a1"></a>Name the new scene `AssetLibrariesExample`.

### Create an AssetLibraryManager

To create an `AssetLibraryManager` object, first create a `MonoBehaviour` class to manage the UI and then create the `AssetLibraryManager` object in your scene as follows:

1. Create a `MonoBehaviour` class to manage the UI:
   1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
   2. Create an `Assets/Scripts` folder if the folder does not already exist.
   3. Select and hold the `Assets/Scripts` folder.
   4. Go to **Create** > **C# Script**.
   5. <a name="a2"></a>Name your script `AssetLibrariesUI`.
2. Create the `AssetLibraryManager` object in your scene:
   1. In the `AssetLibrariesExample` scene that you created [here](#a1), select and hold the hierarchy and select **Create Empty**.
   2. Name your new object `AssetLibraryManager`.
   3. Select the `AssetLibraryManager` object and add the `AssetLibrariesUI` script that you created [here](#a2).

### Create the PlatformServices

If you have not already done so, you must set up the `PlatformServices` class to manage the platform services that Assets SDK uses to access the Asset Manager service.

See the [Get started with Asset SDK](get-started-management.md#create-the-platformservices) documentation for more information.

### Create the base behavior for managing assets

If you have not already done so, you must create a base behavior for asset management. This base behavior provides the basic functionality to search and list assets in a library.

See the [Get started with Asset SDK](get-started-management.md#create-the-base-behavior-for-managing-assets) documentation for more information.

### Create the behavior for managing library assets

To create the behavior for managing library assets, follow these steps:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. <a name="a6"></a>Name your script `AssetLibrariesBehaviour`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetLibrariesBehaviour.cs#Example)]

This script does the following:

* Inherits from the `BaseAssetBehaviour` class that you created [here](#create-the-base-behavior-for-managing-assets).
* Provides the functions to list and select libraries.

### Create the base MonoBehaviour for the UI

If you have not already done so, you must create a base `MonoBehaviour` for the UI. This base `MonoBehaviour` provides the basic functionality to manage the UI for asset management.

See the [Get started with Asset SDK](get-started-management.md#create-the-base-monobehaviour-for-the-ui) documentation for more information.

### Create an interface for all UI scripts

If you have not already done so, you must create an interface for all UI scripts. This interface provides a common contract for all UI scripts to implement, ensuring consistency in how they interact with the asset management system.

See the [Get started with Asset SDK](get-started-management.md#create-an-interface-for-all-ui-scripts) documentation for more information.

## How do I...?

### Select a library

To create a UI for selecting a library, do the following:

1. In the **Project** window of the Unity Editor, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. 
4. Name your script `LibrarySelectionExampleUI`.
5. Open the file and replace its contents with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/AssetLibrarySelectionExampleUI.cs#Example)]

This script generates a UI to list and select libraries.

### Select an asset

If you have not already done so, you must create the UI to list and select assets.

See the [Get started with Asset SDK](get-started-management.md#select-an-asset) documentation for more information.

### View an asset

If you have not already done so, you must create the UI to display the details of the selected asset.

See the [Get started with Asset SDK](get-started-management.md#view-an-asset) documentation for more information.

### Integrate the UI scripts

To bring all your UI scripts into a single `MonoBehaviour`, open the `AssetLibrariesUI` script that you created [here](#a2) and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/GetStartedWithAssetLibrariesUI.cs#Example)]

This script does the following:

* Inherits from the `BaseAssetUI` class that you created [here](#create-the-base-monobehaviour-for-the-ui).
* Registers to the `ICompositeAuthenticator` interface to track sign-in changes.
* Creates an instance of the `AssetLibrariesBehaviour` script.
* Creates instances of the UI scripts for selecting a library and asset.

## Going further

Read more about [Navigating public asset libraries](use-case-asset-libraries.md).