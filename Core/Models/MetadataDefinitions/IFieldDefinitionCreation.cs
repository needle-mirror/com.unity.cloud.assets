namespace Unity.Cloud.Assets
{
    public interface IFieldDefinitionCreation : IFieldDefinitionUpdate
    {
        /// <inheritdoc cref="FieldDefinitionDescriptor.FieldKey"/>
        string Key { get; }

        /// <inheritdoc cref="IFieldDefinition.Type"/>
        FieldDefinitionType Type { get; }
    }
}
