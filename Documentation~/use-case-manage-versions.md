# Use case: Manage the versions of an asset

Use the Unity Cloud Assets package to perform the following:

* Search the versions of an asset.
* Freeze a version.
* Create an editable version from a frozen version.

>[!NOTE]
>To manage assets, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level or the [`Asset Manager Contributor`]( https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#project-level-roles) add-on role at the project level. Asset Manager Contributors can manage assets only for the specific projects to which they have access.


## Before you start

Before you start, do the following:

1. Verify you have the required permissions. Read more about [verifying permissions](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#verify-your-permissions).

   >[!NOTE]
   >Asset Manager roles define the permissions that you have for a single Asset Manager project. Depending on your work, permissions may vary across projects.

3. Set up a Unity scene in the Unity Editor with an Organization and Project browser. Read more about [setting up a Unity scene](get-started-management.md#Set-up-a-Unity-scene).
4. Create assets in Unity Cloud any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [a single asset](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple assets](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) through the dashboard.


## How do I...?

### List an asset's versions

To list an asset's versions, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_SearchVersions)]

The code snippet does the following:

* Creates a query to search the versions of an asset.
* Fills a list of versions.

### Freeze a version

To freeze the editable version of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_FreezeVersion)]

The code snippet does the following:

* Freezes the specific asset.
* Refreshes each listed version.
* Displays a success message in the console.

### Create a new version

To create a new version of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_CreateVersion)]

The code snippet does the following:

* Creates a new version of the asset from the specific frozen version.
* Refreshes the current list of versions for the asset.
* Displays a success message in the console.

### Delete an unfrozen version

To delete an unfrozen version of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_DeleteVersion)]

The code snippet does the following:

* Deletes the specific unfrozen version of the asset.
* Refreshes the current list of versions for the asset.
* Displays a success message in the console.

### Add a UI for managing versions

To create a UI for managing the versions of an asset, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseVersionSearchExampleUI`.
5. Open the `UseCaseVersionSearchExampleUI` script you created in the previous step and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script]( ../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseVersionSearch)]

The code snippet does the following:

* Provides fields to specify the sorting field and order for the list of versions.
* Provides a UI button to refresh the list of versions.
* Displays the list of versions with a **Select** button next to each version.
* When you select a version, the UI displays information on this version and provides buttons to freeze, delete, or create a new version based on the frozen state of the selected version.

## Going further

Read more about [further examples of managing versions](asset-management-sample.md).
