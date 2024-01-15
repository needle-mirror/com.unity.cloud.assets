using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Starts a transformation on the specified dataset.
        /// </summary>
        /// <param name="datasetDescriptor">The object containing the necessary information to identify the dataset on which to start the transformation.</param>
        /// <param name="workflowType">The type of workflow that will be applied in the transformation.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>The ID of the transformation </returns>
        Task<TransformationId> StartTransformationAsync(DatasetDescriptor datasetDescriptor, WorkflowType workflowType, CancellationToken cancellationToken);

        /// <summary>
        /// Get a transformation on the specified dataset.
        /// </summary>
        /// <param name="transformationDescriptor">The object containing the necessary information to identify the transformation.</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A DTO of the transformation</returns>
        Task<ITransformationData> GetTransformationAsync(TransformationDescriptor transformationDescriptor, CancellationToken cancellationToken);
    }
}
