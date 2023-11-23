using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IFieldDefinitionUpdate
    {
        /// <inheritdoc cref="IFieldDefinition.DisplayName"/>
        string DisplayName { get; }

        /// <inheritdoc cref="IFieldDefinition.AcceptedValues"/>
        List<string> AcceptedValues { get; }
    }
}
