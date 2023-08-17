# Sample: Manage asset collections

You can use the Asset discovery sample to list and manage the collections of asset in your projects.

The sample uses the collection endpoints that require a minimum role of:

* [**Manager**](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in the Unity Cloud Organization you belong to. <br/>
  OR
* [**Asset Manager Contributor**](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) in the Unity Cloud Project you belong to.

## Before you start

Before you use the Collection management sample, you must have the following:

* An installed [Assets](installation.md), and [Identity](https://docs.unity3d.com/Packages/com.unity.cloud.identity@0.16/manual/installation.html) packages
> **Note**: While the Assets package itself doesn't depend on the Identity service, it is necessary in the sample to control the authentication flow.
* A valid [Unity ID Account](https://dashboard.unity3d.com/) and [access to the asset manager service](https://docs.unity3d.com/docs-asset-manager/manual/get-started.html)
* At least 1 published asset in an Asset Management Project (see [Asset Manager documentation](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html))

## Installation

To install the sample, follow these steps:

1. Inside your Unity project window, go to **Package Manager** > **Unity Cloud Assets**.
2. Expand the **Samples** section and select **Import** next to the Asset Discovery sample.
   </br>
   ![Screenshot of the samples import section of the package manager window](images/sample-import-collection-management.png)

After the import process is complete, you can view your imported assets under the `Assets/Samples/Unity Cloud Assets` folder.
</br>
![Screenshot of the imported sample](images/tac-sample-collections-scene.png)

## Run the sample

To run the sample, follow these steps:

1. Go to `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Collection Management/Scenes/CollectionManagementSample.unity` and run the scene. If this is your first time launching the sample, make sure to sign in with your Unity Gaming Services account. For more information on creating a Unity project, see the [Asset Manager documentation](https://docs.unity3d.com/docs-asset-manager/manual/index.html).
2. Select an Organization. The list of projects from that organization will be displayed on the left column.
   </br>
   ![Screenshot of the organization selection](images/organizations.png)
3. Select a project. The list of collections for that project will be displayed in the middle column as well as a section for creating a new collection. In the right column, the list of assets for the project will be displayed.
   </br>
   ![Screenshot of the project list](images/project-list.png)
   </br>
   ![Screenshot of the collection list](images/collection-list.png)
   </br>
   ![Screenshot of the collection list](images/collection-asset-list.png)

### Create a new collection

To create a new collection, follow these steps:

1. In the text field to the right of the `Collection Name` label, type a name for your collection. The create button will become active.
2. Click **Create**.
   </br>
   ![Screenshot of the created topic](images/create-collection.png)

#### Delete an existing collection

To delete an existing collection, click **Delete** next to the collection you want to delete.

### Add assets to a collection

To add an asset to a collection, follow these steps:

1. Select one of the collections in the list.
2. Click **All Assets** to display all the assets for the selected project. If it is already selected, it will appear greyed out.
3. Assets that are not already in the collection will display button. Click **Add to Collection** next to the asset you want to add to the collection. The button will disappear.
    </br>
    ![Screenshot of the created topic](images/collection-asset-add.png)
4. To view the assets that are in the selected collection, click **Collection Only**.

### Remove assets from a collection

To remove an asset from a collection, follow these steps:

1. Select one of the collections in the list.
2. Click **Collection Only** to display only the assets that are in the collection. If it is already selected, it will appear greyed out.
3. Click **Remove from Collection** next to the asset you want to remove from the collection. The asset will disappear from the list.
    </br>
    ![Screenshot of the created topic](images/collection-asset-remove.png)

## Main components

This section describes the scripts that make up the main components of the Asset Collection Management sample.

### Platform services script

The `PlatformServices` class initializes and disposes of dependencies required by the `IAssetCollectionManager`. You can use this class to manage the Unity Cloud services and dependencies you use in your application.

To open the platform services script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Services/PlatformServices.cs` file.

The `PlatformServices` class has two accompanying classes called `PlatformServicesInitialization` and `PlatformServicesShutdown` that call the initialization and shutdown methods through Unity's standard `Monobehaviour` methods `Awake()`, `Start()` and `OnDestroy()`.

### User Controller script

The `UserController` class lets you sign in and provides the Asset Discovery sample with your Unity Gaming Services ID. For more information on authentication, see the [Identity package documentation](https://docs.unity3d.com/Packages/com.unity.cloud.identity@0.16/manual/use-case-getting-user-information.html).

To open the UserController script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers/UserController.cs` file.

### Asset collection management sample script

The `CollectionManagementSample` shows you how to do the following:

* Integrate the login flow with the `UserController` class
* Retrieve organizations and projects from the Asset Manager service
* Retrieve published assets from the Asset Manager service
* Search for assets by tag or name

To open the Asset Discovery sample script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Collection Management/Scripts/CollectionManagementSample.cs` file.

### Collection list, asset list, and collection asset list UI scripts

The `CollectionListUi`, `AssetListUi`, and `CollectionAssetListUi` classes are used to display the list of assets, collections, and assets belonging to a collection in the sample.
While the `AssetPanelUi` class is used to toggle between the `AssetListUi` and `CollectionAssetListUi`.

### Collection creation controller

The `CollectionCreationController` class is used to display the UI that allows you to create a new collection.

### Shared UI scripts

The sample includes a set of UI scripts and prefabs used by our samples. To open shared UI scripts, go to `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers`.

## Troubleshoot

This section describes issues you might have while using the Asset Collection management sample.

### Missing dependency

If you get a missing dependency error about a specific package, ensure you have installed all the packages listed in the [Prerequisites](#prerequisites).

### The automatic browser redirection doesn't work

If you run the sample in the Unity Editor, you should see the following page after you successfully login through your browser.

![Login Successful](images/login-redirect.png)

If you aren't automatically redirected to the Editor and nothing happens when you select **Launch Application**, return to the Editor. This should continue the authentication process.

### I can't see my assets

If you can't see any assets, it might be that your organization doesn't have the asset management feature flag enabled. You'll need to [request access to the beta](https://docs.unity3d.com/docs-asset-manager/manual/request-access.html).

## Known issues

- Removing or adding assets quickly from a collection causes errors
- "Add to collection" button is not working properly