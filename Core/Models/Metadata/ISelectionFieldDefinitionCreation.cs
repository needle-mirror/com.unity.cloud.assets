using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface ISelectionFieldDefinitionCreation : IFieldDefinitionCreation
    {
        /// <inheritdoc cref="ISelectionFieldDefinition.Multiselection"/>
        bool Multiselection { get; }

        /// <inheritdoc cref="ISelectionFieldDefinition.AcceptedValues"/>
        List<string> AcceptedValues { get; }
    }
}
