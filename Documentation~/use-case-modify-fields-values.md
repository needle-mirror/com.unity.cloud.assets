# Use case: Modify the accepted values of field definitions in an organization

You can use the Unity Cloud Assets package to add and remove accepted values for field definitions of the `Selection` type.

>[!NOTE]
>To update accepted values of field definitions in an organization, you need the [`Asset Manager Admin`](https://docs.unity.com/cloud/en-us/asset-manager/org-project-roles#organization-level-roles) role at the organization level.

## Before you start

Before you start,  do the following:

1. Set up a Unity scene in the Unity Editor with an Organization and Project browser. Read more about [setting up a Unity scene](get-started-management.md#Set-up-a-Unity-scene).
2. Create assets in Unity Cloud any of the following ways:

   * Add assets using the [Asset SDK](get-started-management.md#Create-an-asset).
   * Add [single](https://docs.unity.com/cloud/en-us/asset-manager/single-asset) or [multiple](https://docs.unity.com/cloud/en-us/asset-manager/multiple-assets) assets through the dashboard.

## How do I...?

### List and select the field definitions in an organization

To list the existing field definitions in an organization, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created  as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFieldDefinitionsModifyAcceptedValuesExample.cs#Example_Behaviour_RefreshMetadata)]

The code snippet does the following:

* Fills a list of field definitions of the `Selection` type for the selected organization.
* Holds a reference to the selected field.

### Modify the accepted values of a field definition

To modify the accepted values of a field definition, follow these steps:

1. Open the `AssetManagementBehaviour` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-the-behavior-for-managing-assets).
2. Add the following code to the end of the class:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFieldDefinitionsModifyAcceptedValuesExample.cs#Example_Behaviour_ModifyAcceptedValues)]

The code snippet does the following:

* Exposes a method to add a new value to the selected field definition.
* Exposes a method to remove a value from the selected field definition.

### Add a UI for listing and modifying field definitions

To create a UI for listing and managing field definitions, follow these steps:

1. In your Unity Project window, go to **Assets** > **Scripts**.
2. Select and hold the `Assets/Scripts` folder.
3. Go to **Create** > **C# Script**.
4. Name your script `UseCaseFieldDefinitionsModifyAcceptedValuesExampleUI`.
5. Open the `UseCaseFieldDefinitionsModifyAcceptedValuesExampleUI` script that you created in the previous step and replace the contents of the file with the following code sample:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFieldDefinitionsModifyAcceptedValuesExample.cs#Example_UIClass)]

6. In the same script, replace the `OnGUI` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseFieldDefinitionsModifyAcceptedValuesExample.cs#Example_UIContent)]

7. Open the `AssetManagementUI` script that you created as described in [Get started with Asset SDK](get-started-management.md#Create-an-AssetManager) and replace the contents of the `Awake` function with the following code:

[!code-cs [behaviour-script](../Samples/Documentation/Manual/Usecases/UseCaseSetupExamples.cs#UseCaseModifyAcceptedValues)]

The code snippet does the following:

   * Displays a list of the selected organization's field definitions. Next to each field definition displays a **Select** UI button.
   * When you select a field definition, additional UI elements list the field definition's accepted values.
