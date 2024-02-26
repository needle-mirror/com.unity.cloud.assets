using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IDatasetCreation : IDatasetInfo
    {
        /// <inheritdoc cref="IDataset.Metadata"/>
        Dictionary<string, MetadataValue> Metadata { get; }
    }
}
