# Use case: Manage field definitions in an organization

You can use the Unity Cloud Assets package to create, delete, and edit the field definitions of an organization.


| Asset Manager Project role                                                                           | Getting field definitions | Create/delete/edit field definitions |
|:-----------------------------------------------------------------------------------------------------|:--------------------------|--------------------------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | yes                       | no                                   |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | yes                       | no                                   |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | yes                       | no                                   |
| [`Asset Management Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)        | yes                       | yes                                  |

## Before you start

Before you start, you must:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. See [Get started with Assets](get-started-management.md) for more information.
2. Have some assets in the cloud. There are several ways to do so:

   * You can create assets through the [Get started with Assets](get-started-management.md).
   * You can upload assets from existing Unity assets; see the [Asset Database Uploader sample](./asset-database-uploader-sample.md).
   * You can create assets through the dashboard; see the [Managing assets on the dashboard](https://docs.unity3d.com/docs-asset-manager/manual/add-asset.html) documentation.

## How do I...?

### List and select the field definitions in an organization

To list the existing field definitions in an Organization, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_Behaviour_RefreshMetadata)]

The code snippet does the following:

* Populates a list of field definition in the selected Organization.
* Holds a reference to the selected field.
* Holds a reference to the object used to update the field.

### Create a field definition

To create a field definition, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_Behaviour_CreateMetadata)]

The code snippet does the following:

* Creates a new field definition given an `IFieldDefinitionCreation` object.
* Prints a message to the console on success.

### Update a field definition

To update a field definition, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_Behaviour_UpdateMetadata)]

The code snippet does the following:

* Updates the selected field definition.
* Prints a message to the console on success.

### Delete a field definition

To delete a field definition, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_Behaviour_DeleteMetadata)]

The code snippet does the following:

* Deletes the field definition passed as a parameter.
* Refreshes the list of field definition for the Organization.
* If the deleted field definition was the currently selected one, it sets the current selection to `null`.
* Prints a message to the console on success.

### Add the UI for listing and managing field definitions

To create UI for listing and managing field definitions, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseManageFieldDefinitionsExampleUI`.
4. Open the `UseCaseManageFieldDefinitionsExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseManageFieldDefinitionsExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

```cs
   m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
   m_UI.Add(new UseCaseManageFieldDefinitionsExampleUI(m_Behaviour));
```

The code snippet does the following:

   * Displays a list of the selected Organization's field definitions. Each field definition has a UI button to select it and a UI button to delete it.
   * Displays a UI button to create a new field definitions.
   * When a field definition is selected, additional UI elements display the field definition's information.
