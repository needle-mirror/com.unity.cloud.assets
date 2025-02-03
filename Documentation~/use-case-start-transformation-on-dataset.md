# Use case: Start a transformation on a dataset

Use the Unity Cloud Assets package to perform the following:
* Start transformations on a given dataset.
* Fetch any previously started transformations.

>[!NOTE]
>To manage assets, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.

## Before you start

Before you start, do the following:

1. Verify you have the required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

   >[!NOTE]
   >Asset Manager roles define the permissions that you have for a single Asset Manager project. Depending on your work, permissions may vary across projects.

2. Set up a Unity scene in the Unity Editor with an Organization and Project browser. Read more about [setting up a Unity scene](get-started-management.md#Set-up-a-Unity-scene).
3. Create assets in Unity Cloud any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [a single asset](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple assets](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) through the dashboard.


## How do I...?

### Start a transformation

To start a transformation on a dataset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseStartTransformationExample.cs#Example_Behaviour_StartTransformation)]

The code snippet starts a transformation on a selected dataset.

### Get a transformation

To get a transformation that has been previously started on a dataset, regardless of its completion status, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseStartTransformationExample.cs#Example_Behaviour_GetTransformation)]

The code snippet gets a transformation that was previously started on the dataset.
