using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IDatasetCreation : IDatasetInfo
    {
        /// <summary>
        /// The user metadata of the dataset.
        /// </summary>
        Dictionary<string, object> Metadata { get; }

        /// <summary>
        /// The system metadata of the dataset.
        /// </summary>
        Dictionary<string, object> SystemMetadata { get; }
    }
}
