# Sample: Manage assets

Use the Asset Management sample, which is available as part of Unity Cloud Assets, to update and publish assets.
This guide provides step-by-step instructions for developers looking to integrate asset management features in their app.

>[!NOTE]
>To update and publish assets, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.

## Before you start

Before you use the Asset Management sample, make sure you have the following:

* The required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

   >  **Note**: Asset Manager roles define the permissions that you have for a single Asset Manager project. Depending on your work, permissions might vary across projects.

* An installed [Assets](installation.md) package.
* An installed [Identity](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest) package.
* A valid [Unity ID Account](https://id.unity.com/).
* Access to your [Unity Gaming Services account](https://dashboard.unity3d.com/).
* Access to the [Asset Manager service](https://docs.unity3d.com/docs-asset-manager/manual/get-started.html).
* A Unity Project with the Asset Manager services enabled. Read more about [creating a Unity project](https://docs.unity.com/cloud/en-us/asset-manager/new-asset-manager-project).

>[!NOTE]
>Although the Assets package itself does not depend on the Identity service, this service is used in the sample to control the authentication process.

## Install the sample

To install the sample, follow these steps:

1. In your Unity Editor project, go to **Window** > **Package Manager**.
3. In the **Packages** list of the **Package Manager**, select **Unity Cloud Assets**.
4. Select the **Samples** tab.
5. Select **Import** next to **Asset Management**.

   ![Screenshot of the samples import section of the package manager window](images/import-manager-sample.png)

4. When the import is complete, the imported sample appears in the `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Management` folder.

## Run the sample

To run the sample, follow these steps:

1. Go to this folder: `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Management/Scenes`.
1. Open **AssetManagerSample.unity** and run the scene.
1. To access your projects, sign in if you haven't already done so: 

   1. In the **Game** section of the sample, select **Login**. 
   1. In the web browser that opens, select **Allow Login Request**.
   1. When a message informs you that you're signed in, close the browser and go back to the Unity Editor.


1. Select an organization. The list of projects in the selected organization displays on the left panel.
 
   ![Screenshot of the organization selection](images/organizations.png)
   
1. To show the list of assets and their details in a project, select a project.

   ![Screenshot of the project list](images/project-list.png)

   ![Screenshot of the assets grid](images/assets-grid.png)

### Search for specific assets

To search for specific assets by tag or name in the sample, follow these steps:

1. Select a project.
2. In the search bar, enter the keywords that you want to search for in asset tags and asset names.
3. Select **Search**.

   ![Screenshot of the created topic](images/search-in-manager.png)

>[!NOTE]
>Build different and more complex queries with the SDK. Read more about [asset search](use-case-search-assets.md).

#### Search prompts

As you enter text in the search bar, the sample suggests keywords that are derived from the tags and names of your assets.
Select a keyword. The sample updates the search results accordingly.

### Manage an asset

>[!NOTE]
>You can update an asset only if it's unfrozen and in draft status.

To update an asset in the sample, follow these steps:

1. Select a project. The list of assets in the selected project displays in a table.
2. In the last column of the table, select the ellipsis button (…) next to the asset.
3. Select **Open**. The asset details page opens.
   
   ![Screenshot of the open button](images/open-selected-asset.png)
   
4. Edit the asset's information and select **Save asset**. 
5. To add metadata to the asset, select **Add Metadata**.
  
   ![Screenshot of the asset edit panel](images/asset-edit-panel.png)
   
6. To add a new dataset to the asset, select **Create dataset**. The system automatically generates the dataset name.
   
   ![Screenshot of the add dataset button](images/asset-details-create-dataset.png)
  
   ![Screenshot of the new created dataset in list](images/asset-edit-new-created-dataset.png)
   
7. To update a dataset or manage the files of an asset, select the desired dataset. The dataset details page opens.
  
   ![Screenshot of the dataset in list](images/asset-edit-dataset-list.png)
   
8. Edit the dataset's information and select **Save dataset**.
   
   ![Screenshot of the dataset panel](images/dataset-edit-panel.png)
   
9. To generate a thumbnail, select **Generate thumbnail**. The system creates a new dataset that contains an image file.
10. To add a new file to the dataset, select **Browse** and select the desired file.
   
      ![Screenshot of the browse button](images/dataset-edit-panel-browse-button.png)
   
11. To generate a preview image, select **Generate preview**.

    >[!NOTE]
    >To generate a preview image, the dataset must have at least one file.

12. To remove a file from the dataset, select the delete icon.
  
    ![Screenshot of the remove button](images/trash-icon-button.png)
    
13. To go back to the previous page, select **Back**.

#### Actions that are available when an asset is unfrozen

When an asset is unfrozen, you can do the following actions:

 * To save the asset, select **Save changes**.
 * To publish the asset, select **Publish**.
 * To freeze the asset, select **Save version**.

   ![Screenshot of the save asset button](images/asset-details-buttons-unfrozen.png)
  
 >[!NOTE]
 > * The **Sources** dataset is a default dataset of an asset, which lists the asset data files.
 > * The **Previews** dataset is a default dataset of an asset, which lists the asset preview files.

 
#### Actions that are available when an asset is frozen

When an asset is frozen, you can do the following actions:

* To publish the asset, select **Publish**.
* To create a new version based on the current version, select **Continue editing**.

  ![Screenshot of the save asset button](images/asset-details-buttons-frozen.png)
 
### Browse the asset's version history

By default, the details panel displays the list of datasets for the default version of the asset. To browse the asset's version history in this sample, follow these steps:

1. Select the asset.
2. To browse the asset's version history, select the **History** tab.
   </br>
   ![Screenshot of the history button](images/asset-details-tabs.png)
2. Select a version in the asset's version history to view its details.
3. To go back to the asset's datasets, expand the **Datasets** section.

## Main components

This section describes the scripts that make up the main components of the Asset Management sample.

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

The `ProjectController` class inherits from the `OrganizationController` class, which enables signing in to your application and uses your ID to grant access to the Asset Discovery sample. Read more about [authentication](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest?subfolder=/manual/use-case-getting-user-information.html).
The `ProjectController` class uses the `IOrganizationRepository` interface of the `PlatformServices` class to retrieve the list of your organizations.
The `ProjectController` class uses the `IAssetRepository` interface of the `PlatformServices` class to retrieve the list of your projects for the selected organization.

To open the project controller script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers/ProjectController.cs` file.

### Asset Management sample script

The `AssetManagerSample` script shows you how to do the following:

* Integrate the login flow with the `ProjectController` class.
* Retrieve organizations and projects from the Asset Manager service.
* Retrieve assets from the Asset Manager service and manage them.
* Search for assets by tag or name.

To open the Asset Management sample script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetManager/Scripts/AssetManagerSample.cs` file.

### Shared UI scripts

The `UI` sample includes a set of UI scripts and prefabs used by Unity Cloud Assets samples. To open shared UI scripts, go to `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers`.

## Troubleshooting

In case of issues with samples, refer to [troubleshooting](troubleshooting.md#sample-issues).
