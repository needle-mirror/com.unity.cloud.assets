using System;
using System.Collections;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IDataset"/> search request.
    /// </summary>
    public class DatasetSearchFilter : ComplexSearchCriteria<IDataset>
    {
        /// <inheritdoc cref="IDataset.Name"/>
        public StringSearchCriteria Name { get; } = new(nameof(IDataset.Name), "name");

        /// <inheritdoc cref="IDataset.Description"/>
        public StringSearchCriteria Description { get; } = new(nameof(IDataset.Description), "description");

        /// <inheritdoc cref="IDataset.Status"/>
        public SearchCriteria<string> Status { get; } = new(nameof(IDataset.Status), "status");

        /// <inheritdoc cref="IDataset.Tags"/>
        public HashsetSearchCriteria<string> Tags { get; } = new(nameof(IDataset.Tags), "tags");

        /// <inheritdoc cref="IDataset.SystemTags"/>
        public HashsetSearchCriteria<string> SystemTags { get; } = new(nameof(IDataset.SystemTags), "systemTags");

        /// <inheritdoc cref="IDataset.AuthoringInfo"/>
        public AuthoringInfoSearchFilter AuthoringInfo { get; } = new(nameof(IDataset.AuthoringInfo), string.Empty);

        /// <inheritdoc cref="DatasetEntity.Metadata"/>
        public MetadataSearchFilter Metadata { get; } = new(nameof(DatasetEntity.Metadata), "metadata");

        /// <inheritdoc cref="IDataset.IsVisible"/>
        public NullableSearchCriteria<bool> IsVisible { get; } = new(nameof(IDataset.IsVisible), "isVisible");

        private protected override Type InstantiatedType => typeof(DatasetEntity);

        internal DatasetSearchFilter(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        private protected override void Include(object value) => base.Include(TransformValue(value));

        private protected override void Exclude(object value) => base.Exclude(TransformValue(value));

        private protected override void ForAny(object value) => base.ForAny(TransformValue(value));

        static object TransformValue(object value)
        {
            if (value is IList list)
            {
                return list.Count > 0 ? list[0] : null;
            }

            return value;
        }
    }
}
