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
    /// This is a class containing the information about an asset.
    /// </summary>
    sealed class Asset : IAsset
    {
        readonly IAssetDataSource m_DataSource;

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

        /// Not exposed in the interface.
        public IEnumerable<string> Labels { get; set; }

        /// <inheritdoc />
        public AssetType Type { get; set; } = AssetType.Other;

        /// <inheritdoc />
        public IMetadataContainer Metadata => MetadataEntity;

        /// <inheritdoc />
        public string PreviewFile { get; set; }

        /// <inheritdoc />
        public string Status { get; set; }

        /// Not exposed in the interface.
        public bool IsFrozen { get; set; }

        /// <inheritdoc />
        public AuthoringInfo AuthoringInfo { get; set; }

        internal Uri PreviewFileUrl { get; set; }
        internal DatasetEntity[] Datasets { get; set; }
        internal FileEntity[] Files { get; set; }
        internal MetadataContainerEntity MetadataEntity { get; }

        internal Asset(IAssetDataSource dataSource, AssetDescriptor assetDescriptor, ProjectId sourceProjectId, IEnumerable<ProjectId> linkedProjectIds)
        {
            m_DataSource = dataSource;
            Descriptor = assetDescriptor;
            if (linkedProjectIds != null)
            {
                m_LinkedProjects = linkedProjectIds.Select(projectId => new ProjectDescriptor(assetDescriptor.OrganizationId, projectId)).ToArray();
            }

            SourceProject = new ProjectDescriptor(assetDescriptor.OrganizationId, sourceProjectId);

            MetadataEntity = new AssetMetadataContainer(Descriptor, AssetFields.metadata, m_DataSource);
        }

        internal Asset(string id, string version = "1")
        {
            var projectDescriptor = new ProjectDescriptor(OrganizationId.None, ProjectId.None);
            Descriptor = new AssetDescriptor(projectDescriptor, new AssetId(id), new AssetVersion(version));

            MetadataEntity = new AssetMetadataContainer(Descriptor, AssetFields.metadata, null);
        }

        /// <inheritdoc />
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
                PreviewFile = PreviewFile,
                Status = Status,
                IsFrozen = IsFrozen,
                AuthoringInfo = AuthoringInfo,
                Datasets = Datasets?.ToArray(),
                Files = Files?.ToArray(),
                MetadataEntity = {Properties = MetadataEntity.Properties},
            };
        }

        /// <inheritdoc />
        public Task RefreshAsync(CancellationToken cancellationToken)
        {
            Datasets = null;
            Files = null;
            PreviewFileUrl = null;
            MetadataEntity.Refresh();

            return RefreshAsync(FieldsFilter.DefaultAssetIncludes, cancellationToken);
        }

        async Task RefreshAsync(FieldsFilter fieldsFilter, CancellationToken cancellationToken)
        {
            var assetData = await m_DataSource.GetAssetAsync(Descriptor, fieldsFilter, cancellationToken);
            this.MapFrom(m_DataSource, assetData, fieldsFilter);
        }

        /// <inheritdoc />
        public Task UpdateAsync(IAssetUpdate assetUpdate, CancellationToken cancellationToken)
        {
            return m_DataSource.UpdateAssetAsync(Descriptor, assetUpdate.From(), cancellationToken);
        }

        /// <inheritdoc />
        public Task UpdateStatusAsync(AssetStatusAction statusAction, CancellationToken cancellationToken)
        {
            return m_DataSource.UpdateAssetStatusAsync(Descriptor, statusAction, cancellationToken);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetProject> GetLinkedProjectsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var projectDescriptor in m_LinkedProjects)
            {
                var data = await m_DataSource.GetProjectAsync(projectDescriptor, cancellationToken);
                yield return data.From(m_DataSource, Descriptor.OrganizationId);
            }
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
            SourceProject = new ProjectDescriptor(Descriptor.OrganizationId, data.SourceProjectId);
            m_LinkedProjects = data.LinkedProjectIds?
                .Select(projectId => new ProjectDescriptor(Descriptor.OrganizationId, projectId))
                .ToArray() ?? Array.Empty<ProjectDescriptor>();
        }

        /// <inheritdoc />
        public async Task UnlinkFromProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            if (!m_LinkedProjects.Contains(projectDescriptor)) return;

            await m_DataSource.UnlinkAssetFromProjectAsync(Descriptor, projectDescriptor, cancellationToken);

            // If we are not unlinking from the current descriptor, we can fetch to refresh the linked projects.
            if (Descriptor.ProjectId != projectDescriptor.ProjectId)
            {
                var data = await m_DataSource.GetAssetAsync(Descriptor, FieldsFilter.None, cancellationToken);
                SourceProject = new ProjectDescriptor(Descriptor.OrganizationId, data.SourceProjectId);
                m_LinkedProjects = data.LinkedProjectIds?
                    .Select(projectId => new ProjectDescriptor(Descriptor.OrganizationId, projectId))
                    .ToArray() ?? Array.Empty<ProjectDescriptor>();
            }
            else // Otherwise, we remove the project from the linked projects. The descriptor path to this asset is no longer valid.
            {
                m_LinkedProjects = m_LinkedProjects.Where(descriptor => descriptor.ProjectId != projectDescriptor.ProjectId).ToArray();
            }
        }

        public async Task<Uri> GetPreviewUrlAsync(CancellationToken cancellationToken)
        {
            if (PreviewFileUrl == null)
            {
                var fieldsFilter = new FieldsFilter {AssetFields = AssetFields.previewFileUrl};
                var assetData = await m_DataSource.GetAssetAsync(Descriptor, fieldsFilter, cancellationToken);
                this.MapFrom(m_DataSource, assetData, fieldsFilter);
            }

            return PreviewFileUrl;
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
        public async IAsyncEnumerable<CollectionDescriptor> ListLinkedAssetCollectionsAsync(Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var enumerable = await m_DataSource.GetAssetCollectionsAsync(Descriptor, cancellationToken);

            var collectionDatas = enumerable?.ToArray() ?? Array.Empty<IAssetCollectionData>();
            if (collectionDatas.Length > 0)
            {
                var (start, length) = range.GetValidatedOffsetAndLength(collectionDatas.Length);
                for (var i = start; i < start + length; ++i)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return new CollectionDescriptor(Descriptor.ProjectDescriptor, collectionDatas[i].GetFullCollectionPath());
                }
            }
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

            var dataset = Datasets?.FirstOrDefault(x => x.Descriptor.DatasetId == datasetId);
            if (dataset == null)
            {
                throw new NotFoundException($"Dataset {datasetId} not found.");
            }

            return dataset;
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
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return Datasets[i];
                }
            }
        }

        /// <inheritdoc />
        public async Task<IFile> GetFileAsync(string filePath, CancellationToken cancellationToken)
        {
            if (Files == null) await RefreshFiles(cancellationToken);

            var file = Files?.FirstOrDefault(x => x.Descriptor.Path == filePath);
            if (file == null)
            {
                throw new NotFoundException($"File {filePath} not found.");
            }

            return file;
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
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return Files[i];
                }
            }
        }

        /// <inheritdoc />
        public string SerializeIdentifiers()
        {
            return IsolatedSerialization.SerializeWithDefaultConverters(GetIdentifier());
        }

        /// <inheritdoc />
        public string Serialize()
        {
            var data = new AssetDataWithIdentifiers
            {
                Identifier = GetIdentifier(),
                Data = this.From()
            };
            return IsolatedSerialization.SerializeWithDefaultConverters(data);
        }

        AssetIdentifier GetIdentifier()
        {
            return new AssetIdentifier
            {
                OrganizationId = Descriptor.OrganizationId,
                ProjectId = Descriptor.ProjectId,
                Id = Descriptor.AssetId,
                Version = Descriptor.AssetVersion
            };
        }

        async Task RefreshDatasets(CancellationToken cancellationToken)
        {
            var data = await m_DataSource.GetAssetAsync(Descriptor, FieldsFilter.DefaultDatasetIncludes, cancellationToken);

            this.MapFrom(m_DataSource, data, FieldsFilter.DefaultDatasetIncludes);
        }

        async Task RefreshFiles(CancellationToken cancellationToken)
        {
            var data = await m_DataSource.GetAssetAsync(Descriptor, FieldsFilter.DefaultFileIncludes, cancellationToken);

            this.MapFrom(m_DataSource, data, FieldsFilter.DefaultFileIncludes);
        }
    }
}
