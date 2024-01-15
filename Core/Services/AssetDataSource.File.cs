using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class AssetDataSource
    {
        /// <inheritdoc />
        public async Task<IPendingFileData> CreateFileAsync(DatasetDescriptor datasetDescriptor, IFileCreateData fileCreation, CancellationToken token)
        {
            var request = new CreateFileRequest(datasetDescriptor.ProjectId, datasetDescriptor.AssetId, datasetDescriptor.AssetVersion, datasetDescriptor.DatasetId, fileCreation);

            var response = await m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
            var jsonContent = await response.GetContentAsString();
            var dto = JsonSerialization.Deserialize<UploadUrlDto>(jsonContent);

            return new PendingFileData
            {
                Path = fileCreation.Path,
                Description = fileCreation.Description,
                Metadata = fileCreation.Metadata,
                SystemMetadata = fileCreation.SystemMetadata,
                Tags = fileCreation.Tags != null ? new List<string>(fileCreation.Tags) : new List<string>(),
                UserChecksum = fileCreation.UserChecksum,
                UploadUrl = new Uri(dto.UploadUrl)
            };
        }

        /// <inheritdoc />
        public async Task<IFileData> GetFileAsync(FileDescriptor fileDescriptor, FieldsFilter includedFieldsFilter, CancellationToken token)
        {
            var assetData = await GetAssetAsync(fileDescriptor.DatasetDescriptor.AssetDescriptor, includedFieldsFilter, token);
            var file = assetData.Files.FirstOrDefault(f => f.Path == fileDescriptor.Path);
            if (file == null)
            {
                throw new NotFoundException($"File with path \"{fileDescriptor.Path}\" not found at that location.");
            }

            return file;
        }

        /// <inheritdoc />
        public Task UpdateFileAsync(FileDescriptor fileDescriptor, IFileBaseData fileUpdate, CancellationToken token)
        {
            var request = new FileRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.Path,
                fileUpdate);

            return m_ServiceHttpClient.PatchAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<Uri> GetFileDownloadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken token)
        {
            var request = GetFileUrlRequest(fileDescriptor, "download", null);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request),
                ServiceHttpClientOptions.Default(), token);
            var jsonContent = await response.GetContentAsString();

            var dto = JsonSerialization.Deserialize<FileUrl>(jsonContent);

            return new Uri(dto.Url);
        }

        /// <inheritdoc />
        public async Task<Uri> GetFileUploadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken token)
        {
            var request = GetFileUrlRequest(fileDescriptor, "upload", fileData);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request),
                ServiceHttpClientOptions.Default(), token);
            var jsonContent = await response.GetContentAsString();

            var dto = JsonSerialization.Deserialize<FileUrl>(jsonContent);

            return new Uri(dto.Url);
        }

        /// <inheritdoc />
        public Task FinalizeFileUploadAsync(FileDescriptor fileDescriptor, CancellationToken token)
        {
            var request = new FinalizeFileUploadRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.Path);
            return m_ServiceHttpClient.PostAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<FileTag[]> GenerateFileTagsAsync(FileDescriptor fileDescriptor, CancellationToken token)
        {
            var request = new GenerateFileTagsRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.DatasetId,
                fileDescriptor.Path);
            var response = await m_ServiceHttpClient.GetAsync(GetPublicRequestUri(request),
                ServiceHttpClientOptions.Default(), token);
            var jsonContent = await response.GetContentAsString();

            var dto = JsonSerialization.Deserialize<FileTags>(jsonContent);

            return dto.Tags;
        }

        /// <inheritdoc />
        public Task RemoveFileMetadataAsync(FileDescriptor fileDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken token)
        {
            var request = new RemoveMetadataRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.DatasetId,
                fileDescriptor.Path,
                metadataType,
                keys);
            return m_ServiceHttpClient.DeleteAsync(GetPublicRequestUri(request), request.ConstructBody(),
                ServiceHttpClientOptions.Default(), token);
        }

        static GetFileUrlRequest GetFileUrlRequest(FileDescriptor fileDescriptor, string urlType, IFileData fileData)
        {
            return new GetFileUrlRequest(fileDescriptor.ProjectId,
                fileDescriptor.AssetId,
                fileDescriptor.AssetVersion,
                fileDescriptor.DatasetId,
                fileDescriptor.Path, urlType, fileData);
        }
    }
}
