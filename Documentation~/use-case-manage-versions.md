# Use case: Manage the versions of an asset

You can use the Unity Cloud Assets package to:

* search the versions of an asset.
* freeze a version.
* create an editable version from a frozen version.

The SDK supports several workflows for users with different roles.

| Organization or Asset Manager Project role                                                           | List versions | Freeze a version         | Create an unfrozen version |
|:-----------------------------------------------------------------------------------------------------|---------------|:-------------------------|----------------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no            | no                       | no                         |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes           | no                       | no                         |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes           | yes                      | yes                        |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes           | yes                      | yes                        |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List an asset's versions

To list version, open the `AssetManagementBehaviour` script you created and add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_SearchVersions)]

The code snippet does the following:

* Creates a query to search the versions of an asset.
* Populates a list of versions.

### Freeze a version

To freeze the editable version of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_FreezeVersion)]

The code snippet does the following:

* Freezes the provided asset.
* Refreshes each listed versions.
* Prints a message to the console on success.

### Create a new version

To create a new version of an asset, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_CreateVersion)]

The code snippet does the following:

* Creates a new version of the asset from the provided frozen version.
* Prints a message to the console on success.

### Add the UI for managing versions

To create UI for managing the versions of an asset, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseVersionSearchExampleUI`.
4. Open the `UseCaseVersionSearchExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseVersionSearchExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script]( ../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseVersionSearch)]

The code snippet does the following:

* Provides fields to specify the sorting field and order for the list of versions.
* Provides a UI button to refresh the list of versions.
* Displays the list of versions with buttons to select a version.
* When a version is selected, the UI displays information on this version and provides buttons to freeze or create a new version based on the frozen state of the selected version.

## Going further

For a further examples of managing versions, see the [Asset Management sample](asset-management-sample.md).
