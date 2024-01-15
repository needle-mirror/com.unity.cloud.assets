# Use case: Modify the accepted values of field definitions in an organization

You can use the Unity Cloud Assets package to add and remove the accepted values of field definitions of type `Selection`.


| Organization or Asset Manager Project role                                                           | Modifying accepted values of field definitions |
|:-----------------------------------------------------------------------------------------------------|------------------------------------------------|
| [`Asset Management Viewer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)      | no                                             |
| [`Asset Management Consumer`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles)    | no                                             |
| [`Asset Management Contributor`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles) | no                                             |
| [`Organization Owner`](https://docs.unity.com/cloud/en-us/accounts/roles-and-permissions)            | yes                                            |

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

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFieldDefinitionsModifyAcceptedValuesExample.cs#Example_Behaviour_RefreshMetadata)]

The code snippet does the following:

* Populates a list of field definitions of type `Selection` for the selected Organization.
* Holds a reference to the selected field.

### Modify the accepted values of a field definition

To modify the accepted values of a field definition, follow these steps:

1. Open the `AssetManagementBehaviour` script you created.
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFieldDefinitionsModifyAcceptedValuesExample.cs#Example_Behaviour_ModifyAcceptedValues)]

The code snippet does the following:

* Exposes a method to add a new value to the selected field definition.
* Exposes a method to remove an value from the selected field definition.

### Add the UI for listing and modifying field definitions

To create UI for listing and managing field definitions, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**. Name your script `UseCaseFieldDefinitionsModifyAcceptedValuesExampleUI`.
4. Open the `UseCaseFieldDefinitionsModifyAcceptedValuesExampleUI` script you created and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFieldDefinitionsModifyAcceptedValuesExample.cs#Example_UIClass)]

5. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFieldDefinitionsModifyAcceptedValuesExample.cs#Example_UIContent)]

6. Open the `AssetManagementUI` script you created and replace the contents of the `Awake` function with the following code:

```cs
   m_UI.Add(new OrganizationSelectionExampleUI(m_Behaviour));
   m_UI.Add(new UseCaseFieldDefinitionsModifyAcceptedValuesExampleUI(m_Behaviour));
```

The code snippet does the following:

   * Displays a list of the selected Organization's field definitions. Each field definition has a UI button to select it.
   * When a field definition is selected, additional UI elements list the field definition's accepted values.
