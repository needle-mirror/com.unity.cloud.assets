using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class DatasetCreation : DatasetInfo, IDatasetCreation
    {
        /// <inheritdoc/>
        public Dictionary<string, object> Metadata { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, object> SystemMetadata { get; set; }

        public DatasetCreation(string name)
            : base(name) { }
    }
}
