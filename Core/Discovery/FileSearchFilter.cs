using System;
using System.Collections;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IFile"/> search request.
    /// </summary>
    public class FileSearchFilter : ComplexSearchCriteria<IFile>
    {
        /// <inheritdoc cref="FileDescriptor.Path"/>
        public SearchCriteria<string> Path { get; } = new(nameof(FileDescriptor.Path), "filePath");

        /// <inheritdoc cref="IFile.Description"/>
        public StringSearchCriteria Description { get; } = new(nameof(IFile.Description), "description");

        /// <inheritdoc cref="IFile.Status"/>
        public SearchCriteria<string> Status { get; } = new(nameof(IFile.Status), "status");

        /// <inheritdoc cref="IFile.Tags"/>
        public HashsetSearchCriteria<string> Tags { get; } = new(nameof(IFile.Tags), "tags");

        /// <inheritdoc cref="IFile.SystemTags"/>
        public HashsetSearchCriteria<string> SystemTags { get; } = new(nameof(IFile.SystemTags), "systemTags");

        /// <inheritdoc cref="IFile.AuthoringInfo"/>
        public AuthoringInfoSearchFilter AuthoringInfo { get; } = new(nameof(IFile.AuthoringInfo), string.Empty);

        /// <inheritdoc cref="IFile.SizeBytes"/>
        public SearchCriteria<long> SizeBytes { get; } = new(nameof(IFile.SizeBytes), "sizeBytes");

        /// <inheritdoc cref="IFile.PortalMetadata"/>
        public DeserializableSearchCriteria PortalMetadata { get; } = new(nameof(IFile.PortalMetadata), "portalMetadata");

        /// <inheritdoc cref="IFile.Metadata"/>
        public MetadataSearchFilter Metadata { get; } = new(nameof(IFile.Metadata), "metadata");

        /// <inheritdoc cref="IFile.SystemMetadata"/>
        public MetadataSearchFilter SystemMetadata { get; } = new(nameof(IFile.SystemMetadata), "systemMetadata");

        private protected override Type InstantiatedType => typeof(FileEntity);

        internal FileSearchFilter(string propertyName, string searchKey)
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
