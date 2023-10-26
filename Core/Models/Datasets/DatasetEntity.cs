using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class DatasetEntity : IDataset
    {
        const int k_MD5_bufferSize = 4096;

        readonly IAssetDataSource m_DataSource;

        /// <inheritdoc />
        public DatasetDescriptor Descriptor { get; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> SystemTags { get; set; }

        /// <inheritdoc />
        public string Status { get; set; }

        /// <inheritdoc />
        public AuthoringInfo AuthoringInfo { get; set; }

        /// <inheritdoc />
        public IDeserializable PortalMetadata { get; set; }

        /// <inheritdoc />
        public IDeserializable Metadata { get; set; }

        /// <inheritdoc />
        public IDeserializable SystemMetadata { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> FileOrder { get; set; }

        internal FileEntity[] Files { get; set; }

        /// <inheritdoc />
        public bool IsVisible { get; set; }

        /// <summary>
        /// The name of the workflow.
        /// </summary>
        internal string WorkflowName { get; set; }

        internal DatasetEntity(IAssetDataSource assetDataSource, DatasetDescriptor datasetDescriptor, IEnumerable<FileEntity> files = null)
            : this(datasetDescriptor)
        {
            m_DataSource = assetDataSource;

            if (files != null)
            {
                Files = files.ToArray();
            }
        }

        internal DatasetEntity(DatasetDescriptor datasetDescriptor)
        {
            Descriptor = datasetDescriptor;
        }

        /// <inheritdoc />
        public async Task RefreshAsync(DatasetFields datasetFields, CancellationToken cancellationToken)
        {
            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.datasets,
                DatasetFields = datasetFields,
                FileFields = FileFields.none
            };
            var data = await m_DataSource.GetDatasetAsync(Descriptor, filter, cancellationToken);
            this.MapFrom(data, datasetFields);
            if (datasetFields.HasFlag(DatasetFields.files))
            {
                await RefreshFiles(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetAsync(FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            var data = await m_DataSource.GetAssetAsync(Descriptor.AssetDescriptor, includedFieldsFilter, cancellationToken);
            return data.From(m_DataSource, Descriptor.AssetDescriptor);
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IDatasetUpdate datasetUpdate, CancellationToken cancellationToken)
        {
            await m_DataSource.UpdateDatasetAsync(Descriptor, datasetUpdate.From(), cancellationToken);
            await RefreshAsync(DatasetFields.all, default);
        }

        /// <inheritdoc />
        public async Task<IFile> AddExistingFileAsync(string filePath, DatasetId sourceDatasetId, CancellationToken cancellationToken)
        {
            await m_DataSource.ReferenceFileFromDatasetAsync(Descriptor, sourceDatasetId, filePath, cancellationToken);

            await RefreshFiles(cancellationToken);
            return Files?.FirstOrDefault(x => x.Descriptor.Path == filePath);
        }

        /// <inheritdoc />
        public async Task RemoveFileAsync(string filePath, CancellationToken cancellationToken)
        {
            await m_DataSource.RemoveFileFromDatasetAsync(Descriptor, filePath, cancellationToken);

            Files = null; // Will force a refresh of the files the next time they are accessed.
        }

        /// <inheritdoc />
        public async Task<IFile> GetFileAsync(string filePath, CancellationToken cancellationToken)
        {
            if (Files == null) await RefreshFiles(cancellationToken);

            return Files?.FirstOrDefault(x => x.Descriptor.Path == filePath);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IFile> ListFilesAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (Files == null) await RefreshFiles(cancellationToken);

            if (Files == null || Files.Length == 0) yield break;

            if (FileOrder == null || !FileOrder.Any())
            {
                var (start, length) = range.GetValidatedOffsetAndLength(Files.Length);
                for (var i = start; i < start + length; ++i)
                {
                    yield return Files[i];
                }
            }
            else
            {
                var fileOrderArray = FileOrder.ToArray();
                var (start, length) = range.GetValidatedOffsetAndLength(fileOrderArray.Length);
                for (var i = start; i < start + length; ++i)
                {
                    yield return Files.FirstOrDefault(x => x.Descriptor.Path == fileOrderArray[i]);
                }
            }
        }

        /// <inheritdoc />
        public async Task RemoveUserMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            await m_DataSource.RemoveDatasetMetadataAsync(Descriptor, "metadata", keys, cancellationToken);
            await RefreshAsync(DatasetFields.metadata, default);
        }

        /// <inheritdoc />
        public async Task RemoveSystemMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            await m_DataSource.RemoveDatasetMetadataAsync(Descriptor, "systemMetadata", keys, cancellationToken);
            await RefreshAsync(DatasetFields.systemMetadata, default);
        }

        /// <inheritdoc />
        public Uri GetFileUrl(string filePath)
        {
            var fileUriBuilder = new UriBuilder(m_DataSource.GetServiceUrl())
            {
                Path = $"assets/storage/v1/projects/{Descriptor.ProjectId}/assets/{Descriptor.AssetId}/versions/{Descriptor.AssetVersion}/datasets/{Descriptor.DatasetId}/files/{filePath}"
            };

            return fileUriBuilder.Uri;
        }

        /// <inheritdoc />
        public async Task<IFile> UploadFileAsync(IFileCreation fileCreation, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var checksum = await CalculateMD5ChecksumAsync(sourceStream, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            var createInternal = new FileCreateData()
            {
                Path = fileCreation.Path,
                Description = fileCreation.Description,
                Metadata = fileCreation.Metadata ?? new JsonObject(new Dictionary<string, string>()), // WORKAROUND until backend supports null metadata
                PortalMetadata = fileCreation.PortalMetadata,
                SystemMetadata = fileCreation.SystemMetadata ?? new JsonObject(new Dictionary<string, string>()), // WORKAROUND until backend supports null system metadata
                UserChecksum = checksum,
                SizeBytes = sourceStream.Length,
                Tags = fileCreation.Tags?.ToList() ?? new List<string>(), // WORKAROUND until backend supports null tags
            };

            var path = fileCreation.Path;
            var pendingfile = await m_DataSource.CreateFileAsync(Descriptor, createInternal, cancellationToken);
            if (cancellationToken.IsCancellationRequested) // if file was created but external code requested cancellation
            {
                await m_DataSource.RemoveFileFromDatasetAsync(Descriptor, path, default);
            }

            if (pendingfile.UploadUrl != null) //file is new for this dataset, needs to be uploaded
            {
                try
                {
                    await m_DataSource.UploadContentAsync(pendingfile.UploadUrl, sourceStream, progress, cancellationToken);
                    await m_DataSource.FinalizeFileUploadAsync(new FileDescriptor(Descriptor, path), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    await m_DataSource.RemoveFileFromDatasetAsync(Descriptor, path, default);
                }
            }

            await RefreshFiles(cancellationToken);
            return Files?.FirstOrDefault(x => x.Descriptor.Path == path);
        }

        static async Task<string> CalculateMD5ChecksumAsync(Stream stream, CancellationToken cancellationToken)
        {
            var position = stream.Position;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                //In this method, MD5 algorythm is used for calculating checksum of a stream or a file before uploading it.
                //It is not used in a sensitive context.
#pragma warning disable S4790 //Using weak hashing algorithms is security-sensitive
                using (var md5 = MD5.Create())
#pragma warning restore S4790
                {
                    var result = new TaskCompletionSource<bool>();
                    await TaskUtils.Run(async () =>
                    {
                        try
                        {
                            await CalculateMD5ChecksumInternalAsync(md5, stream, cancellationToken);
                        }
                        finally
                        {
                            result.SetResult(true);
                        }
                    }, cancellationToken);
                    await result.Task;
                    return BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant();
                }
            }
            finally
            {
                stream.Position = position;
            }
        }

        static async Task CalculateMD5ChecksumInternalAsync(MD5 md5, Stream stream, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[k_MD5_bufferSize];
            int bytesRead;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
#if UNITY_WEBGL && !UNITY_EDITOR
                bytesRead = await Task.FromResult(stream.Read(buffer, 0, k_MD5_bufferSize));
#else
                bytesRead = await stream.ReadAsync(buffer, 0, k_MD5_bufferSize, cancellationToken);
#endif
                if (bytesRead > 0)
                {
                    md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                }
            } while (bytesRead > 0);

            md5.TransformFinalBlock(buffer, 0, 0);
            await Task.CompletedTask;
        }

        async Task RefreshFiles(CancellationToken cancellationToken)
        {
            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.files,
                DatasetFields = DatasetFields.none,
                FileFields = FileFields.none
            };
            var data = await m_DataSource.GetAssetAsync(Descriptor.AssetDescriptor, filter, cancellationToken);

            Files = data.Files?
                .Where(f => f.DatasetIds.Contains(Descriptor.DatasetId))
                .Select(fileData => fileData.From(m_DataSource, new FileDescriptor(Descriptor, fileData.Path), FileFields.none))
                .ToArray();
        }
    }
}
