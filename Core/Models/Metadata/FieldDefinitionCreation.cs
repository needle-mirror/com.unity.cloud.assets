namespace Unity.Cloud.Assets
{
    public class FieldDefinitionCreation : FieldDefinitionUpdate, IFieldDefinitionCreation
    {
        /// <inheritdoc/>
        public string Key { get; set; } = string.Empty;

        /// <inheritdoc/>
        public FieldDefinitionType Type { get; set; } = default;

        /// <inheritdoc/>
        public bool? Multiselection { get; set; }

        public FieldDefinitionCreation() { }
    }
}
