using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This is a base class containing the information about an asset.
    /// </summary>
    sealed class Asset : IAsset
    {
        static readonly FieldsFilter k_BasicFields = new()
        {
            AssetFields = AssetFields.none,
            DatasetFields = DatasetFields.none,
            FileFields = FileFields.none
        };

        readonly IAssetDataSource m_DataSource;

        IEnumerable<IAssetCollection> m_Collections = Array.Empty<IAssetCollection>();
        internal ProjectDescriptor[] m_LinkedProjects = Array.Empty<ProjectDescriptor>();

        /// <inheritdoc />
        public AssetDescriptor Descriptor { get; }

        /// <inheritdoc />
        public ProjectDescriptor SourceProject { get; private set; }

        /// <inheritdoc />
        public IEnumerable<ProjectDescriptor> LinkedProjects => m_LinkedProjects;

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public string Description { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> SystemTags { get; set; }

        /// <inheritdoc />
        public IEnumerable<string> Labels { get; set; }

        /// <inheritdoc />
        public AssetType Type { get; set; }

        /// <inheritdoc />
        public IDeserializable PortalMetadata { get; set; }

        /// <inheritdoc />
        public IDeserializable Metadata { get; set; }

        /// <inheritdoc />
        public IDeserializable SystemMetadata { get; set; }

        /// <inheritdoc />
        public string PreviewFile { get; set; }

        /// <inheritdoc />
        public Uri PreviewFileUrl { get; set; }

        /// <inheritdoc />
        public IEnumerable<CollectionPath> Collections { get; private set; } = Array.Empty<CollectionPath>();

        /// <inheritdoc />
        public string Status { get; set; }

        /// <inheritdoc />
        public bool IsFrozen { get; set; }

        /// <inheritdoc />
        public AuthoringInfo AuthoringInfo { get; set; }

        /// <inheritdoc />
        public string StorageId { get; set; }

        internal DatasetEntity[] Datasets { get; set; }
        internal FileEntity[] Files { get; set; }

        internal Asset(IAssetDataSource dataSource, AssetDescriptor assetDescriptor, ProjectId sourceProjectId, IEnumerable<ProjectId> linkedProjectIds)
        {
            m_DataSource = dataSource;
            Descriptor = assetDescriptor;
            if (linkedProjectIds != null)
            {
                m_LinkedProjects = linkedProjectIds.Select(projectId => new ProjectDescriptor(assetDescriptor.OrganizationGenesisId, projectId)).ToArray();
            }
            SourceProject = new ProjectDescriptor(assetDescriptor.OrganizationGenesisId, sourceProjectId);
        }

        internal Asset(string id, int version = 1)
        {
            var projectDescriptor = new ProjectDescriptor(OrganizationId.None, ProjectId.None);
            Descriptor = new AssetDescriptor(projectDescriptor, new AssetId(id), new AssetVersion(version));
        }

        public IAsset WithProject(ProjectDescriptor projectDescriptor)
        {
            if (projectDescriptor == Descriptor.ProjectDescriptor) return this;

            if (!m_LinkedProjects.Contains(projectDescriptor))
                throw new InvalidArgumentException("The asset does not belong to the specified project.");

            var linkedProjectIds = m_LinkedProjects.Select(x => x.ProjectId);
            return new Asset(m_DataSource, new AssetDescriptor(projectDescriptor, Descriptor.AssetId, Descriptor.AssetVersion), SourceProject.ProjectId, linkedProjectIds)
            {
                Name = Name,
                Description = Description,
                Tags = Tags?.ToArray(),
                SystemTags = SystemTags?.ToArray(),
                Labels = Labels?.ToArray(),
                Type = Type,
                PortalMetadata = PortalMetadata, // Find a better way of copying IDeserializable
                Metadata = Metadata, // Find a better way of copying IDeserializable
                SystemMetadata = SystemMetadata, // Find a better way of copying IDeserializable
                PreviewFile = PreviewFile,
                Collections = Collections?.ToArray(),
                Status = Status,
                IsFrozen = IsFrozen,
                AuthoringInfo = AuthoringInfo,
                StorageId = StorageId,
                Datasets = Datasets?.ToArray(),
                Files = Files?.ToArray(),
            };
        }

        /// <inheritdoc />
        public async Task RefreshAsync(FieldsFilter includeFields, CancellationToken cancellationToken)
        {
            var assetData = await m_DataSource.GetAssetAsync(Descriptor, includeFields, cancellationToken);
            this.MapFrom(m_DataSource, assetData, includeFields);
        }

        /// <inheritdoc />
        public Task UpdateAsync(IAssetUpdate assetUpdate, CancellationToken cancellationToken)
        {
            var data = new AssetUpdateData
            {
                Name = assetUpdate.Name,
                Description = assetUpdate.Description,
                Tags = assetUpdate.Tags,
                Type = assetUpdate.Type,
                PreviewFile = assetUpdate.PreviewFile,
                PortalMetadata = assetUpdate.PortalMetadata,
                Metadata = assetUpdate.Metadata,
                SystemMetadata = assetUpdate.SystemMetadata
            };
            return m_DataSource.UpdateAssetAsync(Descriptor, data, cancellationToken);
        }

        /// <inheritdoc />
        public async Task LinkToProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            if (m_LinkedProjects.Contains(projectDescriptor)) return;

            await m_DataSource.LinkAssetToProjectAsync(Descriptor, projectDescriptor, cancellationToken);

            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.none,
                DatasetFields = DatasetFields.none,
                FileFields = FileFields.none
            };
            var data = await m_DataSource.GetAssetAsync(Descriptor, filter, cancellationToken);
            SourceProject = new ProjectDescriptor(Descriptor.OrganizationGenesisId, data.SourceProjectId);
            m_LinkedProjects = data.LinkedProjectIds?
                .Select(projectId => new ProjectDescriptor(Descriptor.OrganizationGenesisId, projectId))
                .ToArray() ?? Array.Empty<ProjectDescriptor>();
        }

        /// <inheritdoc />
        public async Task UnlinkFromProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            if (!m_LinkedProjects.Contains(projectDescriptor)) return;

            await m_DataSource.UnlinkAssetFromProjectAsync(Descriptor, projectDescriptor, cancellationToken);

            if (Descriptor.ProjectId != projectDescriptor.ProjectId)
            {
                var filter = new FieldsFilter
                {
                    AssetFields = AssetFields.none,
                    DatasetFields = DatasetFields.none,
                    FileFields = FileFields.none
                };
                var data = await m_DataSource.GetAssetAsync(Descriptor, filter, cancellationToken);
                SourceProject = new ProjectDescriptor(Descriptor.OrganizationGenesisId, data.SourceProjectId);
                m_LinkedProjects = data.LinkedProjectIds?
                    .Select(projectId => new ProjectDescriptor(Descriptor.OrganizationGenesisId, projectId))
                    .ToArray() ?? Array.Empty<ProjectDescriptor>();
            }
            else
            {
                m_LinkedProjects = m_LinkedProjects.Where(descriptor => descriptor.ProjectId != projectDescriptor.ProjectId).ToArray();
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetProject> GetLinkedProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var projectDescriptor in m_LinkedProjects)
            {
                var data = await m_DataSource.GetProjectAsync(projectDescriptor, cancellationToken);
                yield return data.From(m_DataSource, Descriptor.ProjectDescriptor);
            }
        }

        /// <inheritdoc />
        public async Task<IDictionary<string, Uri>> GetAssetDownloadUrlsAsync(CancellationToken cancellationToken)
        {
            var fileUrls = await m_DataSource.GetAssetDownloadUrlsAsync(Descriptor, cancellationToken);

            var urls = new Dictionary<string, Uri>();
            foreach (var url in fileUrls)
            {
                urls.Add(url.FilePath, url.DownloadUrl);

                var file = Files?.FirstOrDefault(f => f.Descriptor.Path == url.FilePath);
                if (file != null)
                {
                    file.DownloadUrl = url.DownloadUrl;
                }
            }

            return urls;
        }

        /// <inheritdoc />
        public async Task RefreshAssetCollectionsAsync(CancellationToken cancellationToken)
        {
            var collectionDatas = await m_DataSource.GetAssetCollectionsAsync(Descriptor, cancellationToken);
            m_Collections = collectionDatas.Select(data => data.From(m_DataSource, Descriptor.ProjectDescriptor));
            Collections = m_Collections.Select(c => (CollectionPath) c.GetFullCollectionPath());
        }

        /// <inheritdoc />
        public async Task<IAssetCollection> GetCollectionAsync(CollectionPath collectionPath, CancellationToken cancellationToken)
        {
            var collection = m_Collections.FirstOrDefault(x => x.GetFullCollectionPath() == collectionPath);

            // Try to refresh if not found before returning null.
            if (collection == null)
            {
                await RefreshAssetCollectionsAsync(cancellationToken);
                collection = m_Collections.FirstOrDefault(x => x.GetFullCollectionPath() == collectionPath);
            }

            return collection;
        }

        /// <inheritdoc />
        public async Task<IDataset> CreateDatasetAsync(DatasetCreation datasetCreation, CancellationToken cancellationToken)
        {
            var datasetData = await m_DataSource.CreateDatasetAsync(Descriptor, datasetCreation.From(), cancellationToken);
            var dataset = datasetData.From(m_DataSource, Descriptor, DatasetFields.all);

            // Clear datasets to force a refresh the next time they are accessed.
            Datasets = null;

            return dataset;
        }

        /// <inheritdoc />
        public async Task<IDataset> GetDatasetAsync(DatasetId datasetId, CancellationToken cancellationToken)
        {
            if (Datasets == null) await RefreshDatasets(cancellationToken);

            return Datasets?.FirstOrDefault(x => x.Descriptor.DatasetId == datasetId);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IDataset> ListDatasetsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (Datasets == null) await RefreshDatasets(cancellationToken);

            if (Datasets != null)
            {
                var (start, length) = range.GetValidatedOffsetAndLength(Datasets.ToArray().Length);
                for (var i = start; i < start + length; ++i)
                {
                    yield return await Task.FromResult(Datasets[i]);
                }
            }
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

            if (Files != null)
            {
                var (start, length) = range.GetValidatedOffsetAndLength(Files.Length);
                for (var i = start; i < start + length; ++i)
                {
                    yield return await Task.FromResult(Files[i]);
                }
            }
        }

        /// <inheritdoc />
        public async Task RemoveUserMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            await m_DataSource.RemoveAssetMetadataAsync(Descriptor, "metadata", keys, cancellationToken);

            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.metadata,
                DatasetFields = DatasetFields.none,
                FileFields = FileFields.none
            };
            await RefreshAsync(filter, cancellationToken);
        }

        /// <inheritdoc />
        public async Task RemoveSystemMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            await m_DataSource.RemoveAssetMetadataAsync(Descriptor, "systemMetadata", keys, cancellationToken);

            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.systemMetadata,
                DatasetFields = DatasetFields.none,
                FileFields = FileFields.none
            };
            await RefreshAsync(filter, cancellationToken);
        }

        /// <inheritdoc />
        public async Task PublishAsync(CancellationToken cancellationToken)
        {
            await m_DataSource.PublishApprovedAssetAsync(Descriptor, cancellationToken);
            await RefreshAsync(k_BasicFields, cancellationToken);
        }

        /// <inheritdoc />
        public async Task WithdrawAsync(CancellationToken cancellationToken)
        {
            await m_DataSource.WithdrawPublishedAssetAsync(Descriptor, cancellationToken);
            await RefreshAsync(k_BasicFields, cancellationToken);
        }

        /// <inheritdoc />
        public async Task SendToReviewAsync(CancellationToken cancellationToken)
        {
            await m_DataSource.SendAssetToReviewAsync(Descriptor, cancellationToken);
            await RefreshAsync(k_BasicFields, cancellationToken);
        }

        /// <inheritdoc />
        public async Task ApproveAsync(CancellationToken cancellationToken)
        {
            await m_DataSource.ApproveAssetAsync(Descriptor, cancellationToken);
            await RefreshAsync(k_BasicFields, cancellationToken);
        }

        /// <inheritdoc />
        public async Task RejectAsync(CancellationToken cancellationToken)
        {
            await m_DataSource.RejectAssetAsync(Descriptor, cancellationToken);
            await RefreshAsync(k_BasicFields, cancellationToken);
        }

        /// <inheritdoc />
        public string SerializeIdentifiers()
        {
            return IsolatedJsonConvert.SerializeObject(GetIdentifier(), SerializationUtilities.Converters);
        }

        /// <inheritdoc />
        public string Serialize()
        {
            var data = new AssetDataWithIdentifiers
            {
                Identifier = GetIdentifier(),
                Data = this.From()
            };
            return IsolatedJsonConvert.SerializeObject(data, SerializationUtilities.Converters);
        }

        AssetIdentifier GetIdentifier()
        {
            return new AssetIdentifier
            {
                OrganizationId = Descriptor.OrganizationGenesisId,
                ProjectId = Descriptor.ProjectId,
                Id = Descriptor.AssetId,
                Version = Descriptor.AssetVersion
            };
        }

        async Task RefreshDatasets(CancellationToken cancellationToken)
        {
            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.datasets,
                DatasetFields = DatasetFields.all,
                FileFields = FileFields.none
            };

            var data = await m_DataSource.GetAssetAsync(Descriptor, filter, cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;

            this.MapFrom(m_DataSource, data, filter);
        }

        async Task RefreshFiles(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.files,
                DatasetFields = DatasetFields.none,
                FileFields = FileFields.all
            };

            var data = await m_DataSource.GetAssetAsync(Descriptor, filter, cancellationToken);

            if (cancellationToken.IsCancellationRequested) return;

            this.MapFrom(m_DataSource, data, filter);
        }
    }
}
