using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public interface IDatasetUpdate : IDatasetInfo
    {
        /// <summary>
        /// The order the files should be accessed in.
        /// </summary>
        IReadOnlyList<string> FileOrder { get; }

        /// <summary>
        /// Whether the dataset is visible.
        /// </summary>
        bool IsVisible { get; }
    }
}
