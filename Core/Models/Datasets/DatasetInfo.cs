using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.Assets
{
    public abstract class DatasetInfo : IDatasetInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }

        protected DatasetInfo(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "The name of the dataset cannot be null or empty.");
            }

            Name = name;
        }

        protected DatasetInfo(IDataset dataset)
            : this(dataset.Name)
        {
            Description = dataset.Description;
            Tags = dataset.Tags?.ToList();
        }
    }
}
