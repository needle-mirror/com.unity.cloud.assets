using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    public class DatasetCreation : DatasetInfo, IDatasetCreation
    {
        /// <inheritdoc/>
        public Dictionary<string, MetadataValue> Metadata { get; set; }

        public DatasetCreation(string name)
            : base(name) { }
    }
}
