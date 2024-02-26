using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.Assets
{
    public abstract class DatasetInfo : IDatasetInfo
    {
        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public List<string> Tags { get; set; }

        /// <inheritdoc />
        public bool? IsVisible { get; set; }

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
            Tags = dataset.Tags?.ToList() ?? new List<string>();
            IsVisible = dataset.IsVisible;
        }
    }
}
