# Use case: Create and upload files

You can use the Unity Cloud Assets package to start any transformation on a given dataset. You can also fetch any previously started ones.

The SDK supports different workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | Start a transformation | Get a transformation |
|:-----------------------------------------------------------------------------------------------------|:-----------------------|----------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no                     | no                   |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | no                     | yes                  |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes                    | yes                  |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes                    | yes                  |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Asset Management](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Asset Management](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### Start a transformation

To list the datasets of an asset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseStartTransformationExample.cs#Example_Behaviour_StartTransformation)]

The code snippet starts a transformation on a selected dataset.

### Get a transformation

To create a new dataset, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseStartTransformationExample.cs#Example_Behaviour_GetTransformation)]

The code snippet gets a transformation that has been previously started on the dataset.

