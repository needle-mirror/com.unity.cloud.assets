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
    /// Interface to transform user facing data like <see cref="IProjectData"/> into service DTOs.
    /// </summary>
    class AssetRepository : IAssetRepository
    {
        readonly IAssetDataSource m_DataSource;

        internal AssetRepository(IAssetDataSource dataSource)
        {
            m_DataSource = dataSource;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetProject> ListAssetProjectsAsync(OrganizationId organizationId, Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var projectsEnumerator = m_DataSource.ListProjectsAsync(organizationId, pagination, cancellationToken).GetAsyncEnumerator(cancellationToken);
            while (await projectsEnumerator.MoveNextAsync())
            {
                yield return projectsEnumerator.Current.From(m_DataSource, organizationId);
            }
        }

        /// <inheritdoc />
        public async Task<IAssetProject> GetAssetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            var projectData = await m_DataSource.GetProjectAsync(projectDescriptor, cancellationToken);
            return projectData.From(m_DataSource, projectDescriptor);
        }

        /// <inheritdoc />
        public async Task<IAssetProject> CreateAssetProjectAsync(OrganizationId organizationId, IAssetProjectCreation projectCreation, CancellationToken cancellationToken)
        {
            var data = new ProjectBaseData
            {
                Name = projectCreation.Name,
                Metadata = projectCreation.Metadata
            };
            var projectData = await m_DataSource.CreateProjectAsync(organizationId, data, cancellationToken);
            return projectData.From(m_DataSource, organizationId);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IAssetCollection> ListAssetCollectionsAsync(ProjectDescriptor projectDescriptor, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var collectionDatas = await m_DataSource.ListCollectionsAsync(projectDescriptor, cancellationToken);
            foreach (var data in collectionDatas)
            {
                yield return data.From(m_DataSource, projectDescriptor);
            }
        }

        /// <inheritdoc />
        public async Task<IAssetCollection> GetAssetCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken)
        {
            var collectionData = await m_DataSource.GetCollectionAsync(collectionDescriptor, cancellationToken);
            return collectionData.From(m_DataSource, collectionDescriptor);
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<IAsset> SearchAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter, Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var availableProjects = projectIds as ProjectId[] ?? projectIds.ToArray();
            if (!availableProjects.Any())
                yield break;

            var assetEnumerator = m_DataSource.ListAssetsAsync(organizationId, availableProjects, assetSearchFilter, pagination, cancellationToken).GetAsyncEnumerator(cancellationToken);
            while (await assetEnumerator.MoveNextAsync())
            {
                yield return assetEnumerator.Current.From(m_DataSource, organizationId, availableProjects, assetSearchFilter.IncludedFields);
            }

            await assetEnumerator.DisposeAsync();
        }

        /// <inheritdoc />
        public Task<Aggregation> CountAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken cancellationToken)
        {
            return m_DataSource.GetAssetAggregateAsync(organizationId, projectIds, assetSearchFilter, parameters, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetAsync(AssetDescriptor assetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken)
        {
            var assetData = await m_DataSource.GetAssetAsync(assetDescriptor, includedFieldsFilter, cancellationToken);
            return assetData.From(m_DataSource, assetDescriptor, includedFieldsFilter);
        }

        /// <inheritdoc />
        public async Task<IDataset> GetDatasetAsync(DatasetDescriptor datasetDescriptor, DatasetFields includedFields, CancellationToken cancellationToken)
        {
            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.datasets,
                DatasetFields = includedFields,
                FileFields = FileFields.none
            };
            var datasetData = await m_DataSource.GetDatasetAsync(datasetDescriptor, filter, cancellationToken);
            return datasetData.From(m_DataSource, datasetDescriptor.AssetDescriptor, includedFields);
        }

        /// <inheritdoc />
        public async Task<IDataset> GetDatasetBySystemTagAsync(AssetDescriptor assetDescriptor, string systemTag, DatasetFields includedFields, CancellationToken cancellationToken)
        {
            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.datasets,
                DatasetFields = includedFields,
                FileFields = FileFields.none
            };
            var datasetData = await m_DataSource.GetDatasetBySystemTagAsync(assetDescriptor, systemTag, filter, cancellationToken);
            return datasetData.From(m_DataSource, assetDescriptor, includedFields);
        }

        /// <inheritdoc />
        public async Task<IFile> GetFileAsync(FileDescriptor fileDescriptor, FileFields includedFields, CancellationToken cancellationToken)
        {
            var filter = new FieldsFilter
            {
                AssetFields = AssetFields.files,
                DatasetFields = DatasetFields.none,
                FileFields = includedFields
            };
            var fileData = await m_DataSource.GetFileAsync(fileDescriptor, filter, cancellationToken);
            return fileData.From(m_DataSource, fileDescriptor, includedFields);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IFieldDefinition> ListFieldDefinitionsAsync(OrganizationId organizationId, Pagination pagination, bool includeDeleted, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var asyncEnumerator = m_DataSource.ListFieldDefinitionsAsync(organizationId, pagination, includeDeleted, cancellationToken).GetAsyncEnumerator(cancellationToken);
            while (await asyncEnumerator.MoveNextAsync())
            {
                yield return asyncEnumerator.Current.From(m_DataSource, organizationId);
            }
        }

        /// <inheritdoc />
        public async Task<IFieldDefinition> GetFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken)
        {
            var data = await m_DataSource.GetFieldDefinitionAsync(fieldDefinitionDescriptor, cancellationToken);
            return data.From(m_DataSource, fieldDefinitionDescriptor);
        }

        /// <inheritdoc />
        public async Task<IFieldDefinition> CreateFieldDefinitionAsync(OrganizationId organizationId, IFieldDefinitionCreation fieldDefinitionCreation, CancellationToken cancellationToken)
        {
            var data = await m_DataSource.CreateFieldDefinitionAsync(organizationId, fieldDefinitionCreation.From(), cancellationToken);
            return data.From(m_DataSource, organizationId);
        }

        /// <inheritdoc />
        public Task DeleteFieldDefinitionAsync(FieldDefinitionDescriptor fieldDefinitionDescriptor, CancellationToken cancellationToken)
        {
            return m_DataSource.DeleteFieldDefinitionAsync(fieldDefinitionDescriptor, cancellationToken);
        }

        /// <inheritdoc />
        public AssetDescriptor DeserializeAssetIdentifiers(string jsonSerialization)
        {
            var ids = IsolatedSerialization.DeserializeWithDefaultConverters<AssetIdentifier>(jsonSerialization);
            return ids.From();
        }

        /// <inheritdoc />
        public IAsset DeserializeAsset(string jsonSerialization)
        {
            if (jsonSerialization.Contains(AssetDataWithIdentifiers.SerializedType))
            {
                var data = IsolatedSerialization.DeserializeWithDefaultConverters<AssetDataWithIdentifiers>(jsonSerialization);
                return data.From(m_DataSource, FieldsFilter.All);
            }

            return null;
        }
    }
}
