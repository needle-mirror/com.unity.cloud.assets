# Use case: Manage field definitions in an organization

You can use the Unity Cloud Assets package to create, delete, and edit the field definitions of an organization.

>[!NOTE]
>To create, delete, and edit the field definitions of an organization, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level.

## Before you start

Before you start, do the following:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. Read more about [setting up a Unity scene](get-started-management.md#Set-up-a-Unity-scene).
2. Create assets in Unity Cloud any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [single](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) assets through the dashboard.

## How do I...?

### List and select the field definitions in an organization

To list the existing field definitions in an Organization, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_Behaviour_RefreshMetadata)]

The code snippet does the following:

* Fills a list of field definitions of the selected organization.
* Holds a reference to the selected field.
* Holds a reference to the object that is used to update the field.

### Create a field definition

To create a field definition, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_Behaviour_CreateMetadata)]

The code snippet does the following:

* Creates a new field definition based on the values specified in the `IFieldDefinitionCreation` object.
* Displays a success message in the console.

### Update a field definition

To update a field definition, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_Behaviour_UpdateMetadata)]

The code snippet does the following:

* Updates the selected field definition.
* Displays a success message in the console.

### Delete a field definition

To delete a field definition, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_Behaviour_DeleteMetadata)]

The code snippet does the following:

* Deletes the field definition passed as a parameter.
* Refreshes the list of field definitions of the Organization.
* If the deleted field definition was the currently selected one, it sets the current selection to `null`.
* Displays a success message in the console.

### Add a UI for listing and managing field definitions

To create a UI for listing and managing field definitions, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseManageFieldDefinitionsExampleUI`.
5. Open the `UseCaseManageFieldDefinitionsExampleUI` script that you created in the previous step and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_UIContent)]

7. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseManageFields)]

The code snippet does the following:

   * Displays a list of the selected Organization's field definitions. Next to each field definition displays a **Select** and a **Delete** UI button.
   * Displays a UI button to create a new field definition.
   * When you select a field definition, additional UI elements display the field definition's information.
