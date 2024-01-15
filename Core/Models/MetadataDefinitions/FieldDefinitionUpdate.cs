namespace Unity.Cloud.Assets
{
    public class FieldDefinitionUpdate : IFieldDefinitionUpdate
    {
        /// <inheritdoc/>
        public string DisplayName { get; set; } = string.Empty;

        public FieldDefinitionUpdate() { }

        public FieldDefinitionUpdate(IFieldDefinition fieldDefinition)
        {
            DisplayName = fieldDefinition.DisplayName;
        }
    }
}
