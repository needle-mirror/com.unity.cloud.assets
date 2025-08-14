# Use case: Navigating public asset libraries

You can use the Unity Cloud Assets package to view public asset libraries and their contents, such as assets, datasets, and files. You can also copy assets from public libraries to your own project.

| Organization or Asset Manager Project role                                                           | List libraries | List assets/datasets/files in libraries | Download files | Copy assets from libraries to projects |
|:-----------------------------------------------------------------------------------------------------|:---------------|-----------------------------------------|:---------------|:---------------------------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes | yes | yes | no                                     |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes | yes | yes | no                                     |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes | yes | yes | yes                                    |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes | yes | yes | yes                                    |

## Before you start

Before you start, you must:

* Set up a Unity scene in the Unity Editor with an Asset Library browser. See [Get started with Asset Libraries](get-started-libraries.md) for more information.

## How do I...?

### Download files for assets in public Asset Libraries

If you have not already done so, you must update the behaviour to get the list of files.

See the [Use case: Update an asset's files](use-case-update-files.md#download-a-file) documentation for more information.

### List collections in public Asset Libraries

To list the collections of an Asset Library, follow these steps:

1. Open the `AssetLibrariesBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryCollectionsExample.cs#Example_Behaviour_RefreshCollections)]

The code snippet does the following:

* Maintains a list of collections and their properties for the selected Asset Library.

To create UI for listing collections, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseListAssetLibraryCollectionsExampleUI`.
4. Open the `UseCaseListAssetLibraryCollectionsExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryCollectionsExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryCollectionsExample.cs#Example_UIContent)]

6. Open the `AssetLibrariesBehaviour` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseListLibraryCollections)]

The code snippet does the following:

* Displays a list of collections in the selected Asset Library. Each collection has a UI button to select it and display its properties.

### List labels in public Asset Libraries

To list the labels of an Asset Library, follow these steps:

1. Open the `AssetLibrariesBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryLabelsExample.cs#Example_Behaviour_RefreshLabels)]

The code snippet does the following:

* Maintains a list of labels and their properties for the selected Asset Library.

To create UI for listing labels, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseListAssetLibraryLabelsExampleUI`.
4. Open the `UseCaseListAssetLibraryLabelsExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryLabelsExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryLabelsExample.cs#Example_UIContent)]

6. Open the `AssetLibrariesBehaviour` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseListLibraryLabels)]

The code snippet does the following:

* Displays a list of labels in the selected Asset Library. Each label has a UI button to select it and display its properties.

### List field definitions for assets in public Asset Libraries

To list the field definitions of an asset in an Asset Library, follow these steps:

1. Open the `AssetLibrariesBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryFieldDefinitionsExample.cs#Example_Behaviour_RefreshMetadata)]

The code snippet does the following:

* Maintains a list of field definitions and their properties for the selected Asset Library.

To create UI for listing field definitions, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseListAssetLibraryFieldDefinitionsExampleUI`.
4. Open the `UseCaseListAssetLibraryFieldDefinitionsExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryFieldDefinitionsExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryFieldDefinitionsExample.cs#Example_UIContent)]

6. Open the `AssetLibrariesBehaviour` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseListLibraryFieldDefinitions)]

The code snippet does the following:

* Displays a list of assets in the selected Asset Library. Each asset has a UI button to select it and display its properties.
* Displays a list of field definitions in the selected Asset Library. Each field definition has a UI button to select it and display its properties.

### List jobs actively copying assets from public Asset Libraries to your project

To create UI for listing Asset Library jobs, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `AssetLibraryJobSelectionExampleUI`.
4. Open the `AssetLibraryJobSelectionExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/AssetLibraryJobSelectionExampleUI.cs#Example)]

5. In your Unity Project window, go to **Assets** > **Scripts**.
6. Select and hold the `Assets/Scripts` folder.
7. Go to **Create** > **C# Script**. Name your script `UseCaseViewAssetLibraryJobExampleUI`.
8. Open the `UseCaseViewAssetLibraryJobExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryFieldDefinitionsExample.cs#Example_UIClass)]

9. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseListAssetLibraryFieldDefinitionsExample.cs#Example_UIContent)]

10. Open the `AssetLibrariesBehaviour` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseListLibraryJobs)]

The code snippets do the following:

* Display a list of jobs for the selected Asset Library. Each job has a UI button to select it and display its properties.

### Copy assets from public Asset Libraries to your project

To copy assets from a public Asset Library to your project, follow these steps:

1. Open the `AssetLibrariesBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseStartAssetLibraryJobExample.cs#Example_Behaviour_CopyAsset)]

The code snippet does the following:

* Starts a job on the selected Asset Library to copy the selected asset.

To create UI for copying assets, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseStartAssetLibraryJobExampleUI`.
4. Open the `UseCaseStartAssetLibraryJobExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseStartAssetLibraryJobExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseStartAssetLibraryJobExample.cs#Example_UIContent)]

6. Open the `AssetLibrariesBehaviour` script you created and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseStartLibraryJobs)]

The code snippet does the following:

* Displays a list of assets in the selected Asset Library. Each asset has a UI button to select it.
* Displays the necessary fields to start a job to copy the selected asset from the Asset Library to your project.