using System;

namespace Unity.Cloud.Assets
{
    public class DatasetCreation : DatasetInfo, IDatasetCreation
    {
        public DatasetCreation(string name)
            : base(name) { }
    }
}
