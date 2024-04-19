using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource
    {
        /// <inheritdoc />
        public async Task<IPendingFileData> CreateFileAsync(DatasetDescriptor datasetDescriptor, IFileCreateData fileCreation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new CreateFileRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, fileCreation);
            var response = await RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = JsonSerialization.Deserialize<UploadUrlDto>(jsonContent);

            return new PendingFileData
            {
                Path = fileCreation.Path,
                Description = fileCreation.Description,
                Metadata = fileCreation.Metadata,
                Tags = fileCreation.Tags != null ? new List<string>(fileCreation.Tags) : new List<string>(),
                UserChecksum = fileCreation.UserChecksum,
                UploadUrl = new Uri(dto.UploadUrl)
            };
        }

        /// <inheritdoc />
        public async Task<IFileData> GetFileAsync(FileDescriptor fileDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var assetData = await GetAssetAsync(fileDescriptor.DatasetDescriptor.AssetDescriptor, includedFieldsFilter, cancellationToken);
            var file = assetData.Files.FirstOrDefault(f => f.Path == fileDescriptor.Path);
            if (file == null)
            {
                throw new NotFoundException($"File with path \"{fileDescriptor.Path}\" not found at that location.");
            }

            return file;
        }

        /// <inheritdoc />
        public Task UpdateFileAsync(FileDescriptor fileDescriptor, IFileBaseData fileUpdate, CancellationToken cancellationToken)
        {
            var request = new FileRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.Path,
                fileUpdate);

            return RateLimitedServiceClient(request, HttpClientExtensions.HttpMethodPatch).PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Uri> GetFileDownloadUrlAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetFileDownloadUrlRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.DatasetId,
                fileDescriptor.Path,
                null);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request), ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = JsonSerialization.Deserialize<FileUrl>(jsonContent);

            return new Uri(dto.Url);
        }

        /// <inheritdoc />
        public async Task<Uri> GetFileUploadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GetFileUploadUrlRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.DatasetId,
                fileDescriptor.Path,
                fileData);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = JsonSerialization.Deserialize<FileUrl>(jsonContent);

            return new Uri(dto.Url);
        }

        /// <inheritdoc />
        public Task FinalizeFileUploadAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken)
        {
            var request = new FinalizeFileUploadRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.Path);
            return RateLimitedServiceClient(request, HttpMethod.Post).PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<FileTag[]> GenerateFileTagsAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = new GenerateFileTagsRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.DatasetId,
                fileDescriptor.Path);
            var response = await RateLimitedServiceClient(request, HttpMethod.Get).GetAsync(GetPublicRequestUri(request),
                ServiceHttpClientOptions.Default(), cancellationToken);

            var jsonContent = await response.GetContentAsString();
            cancellationToken.ThrowIfCancellationRequested();

            var dto = JsonSerialization.Deserialize<FileTags>(jsonContent);

            return dto.Tags;
        }

        /// <inheritdoc />
        public Task RemoveFileMetadataAsync(FileDescriptor fileDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var request = new RemoveMetadataRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.DatasetId,
                fileDescriptor.Path,
                metadataType,
                keys);
            return RateLimitedServiceClient(request, HttpMethod.Delete).DeleteAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), cancellationToken);
        }
    }
}
