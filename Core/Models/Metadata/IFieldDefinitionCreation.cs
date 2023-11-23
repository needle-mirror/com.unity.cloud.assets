namespace Unity.Cloud.Assets
{
    public interface IFieldDefinitionCreation : IFieldDefinitionUpdate
    {
        /// <inheritdoc cref="IFieldDefinition.Key"/>
        string Key { get; }

        /// <inheritdoc cref="IFieldDefinition.Type"/>
        FieldDefinitionType Type { get; }

        /// <inheritdoc cref="IFieldDefinition.Multiselection"/>
        bool? Multiselection { get; }
    }
}
