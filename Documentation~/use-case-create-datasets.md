# Use case: Create and upload asset files

You can use the Unity Cloud Assets package to view and create datasets within an asset.

The SDK supports different workflows for users with different roles.

| Asset Manager Project role                                                                             | View datasets | Create dataset |
|:-------------------------------------------------------------------------------------------------------|:--------------|----------------|
| [`Asset Management Viewer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)      | yes           | no             |
| [`Asset Management Consumer`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)    | yes           | no             |
| [`Asset Management Contributor`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html) | yes           | yes            |
| [`Asset Management Owner`](https://docs.unity3d.com/docs-asset-manager/manual/manage-users.html)     | yes           | yes            |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Asset Management](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Asset Management](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List datasets

To list the datasets of an asset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseDatasetCreationExample.cs#Example_Behaviour_RefreshDatasets)]

The script populates a list of datasets for the selected asset.

### Create a dataset

To create a new dataset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseDatasetCreationExample.cs#Example_Behaviour_CreateDataset)]

The script creates a new dataset with the given name on the selected asset.

### Add the UI for interacting with asset files

To add UI for the example:

1. Open the `AssetManagementUI` script you created.
2. Replace the `AssetActions` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Management/UseCaseFileCreationExample.cs#Example_UI)]

The script does the following:

* Displays a list of datasets for the selected asset.
* Provides a text input and UI button to create a new dataset.
