using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.Assets
{
    public class DatasetUpdate : DatasetInfo, IDatasetUpdate
    {
        public IReadOnlyList<string> FileOrder { get; set; }

        public bool IsVisible { get; set; }

        public DatasetUpdate(string name)
            : base(name)
        {
            Tags ??= new List<string>();
        }

        public DatasetUpdate(IDataset dataset)
            : base(dataset)
        {
            Tags ??= new List<string>();
            FileOrder = dataset.FileOrder?.ToList();
            IsVisible = dataset.IsVisible;
        }
    }
}
