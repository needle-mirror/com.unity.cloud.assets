using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class TransformationEntity : ITransformation
    {
        readonly IAssetDataSource m_DataSource;

        /// <inheritdoc />
        public TransformationDescriptor Descriptor { get; }

        /// <inheritdoc />
        public DatasetId OutputDatasetId { get; set; }

        /// <inheritdoc />
        public DatasetId LinkDatasetId { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> InputFiles { get; set; }

        /// <inheritdoc />
        public WorkflowType WorkflowType { get; set; }

        /// <inheritdoc />
        public TransformationStatus Status { get; set; }

        /// <inheritdoc />
        public string ErrorMessage { get; set; }

        /// <inheritdoc />
        public int Progress { get; set; }

        /// <inheritdoc />
        public DateTime CreatedOn { get; set; }

        /// <inheritdoc />
        public DateTime UpdatedAt { get; set; }

        /// <inheritdoc />
        public DateTime StartedAt { get; set; }

        internal TransformationEntity(TransformationDescriptor descriptor)
        {
            Descriptor = descriptor;
        }
    }
}
