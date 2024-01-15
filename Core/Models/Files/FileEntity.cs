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
            SystemMetadataEntity = new FileMetadataContainer(Descriptor, FileFields.systemMetadata, m_DataSource);
        }

        internal FileEntity(FileDescriptor fileDescriptor)
        {
            Descriptor = fileDescriptor;

            MetadataEntity = new FileMetadataContainer(Descriptor, FileFields.metadata, null);
            SystemMetadataEntity = new FileMetadataContainer(Descriptor, FileFields.systemMetadata, null);
        }

        /// <inheritdoc />
        public FileDescriptor Descriptor { get; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public Uri PreviewUrl { get; set; }

        /// <inheritdoc />
        public string Status { get; set; }

        /// <inheritdoc />
        public AuthoringInfo AuthoringInfo { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> SystemTags { get; set; }

        /// <summary>
        /// The user metadata of the asset.
        /// </summary>
        public IMetadataContainer Metadata => MetadataEntity;

        /// <summary>
        /// The system metadata of the asset.
        /// </summary>
        public IMetadataContainer SystemMetadata => SystemMetadataEntity;

        /// <inheritdoc />
        public IEnumerable<DatasetDescriptor> LinkedDatasets => m_LinkedDatasets;

        /// <summary>
        /// The metadata of the file.
        /// </summary>
        public MetadataContainerEntity MetadataEntity { get; }

        /// <summary>
        /// The system metadata of the file.
        /// </summary>
        public MetadataContainerEntity SystemMetadataEntity { get; }

        /// <inheritdoc />
        public long SizeBytes { get; set; }

        public string UserChecksum { get; set; }

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
                SystemMetadataEntity = { Properties = SystemMetadataEntity.Properties },
                SizeBytes = SizeBytes,
                UserChecksum = UserChecksum,
                UploadUrl = UploadUrl,
                DownloadUrl = DownloadUrl
            };
        }

        /// <inheritdoc />
        public async Task RefreshAsync(FileFields includeFields, CancellationToken cancellationToken)
        {
            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.files,
                DatasetFields = DatasetFields.none,
                FileFields = includeFields
            };

            var fileData = await m_DataSource.GetFileAsync(Descriptor, filter, cancellationToken);
            this.MapFrom(m_DataSource, fileData, includeFields);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IDataset> GetLinkedDatasetsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.datasets,
                DatasetFields = DatasetFields.none,
                FileFields = FileFields.none
            };

            var (start, length) = range.GetValidatedOffsetAndLength(m_LinkedDatasets.Length);
            for (var i = start; i < start + length; i++)
            {
                var dataset = await m_DataSource.GetDatasetAsync(m_LinkedDatasets[i], filter, cancellationToken);
                yield return dataset.From(m_DataSource, AssetDescriptor, DatasetFields.none);
            }
        }

        /// <inheritdoc />
        public void InvalidateCachedUrls()
        {
            DownloadUrl = null;
            UploadUrl = null;
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

        /// <inheritdoc />
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
        public async Task UploadAsync(Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken)
        {
            await GetUploadUrlAsync(cancellationToken);

            try
            {
                await m_DataSource.UploadContentAsync(UploadUrl, sourceStream, progress, cancellationToken);
            }
            catch (NotFoundException)
            {
                // If the upload fails, try to get a new upload url and try again.
                UploadUrl = null;
                await GetUploadUrlAsync(cancellationToken);
                await m_DataSource.UploadContentAsync(UploadUrl, sourceStream, progress, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task UpdateAsync(IFileUpdate fileUpdate, CancellationToken cancellationToken)
        {
            await m_DataSource.UpdateFileAsync(Descriptor, fileUpdate.From(), cancellationToken);
            await RefreshAsync(FileFields.all, default);
        }
    }
}
