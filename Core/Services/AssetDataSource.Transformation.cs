using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource
    {
        /// <inheritdoc/>
        public async Task<TransformationId> StartTransformationAsync(DatasetDescriptor datasetDescriptor, string workflowType, string[] inputFiles, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new StartTransformationRequest(workflowType, inputFiles, parameters, datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            var startedTransformationResponse = IsolatedSerialization.DeserializeWithConverters<StartedTransformationDto>(jsonContent, IsolatedSerialization.TransformationIdConverter);

            return startedTransformationResponse.TransformationId;
        }

        /// <inheritdoc/>
        public async Task<ITransformationData> GetTransformationAsync(TransformationDescriptor transformationDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetTransformationRequest(transformationDescriptor.TransformationId,
                transformationDescriptor.ProjectId, transformationDescriptor.AssetId,
                transformationDescriptor.AssetVersion, transformationDescriptor.DatasetId);

            using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return IsolatedSerialization.DeserializeWithDefaultConverters<TransformationData>(jsonContent);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ITransformationData> GetTransformationsAsync(ProjectDescriptor projectDescriptor, Range range, TransformationSearchData searchData, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (start, length) = range.GetValidatedOffsetAndLength(int.MaxValue);

            if (length == 0) yield break;

            var request = new SearchTransformationRequest(projectDescriptor.ProjectId, searchData);
            using var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            var transformations = IsolatedSerialization.DeserializeWithDefaultConverters<TransformationData[]>(jsonContent);
            for (var i = start; i < transformations.Length && i < start + length; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return transformations[i];
            }
        }

        /// <inheritdoc/>
        public async Task TerminateTransformationAsync(ProjectDescriptor projectDescriptor, TransformationId transformationId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new TerminateTransformationRequest(projectDescriptor.ProjectId, transformationId);
            using var _ = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(), ServiceHttpClientOptions.Default(), cancellationToken);
        }
    }
}
