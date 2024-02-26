using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IDatasetUpdate : IDatasetInfo
    {
        /// <inheritdoc cref="IDataset.FileOrder"/>
        IReadOnlyList<string> FileOrder { get; }
    }
}
