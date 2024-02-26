using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    class FileEntity : IFile
    {
        static readonly UCLogger k_Logger = LoggerProvider.GetLogger<FileEntity>();

        readonly IAssetDataSource m_DataSource;
        internal DatasetDescriptor[] m_LinkedDatasets = Array.Empty<DatasetDescriptor>();

        internal FileEntity(IAssetDataSource dataSource, FileDescriptor descriptor, IEnumerable<DatasetId> datasetIds)
            : this(descriptor)
        {
            m_DataSource = dataSource;
            if (datasetIds != null)
            {
                m_LinkedDatasets = datasetIds.Select(id => new DatasetDescriptor(descriptor.DatasetDescriptor.AssetDescriptor, id)).ToArray();
            }

            MetadataEntity = new FileMetadataContainer(Descriptor, FileFields.metadata, m_DataSource);
        }

        internal FileEntity(FileDescriptor fileDescriptor)
        {
            Descriptor = fileDescriptor;

            MetadataEntity = new FileMetadataContainer(Descriptor, FileFields.metadata, null);
        }

        /// <inheritdoc />
        public FileDescriptor Descriptor { get; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public string Status { get; set; }

        /// <inheritdoc />
        public AuthoringInfo AuthoringInfo { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> SystemTags { get; set; }

        /// <inheritdoc />
        public IMetadataContainer Metadata => MetadataEntity;

        /// <inheritdoc />
        public IEnumerable<DatasetDescriptor> LinkedDatasets => m_LinkedDatasets;

        /// <summary>
        /// The metadata of the file.
        /// </summary>
        public MetadataContainerEntity MetadataEntity { get; }

        /// <inheritdoc />
        public long SizeBytes { get; set; }

        public string UserChecksum { get; set; }

        internal Uri PreviewUrl { get; set; }

        internal Uri UploadUrl { get; set; }

        internal Uri DownloadUrl { get; set; }

        internal bool IsDownloadable { get; set; } = true;

        AssetDescriptor AssetDescriptor => Descriptor.DatasetDescriptor.AssetDescriptor;

        /// <inheritdoc />
        public IFile WithDataset(DatasetDescriptor datasetDescriptor)
        {
            if (datasetDescriptor == Descriptor.DatasetDescriptor) return this;

            if (!m_LinkedDatasets.Contains(datasetDescriptor))
                throw new InvalidArgumentException("The file does not belong to the specified dataset.");

            var descriptor = new FileDescriptor(datasetDescriptor, Descriptor.Path);
            return new FileEntity(m_DataSource, descriptor, m_LinkedDatasets.Select(d => d.DatasetId))
            {
                Description = Description,
                Status = Status,
                AuthoringInfo = AuthoringInfo,
                Tags = Tags?.ToArray(),
                SystemTags = SystemTags?.ToArray(),
                MetadataEntity = { Properties = MetadataEntity.Properties },
                SizeBytes = SizeBytes,
                UserChecksum = UserChecksum,
                UploadUrl = UploadUrl,
                DownloadUrl = DownloadUrl
            };
        }

        /// <inheritdoc />
        public Task RefreshAsync(CancellationToken cancellationToken)
        {
            PreviewUrl = null;
            DownloadUrl = null;
            UploadUrl = null;
            MetadataEntity.Refresh();

            return RefreshAsync(FieldsFilter.DefaultFileIncludes, cancellationToken);
        }

        async Task RefreshAsync(FieldsFilter fieldsFilter, CancellationToken cancellationToken)
        {
            var fileData = await m_DataSource.GetFileAsync(Descriptor, fieldsFilter, cancellationToken);
            this.MapFrom(m_DataSource, fileData, fieldsFilter.FileFields);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IDataset> GetLinkedDatasetsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var (start, length) = range.GetValidatedOffsetAndLength(m_LinkedDatasets.Length);
            for (var i = start; i < start + length; i++)
            {
                var dataset = await m_DataSource.GetDatasetAsync(m_LinkedDatasets[i], FieldsFilter.DefaultDatasetIncludes, cancellationToken);
                yield return dataset.From(m_DataSource, AssetDescriptor, FieldsFilter.DefaultDatasetIncludes.DatasetFields);
            }
        }

        /// <inherticdoc />
        public async Task<Uri> GetPreviewUrlAsync(CancellationToken cancellationToken)
        {
            if (PreviewUrl == null)
            {
                var filter = new FieldsFilter
                {
                    AssetFields = AssetFields.files,
                    DatasetFields = DatasetFields.none,
                    FileFields = FileFields.previewUrl
                };

                var fileData = await m_DataSource.GetFileAsync(Descriptor, filter, cancellationToken);
                this.MapFrom(m_DataSource, fileData, filter.FileFields);
            }

            return PreviewUrl;
        }

        /// <inheritdoc />
        public async Task<Uri> GetDownloadUrlAsync(CancellationToken cancellationToken)
        {
            if (!IsDownloadable) return null;

            if (DownloadUrl == null)
            {
                var data = new FileData
                {
                    Path = Descriptor.Path,
                    UserChecksum = UserChecksum,
                    SizeBytes = SizeBytes
                };
                try
                {
                    DownloadUrl = await m_DataSource.GetFileDownloadUrlAsync(Descriptor, data, cancellationToken);
                }
                catch (NotFoundException)
                {
                    IsDownloadable = false;
                    return null;
                }
            }

            return DownloadUrl;
        }

        /// <inheritdoc />
        public async Task DownloadAsync(Stream targetStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            if (!IsDownloadable) return;

            await GetDownloadUrlAsync(cancellationToken);

            try
            {
                await m_DataSource.DownloadContentAsync(DownloadUrl, targetStream, progress, cancellationToken);
            }
            catch (NotFoundException)
            {
                // If the download fails, try to get a new download url and try again.
                DownloadUrl = null;
                await GetDownloadUrlAsync(cancellationToken);
                await m_DataSource.DownloadContentAsync(DownloadUrl, targetStream, progress, cancellationToken);
            }
        }

        /// Not exposed in the interface
        public async Task<Uri> GetUploadUrlAsync(CancellationToken cancellationToken)
        {
            if (UploadUrl == null)
            {
                var data = new FileData
                {
                    Path = Descriptor.Path,
                    UserChecksum = UserChecksum,
                    SizeBytes = SizeBytes
                };
                UploadUrl = await m_DataSource.GetFileUploadUrlAsync(Descriptor, data, cancellationToken);
            }

            return UploadUrl;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IFileUpdate fileUpdate, CancellationToken cancellationToken)
        {
            await m_DataSource.UpdateFileAsync(Descriptor, fileUpdate.From(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task UploadAsync(Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            var result = Metadata.Query().ExecuteAsync(cancellationToken);
            var metadata = new Dictionary<string, MetadataValue>();
            await foreach (var item in result)
            {
                metadata.Add(item.Key, item.Value);
            }

            var datasets = new List<IDataset>();
            var datasetList = GetLinkedDatasetsAsync(Range.All, cancellationToken);

            // Remove file from all datasets
            await foreach (var dataset in datasetList)
            {
                datasets.Add(dataset);
                await RemoveFileAsync(dataset, Descriptor.Path, cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;
            }

            // Reupload to dataset[0]
            var fileCreation = new FileCreation
            {
                Path = Descriptor.Path,
                Description = Description,
                Tags = Tags,
                Metadata = metadata
            };

            var newFile = await UploadFileAsync(datasets[0], fileCreation, sourceStream, progress, cancellationToken);

            // Link to remaining datasets
            var tasks = new List<Task>();
            for (var i = 1; i < datasets.Count; ++i)
            {
                var task = AddFileAsync(datasets[i], newFile, cancellationToken);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        static async Task RemoveFileAsync(IDataset dataset, string path, CancellationToken cancellationToken)
        {
            try
            {
                await dataset.RemoveFileAsync(path, cancellationToken);
                k_Logger.LogInformation($"{path} removed from {dataset.Name}.");
            }
            catch (OperationCanceledException)
            {
                k_Logger.LogWarning("File replacement cancelled.");
            }
            catch (AggregateException e)
            {
                k_Logger.LogError($"Failed to remove file reference from {dataset.Name}. {e.InnerException}");
            }
            catch (Exception e)
            {
                k_Logger.LogError($"Failed to remove file reference from {dataset.Name}. {e}");
            }
        }

        static async Task<IFile> UploadFileAsync(IDataset dataset, IFileCreation fileCreation, Stream memoryStream,
            IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            try
            {
                var newFile = await dataset.UploadFileAsync(fileCreation, memoryStream, progress, cancellationToken);
                k_Logger.LogInformation($"{newFile.Descriptor.Path} uploaded to {dataset.Name}.");
                return newFile;
            }
            catch (OperationCanceledException)
            {
                k_Logger.LogWarning("File replacement cancelled.");
            }
            catch (AggregateException e)
            {
                k_Logger.LogError($"Failed to upload file to {dataset.Name}. {e.InnerException}");
            }
            catch (Exception e)
            {
                k_Logger.LogError($"Failed to upload file to {dataset.Name}. {e}");
            }

            return null;
        }

        static async Task AddFileAsync(IDataset dataset, IFile file, CancellationToken cancellationToken)
        {
            try
            {
                await dataset.AddExistingFileAsync(file.Descriptor.Path, file.Descriptor.DatasetId, cancellationToken);
                k_Logger.LogInformation($"{file.Descriptor.Path} linked to {dataset.Name}.");
            }
            catch (OperationCanceledException)
            {
                k_Logger.LogWarning("File replacement cancelled.");
            }
            catch (AggregateException e)
            {
                k_Logger.LogError($"Failed to link file to {dataset.Name}. {e.InnerException}");
            }
            catch (Exception e)
            {
                k_Logger.LogError($"Failed to link file to {dataset.Name}. {e}");
            }
        }
    }
}
