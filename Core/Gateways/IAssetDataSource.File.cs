using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial interface IAssetDataSource
    {
        Task<IPendingFileData> CreateFileAsync(DatasetDescriptor datasetDescriptor, IFileCreateData fileCreation, CancellationToken token);

        Task<IFileData> GetFileAsync(FileDescriptor fileDescriptor, FieldsFilter includedFieldsFilter, CancellationToken token);

        Task UpdateFileAsync(FileDescriptor fileDescriptor, IFileBaseData fileUpdate, CancellationToken token);

        Task<Uri> GetFileDownloadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken token);

        Task<Uri> GetFileUploadUrlAsync(FileDescriptor fileDescriptor, IFileData fileData, CancellationToken token);

        Task FinalizeFileUploadAsync(FileDescriptor fileDescriptor, CancellationToken token);

        Task<FileTag[]> GenerateFileTagsAsync(FileDescriptor fileDescriptor, CancellationToken token);

        Task RemoveFileMetadataAsync(FileDescriptor fileDescriptor, string metadataType, IEnumerable<string> keys, CancellationToken token);
    }
}
