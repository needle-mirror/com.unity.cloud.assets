using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Assets.Transformations;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource
    {
        /// <inheritdoc/>
        public async Task<TransformationId> StartTransformationAsync(DatasetDescriptor datasetDescriptor, WorkflowType workflowType, CancellationToken cancellationToken)
        {
            var request = new StartTransformationRequest(workflowType, datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId);
            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();

            var startedTransformationResponse = IsolatedSerialization.DeserializeWithConverters<StartedTransformationDto>(jsonContent, IsolatedSerialization.TransformationIdConverter);

            return startedTransformationResponse.TransformationId;
        }

        /// <inheritdoc/>
        public async Task<ITransformationData> GetTransformationAsync(TransformationDescriptor transformationDescriptor, CancellationToken cancellationToken)
        {
            var request = new GetTransformationRequest(transformationDescriptor.TransformationId,
                transformationDescriptor.ProjectId, transformationDescriptor.AssetId,
                transformationDescriptor.AssetVersion, transformationDescriptor.DatasetId);

            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();

            return IsolatedSerialization.DeserializeWithDefaultConverters<TransformationData>(jsonContent);
        }
    }
}
