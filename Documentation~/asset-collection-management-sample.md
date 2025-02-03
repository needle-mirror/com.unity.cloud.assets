# Sample: Manage asset collections

Use the Collection Management sample to list and manage the collections of assets in your projects.

>[!NOTE]
>To manage assets, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.

## Before you start

Before you use the Collection Management sample, make sure you have the following:

* An installed [Assets](installation.md) package.
* An installed [Identity](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest) package.
* A valid [Unity ID Account](https://id.unity.com/).
* Access to your [Unity Gaming Services account](https://dashboard.unity3d.com/).
* A Unity Project with the Asset Manager service enabled. Read more about [creating a Unity project](https://docs.unity.com/cloud/en-us/asset-manager/new-asset-manager-project).
* Access to the [Asset Manager service](https://docs.unity3d.com/docs-asset-manager/manual/get-started.html).
* At least one published asset in an Asset Manager project. Read more about [adding assets using the Asset SDK](get-started-management.md#Create-an-asset) and adding [a single asset](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple assets](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) through the dashboard.

>[!NOTE]
>Although the Assets package does not depend on the Identity service, the sample uses the service to control the authentication process.

## Install the sample

To install the sample, follow these steps:

1. Open your application project in the Unity Editor.
2. Go to **Window** > **Package Manager** > **Unity Cloud Assets**.
3. Expand the **Samples** section.
4. Next to `Collection Management`, select **Import**.

   <img alt="Screenshot of the samples import section of the package manager window" height="64" src="images/sample-import-collection-management.png"/>

5. After the import process completes, the sample displays under the `Assets/Samples/Unity Cloud Assets` folder.

  <img alt="Screenshot of the imported sample" height="256" src="images/tac-sample-collections-scene.png"/>

## Run the sample

To run the sample, follow these steps:

1. Go to `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Collection Management/Scenes/CollectionManagementSample.unity` and run the scene.
2. Select an organization to display the list of projects in the selected organization on the left panel.

   <img alt="Screenshot of the Organization selection" width="260" src="images/organizations.png"/>
   
3. Select a project to display the list of collections in the selected project on the middle panel. 

   <img alt="Screenshot of the project list" width="300" src="images/project-list.png"/>

 4. Select a collection to display the list of assets in the selected collection on the right panel.
  
    <img alt="Screenshot of the collection list" width="800" src="images/collection-list.png"/>

### Create a new collection

To create a new collection, follow these steps:

1. Next to `Collections`, select the ellipsis button (…).

   <img alt="Screenshot of the context menu" width="460" src="images/collections-context-menu-noselection.png"/>
   
2. Select **Create**.

   <img alt="Screenshot of creating collection popup" width="380" src="images/create-collection-popup.png"/>

3. Enter a name and a description for the collection.
4. Optionally, enter a parent path.
5. Select **Create**.

   <img alt="Screenshot of the created collection" width="460" src="images/collection-created.png"/>

### Edit an existing collection

To edit an existing collection, follow these steps:

1. Select a collection in the list.
2. Next to `Collections`, select the ellipsis button (…).

   <img alt="Screenshot of the context menu" width="460" src="images/collections-context-menu.png"/>
   
3. Select **Edit**.

   <img alt="Screenshot of editing collection popup" width="380" src="images/edit-collection-popup.png"/>
   
4. Enter a new name and a new description for the collection.
5. Select **Apply**.

#### Delete an existing collection

To delete an existing collection, follow these steps:

1. Select a collection in the list.
2. Next to `Collections`, select the ellipsis button (…).

   <img alt="Screenshot of the context menu" width="460" src="images/collections-context-menu.png"/>

3. Select **Delete**.

### Add assets to a collection

To add an asset to a collection, follow these steps:

1. Select a collection in the list.
2. Next to `Assets in Collection`, select the ellipsis button (…).

   <img alt="Screenshot of the context menu" width="460" src="images/collection-assets-context-menu-no-selection.png"/>
   
3. Select **Add**.

   <img alt="Screenshot of adding assets to collection popup" width="380" src="images/add-to-collection-popup.png"/>

4. Select all the assets you want to add to the collection. 
5. Select **Add**.

   <img alt="Screenshot of selected assets" width="460" src="images/collection-assets-added.png"/>

### Remove assets from a collection

To remove an asset from a collection, follow these steps:

1. Select an asset in the list.
2. Next to `Assets in Collection`, select the ellipsis button (…).

   <img alt="Screenshot of the context menu" width="460" src="images/collection-assets-context-menu.png"/>
3. Select **Remove**.

## Main components

This section describes the scripts that make up the main components of the Asset Collection Management sample.

### Platform services script

The `PlatformServices` class handles the initialization and disposal of dependencies necessary for the `IAssetRepository` interface. Use this class to manage the Unity Cloud services and dependencies in your application.

To open the platform services script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Services/PlatformServices.cs` file.

Use the following classes with the `PlatformServices` class:

* `PlatformServicesInitialization` 
* `PlatformServicesShutdown`

These classes use the following standard Unity `Monobehaviour` methods to run the initialization and shutdown methods:

* `Awake()`
* `Start()`
* `OnDestroy()`

### Project controller script

The `ProjectController` class inherits the following from the `OrganizationController` class:

* Sign in to your application.
* Use your ID to grant access to the Collection Management sample.

Read more about [authentication](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest?subfolder=/manual/use-case-getting-user-information.html).
The `ProjectController` class uses the `IOrganizationRepository` interface of the `PlatformServices` class to retrieve the list of organizations you have access to.
The `ProjectController` class uses the `IAssetRepository` interface of the `PlatformServices` class to retrieve the list of your projects for the selected organization.

To open the project controller script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers/ProjectController.cs` file.

### Asset collection management sample script

The `CollectionManagementSample` shows you how to do the following:

* Integrate the login flow with the `ProjectController` class.
* Retrieve organizations and projects from the Asset Manager service.
* Retrieve published assets from the Asset Manager service.
* Search for assets by tag or name.

To open the Collection Management sample script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Collection Management/Scripts/CollectionManagementSample.cs` file.

### Collection list, asset list, and collection asset list UI scripts

* The `CollectionListUi`, `AssetListUi`, and `CollectionAssetListUi` classes list the assets, collections, and assets belonging to a collection in the sample.
* The `AssetPanelUi` class bridges data between `AssetListUi` and `CollectionAssetListUi`.

### Shared UI scripts

The `UI` sample includes a set of UI scripts and prefabs that Unity Cloud Assets samples use. To open shared UI scripts, go to `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers`.

## Troubleshooting

In case of issues with samples, refer to [troubleshooting](troubleshooting.md#sample-issues).
