#if UC_MOCK_ASSETS
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class MockDataSource : IAssetDataSource
    {
        const byte k_SizeBytes = 5;

        static PendingFileData GetDefaultPendingFile(string name, IFileCreateData fileInfo)
        {
            var ub = new UriBuilder();
            return new PendingFileData
            {
                Path = name,
                Description = fileInfo.Description,
                Metadata = fileInfo.Metadata,
                PortalMetadata = fileInfo.PortalMetadata,
                SystemMetadata = fileInfo.SystemMetadata,
                Tags = fileInfo.Tags != null ? new List<string>(fileInfo.Tags) : new List<string>(),
                UploadUrl = ub.Uri,
                UserChecksum = "userChecksum"
            };
        }

        static FileData GetDefaultFile(string filePath)
        {
            return new FileData
            {
                Path = filePath,
                Description = $"{filePath} description",
                Metadata = null,
                PortalMetadata = null,
                SystemMetadata = null,
                Tags = new List<string>(),
                UserChecksum = "userChecksum",
                SizeBytes = k_SizeBytes,
            };
        }

        /// <inheritdoc />
        public async Task<IPendingFileData> CreateFileAsync(DatasetDescriptor datasetDescriptor, IFileCreateData fileCreation, CancellationToken token)
        {
            await Task.CompletedTask;
            var file = GetDefaultPendingFile(fileCreation.Path, fileCreation);
            file.Path = fileCreation.Path;
            file.UserChecksum = fileCreation.UserChecksum;
            return file;
        }

        /// <inheritdoc />
        public async Task<IFileData> GetFileAsync(FileDescriptor fileDescriptor, FieldsFilter fieldsFilter, CancellationToken token)
        {
            await Task.CompletedTask;
            var fileData = GetDefaultFile(fileDescriptor.Path);
            fileData.DatasetIds = new[] { fileDescriptor.DatasetId };
            return fileData;
        }

        /// <inheritdoc />
        public Task UpdateFileAsync(FileDescriptor fileDescriptor, IFileBaseData fileUpdate, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<Uri> GetFileDownloadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken token)
        {
            await Task.CompletedTask;
            return new Uri($"file://download/{fileDescriptor.Path}");
        }

        /// <inheritdoc />
        public async Task<Uri> GetFileUploadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken token)
        {
            await Task.CompletedTask;
            return new Uri($"file://upload/{fileDescriptor.Path}");
        }

        /// <inheritdoc />
        public Task FinalizeFileUploadAsync(FileDescriptor fileDescriptor, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<FileTag[]> GenerateFileTagsAsync(FileDescriptor fileDescriptor, CancellationToken token)
        {
            var tags = new FileTag[]
            {
                new()
                {
                    Tag = "tag1",
                    Confidence = 1f
                },
                new()
                {
                    Tag = "tag2",
                    Confidence = 0f
                }
            };

            await Task.CompletedTask;
            return tags;
        }

        /// <inheritdoc />
        public Task RemoveFileMetadataAsync(FileDescriptor fileDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
#endif
