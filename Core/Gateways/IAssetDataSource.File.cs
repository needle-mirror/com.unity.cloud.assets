using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial interface IAssetDataSource
    {
        Task<IPendingFileData> CreateFileAsync(DatasetDescriptor datasetDescriptor, IFileCreateData fileCreation, CancellationToken cancellationToken);

        Task<IFileData> GetFileAsync(FileDescriptor fileDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        Task UpdateFileAsync(FileDescriptor fileDescriptor, IFileBaseData fileUpdate, CancellationToken cancellationToken);

        Task<Uri> GetFileDownloadUrlAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken);

        Task<Uri> GetFileUploadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken cancellationToken);

        Task FinalizeFileUploadAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken);

        Task<FileTag[]> GenerateFileTagsAsync(FileDescriptor fileDescriptor, CancellationToken cancellationToken);

        Task RemoveFileMetadataAsync(FileDescriptor fileDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken cancellationToken);
    }
}
