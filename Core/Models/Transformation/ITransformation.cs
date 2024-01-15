using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public interface ITransformation
    {
        /// <summary>
        /// The descriptor of the transformation.
        /// </summary>
        TransformationDescriptor Descriptor { get; }

        /// <summary>
        /// The ID of the Dataset on which the transformation is applied
        /// </summary>
        DatasetId InputDatasetId => Descriptor.DatasetId;

        /// <summary>
        /// The ID of the Dataset that will be created by the transformation if any
        /// </summary>
        DatasetId OutputDatasetId { get; }

        /// <summary>
        /// The ID of the Dataset that will be linked to the transformation if any
        /// </summary>
        DatasetId LinkDatasetId { get; }

        /// <summary>
        /// The files on which the transformation is applied
        /// </summary>
        IEnumerable<string> InputFiles { get; }

        /// <summary>
        /// The type of transformation
        /// </summary>
        WorkflowType WorkflowType { get; }

        /// <summary>
        /// The status of the transformation
        /// </summary>
        TransformationStatus Status { get; }

        /// <summary>
        /// If the transformation failed, this will contain the associated error message
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// The datetime at which the transformation was created
        /// </summary>
        DateTime CreatedOn { get; }

        /// <summary>
        /// The datetime at which the transformation was last updated
        /// </summary>
        DateTime UpdatedAt { get; }

        /// <summary>
        /// The datetime at which the transformation was started
        /// </summary>
        DateTime StartedAt { get; }
    }
}
