# Sample: Manage assets

This sample is available as a part of Unity Cloud Assets and enables users to update and publish assets.
An example of an audience for this guide are developers who want to integrate asset management features in their app.


## Before you start

To use the Asset Management sample, make sure you meet the following prerequisites:

### Roles and permissions

The sample uses management endpoints that require you to have any one of the below roles:

* [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles). <br/> 
* [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions).

### Prerequisites 

* Installed [Assets](installation.md) package
* Installed [Identity](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest) package
* A valid [Unity ID Account](https://id.unity.com/)
* Access to your [Unity Gaming Services account](https://dashboard.unity3d.com/)
* Access to [Asset Manager service](https://docs.unity3d.com/docs-asset-manager/manual/get-started.html)
* A Unity Project with Asset Manager services enabled, see: [Asset Manager documentation](https://docs.unity3d.com/docs-asset-manager/manual/modify-project.html)

>[!NOTE]
>The Assets package doesn't depend on the Identity service, however, it's used in the sample to control the authentication process.

## Install the sample

To install the sample, follow these steps:

1. In the Unity Editor, go to the top menu bar, select **Window**.
2. Select **Package Manager**. The package manager window opens.
3. From the Packages list, select **Unity Cloud Assets**.
2. On the right side, select the **Samples** tab.
3. In the section below, go to Asset Management and select the **Import** button.

   ![Screenshot of the samples import section of the package manager window](images/import-manager-sample.png)

4. After the import process is complete, you can view the imported sample in the `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Management` folder.
   
   ![Screenshot of the imported sample](images/tac-sample-management-scene.png)

## Run the sample

To run the sample, follow these steps:

1. Go to `Assets/Samples/Unity Cloud Assets/<package-version>/Asset Management/Scenes` folder. Open **AssetManagerSample.unity** and run the scene.

>[!NOTE]
>Ensure that you are logged in to access your Projects. To log in, follow these steps: <br> 1. Select the **Login** button on the top right side. This action redirects you to a web browser. <br> 2. Select the **Allow Login Request** button. The browser displays Login completed successfully. You can close the browser and go back to the Unity Editor. 

2. Select an Organization. The list of Projects in this Organization appear on the left panel.
   <br>
   ![Screenshot of the organization selection](images/organizations.png)
3. Select a Project. The list of assets in this Project and their details appear in a table.
   </br>
   ![Screenshot of the project list](images/project-list.png)
   </br>
   ![Screenshot of the assets grid](images/assets-grid.png)

### Search for specific assets

To search for specific assets by tag or name in this sample, follow these steps:

1. Select a Project.
2. In the search bar, type the keywords you want to use to search for your assets.
3. Select **Search**.
   </br>
   ![Screenshot of the created topic](images/search-in-manager.png)

>[!NOTE]
>You can build different and more complex queries while using the SDK. For more information on asset search, see the [Search Assets](use-case-search-assets.md) use case.

#### Search prompts

As you type in the search bar, the sample will display a list of keyword suggestions based on your search. These keywords are aggregated from the tags and names of your assets.
To pick a keyword, select it. The search bar adds it as a parameter and the sample refreshes the asset list to match the new search.

### Update/edit an asset

You can update an asset only if it's unfrozen and in Draft status. To update an asset in this sample, follow these steps:

1. Select a Project. The list of assets in this project appear in a table.
   </br>
2. Go to the last column of the table , select the **...** button for the asset you want to update. 
   </br>
3. Select **Open**. The asset details page opens.
   </br>
   ![Screenshot of the open button](images/open-selected-asset.png)
4. Edit the asset's information and select the **Save asset** button. 
  </br>
5. To add metadata to the asset, select the **Add Metadata** button.
   </br>
   ![Screenshot of the asset edit panel](images/asset-edit-panel.png)
6. To add a new dataset to the asset, select the **Create dataset** button. A new dataset with an automatic name is added to the asset.
   </br>
   ![Screenshot of the add dataset button](images/asset-details-create-dataset.png)
    </br>
   ![Screenshot of the new created dataset in list](images/asset-edit-new-created-dataset.png)
7. To update a dataset or manage the files of an asset, select the dataset you want. The dataset details page opens.
   </br>
   ![Screenshot of the dataset in list](images/asset-edit-dataset-list.png)
8. Edit the dataset's information and select the **Save dataset** button.
   </br>
   ![Screenshot of the dataset panel](images/dataset-edit-panel.png)
9. To start a workflow to generate a thumbnail, select the **Generate thumbnail** button. This will create a new dataset containing an image file.
10. To add a new file to the dataset, select the **Browse** button and select the desired file. If a dataset has at least one file, you can generate a preview image. Select the **Generate preview** button.
   </br>
   ![Screenshot of the browse button](images/dataset-edit-panel-browse-button.png)
11. To remove a file from the dataset, select the delete icon.
    </br>
    ![Screenshot of the remove button](images/trash-icon-button.png)
12. To go back to a previous page, on the top left corner, select the **Back** button.


>[!NOTE]
>When an asset is unfrozen, you can do the following actions :
> * To save the asset, select the **Save changes** button.
> 
> * To publish the asset, select the **Publish** button.
> 
> * To freeze the asset, select the **Save version** button.
>
> ![Screenshot of the save asset button](images/asset-details-buttons-unfrozen.png)
>  
> * **Sources** dataset is a default dataset of an asset. It lists the asset data files.

> * **Previews** dataset is a default dataset of an asset. It lists the asset preview files.
>
> 
>When an asset is frozen, you can do the following actions :
> * To publish the asset, select the **Publish** button.
>
> * To create a new version based off the current version, select the **Continue editing** button.
>
> ![Screenshot of the save asset button](images/asset-details-buttons-frozen.png)
>
> * **Sources** dataset is a default dataset of an asset. It lists the asset data files.

> * **Previews** dataset is a default dataset of an asset. It lists the asset preview files.

### Browse the asset's version history

By default, the details panel displays the list of datasets for the default version of the asset. To browse the asset's version history in this sample, follow these steps once an asset is selected:

1. To browse the asset's version history, select the **History** tab.
   </br>
   ![Screenshot of the history button](images/asset-details-tabs.png)
2. In the asset's version history, you can select a version to view its details.
3. To go back to the asset's datasets, select the **Datasets** tab.

## Main components

This section describes the scripts that form the main components of the Asset Management sample.

### Platform services script

The `PlatformServices` class initializes and disposes dependencies required by the `IAssetRepository`. You can use this class to manage Unity Cloud services and dependencies you use in your application.

To open the platform services script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Services/PlatformServices.cs` file.

The `PlatformServices` class has two associated classes called `PlatformServicesInitialization` and `PlatformServicesShutdown` that call the initialization and shutdown methods through Unity's standard `Monobehaviour` methods `Awake()`, `Start()` and `OnDestroy()`.

### Project controller script

The `ProjectController` class inherits from the `OrganizationController` class which enables signing in to your application and uses your ID to grant access to the Asset Management sample. For more information on authentication, see the **Get user information** use case in the [Identity package documentation](https://docs.unity3d.com/Packages/com.unity.cloud.identity@latest).
The `ProjectController` class uses the `IOrganizationRepository` of the `PlatformServices` to retrieve the list of organizations you have access to.
The `ProjectController` class uses the `IAssetRepository` of the `PlatformServices` to retrieve the list of projects you have access to in the selected organization.

To open the project controller script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers/ProjectController.cs` file.

### Asset Management sample script

The `AssetManagerSample` script helps you with the following:

* Integrate login flow with the `ProjectController` class
* Retrieve Organizations and Projects from the Asset Manager service
* Retrieve assets from the Asset Manager service and manage them
* Search for assets by tag or name

To open the Asset Management sample script, go to your `Assets/Samples/Unity Cloud Assets/<package-version>/AssetManager/Scripts/AssetManagerSample.cs` file.

### Shared UI scripts

The `UI` sample includes a set of UI scripts and prefabs used by Unity Cloud Assets samples. To open shared UI scripts, go to `Assets/Samples/Unity Cloud Assets/<package-version>/Shared/Scripts/Controllers`.

## Troubleshooting

Refer to the [troubleshooting](troubleshooting.md#sample-issues) section for help with sample issues.
