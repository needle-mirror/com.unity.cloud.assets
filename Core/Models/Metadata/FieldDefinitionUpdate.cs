using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.Assets
{
    public class FieldDefinitionUpdate : IFieldDefinitionUpdate
    {
        /// <inheritdoc/>
        public string DisplayName { get; set; } = string.Empty;

        /// <inheritdoc/>
        public List<string> AcceptedValues { get; set; } = new();

        public FieldDefinitionUpdate() { }

        public FieldDefinitionUpdate(IFieldDefinition fieldDefinition)
        {
            DisplayName = fieldDefinition.DisplayName;
            AcceptedValues = fieldDefinition.AcceptedValues.ToList();
        }
    }
}
