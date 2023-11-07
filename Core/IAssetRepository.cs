using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface that provides all the methods to interact with an <see cref="IProjectData"/>.
    /// </summary>
    public interface IAssetRepository
    {
        /// <summary>
        /// Lists an organization's <see cref="IAssetProject"/> for current user.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="pagination">The pagination parameters. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IAssetProject"/>. </returns>
        IAsyncEnumerable<IAssetProject> ListAssetProjectsAsync(OrganizationId organizationId, Pagination pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an organization's <see cref="IAssetProject"/> for current user.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information for identifying the project. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an <see cref="IAssetProject"/>. </returns>
        Task<IAssetProject> GetAssetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new <see cref="IAssetProject"/> in the specified organization.
        /// </summary>
        /// <param name="organizationId">The organization to create the project in. </param>
        /// <param name="projectCreation">The object containing the necessary information to create a new project. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is the new <see cref="IAssetProject"/>. </returns>
        Task<IAssetProject> CreateAssetProjectAsync(OrganizationId organizationId, IAssetProjectCreation projectCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Lists a project's <see cref="IAssetCollection"/>.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information for identifying the project. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IAssetCollection"/>. </returns>
        IAsyncEnumerable<IAssetCollection> ListAssetCollectionsAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an <see cref="IAssetCollection"/>.
        /// </summary>
        /// <param name="collectionDescriptor">The object containing the necessary information for identifying the collection. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an <see cref="IAssetCollection"/></returns>
        Task<IAssetCollection> GetAssetCollectionAsync(CollectionDescriptor collectionDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Lists an organization's <see cref="Asset"/> for current user.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="projectIds">A list of project ids. </param>
        /// <param name="assetSearchFilter">The search filter. </param>
        /// <param name="pagination">The pagination parameters. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="Asset"/>. </returns>
        IAsyncEnumerable<IAsset> SearchAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter, Pagination pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Lists an organization's <see cref="Asset"/> for a user.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="projectIds">A list of project ids. </param>
        /// <param name="assetSearchFilter">The search filter. </param>
        /// <param name="parameters">The aggregation parameters. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an aggregation. </returns>
        Task<Aggregation> CountAssetsAsync(OrganizationId organizationId, IEnumerable<ProjectId> projectIds, IAssetSearchFilter assetSearchFilter, AggregationParameters parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an <see cref="IAsset"/> by its id and version.
        /// </summary>
        /// <param name="assetDescriptor">The descriptor containing identifiers for the asset. </param>
        /// <param name="includedFieldsFilter">The filter describing which fields to return populated. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an <see cref="IAsset"/>. </returns>
        Task<IAsset> GetAssetAsync(AssetDescriptor assetDescriptor, FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an <see cref="IDataset"/> from a specified asset version.
        /// </summary>
        /// <param name="datasetDescriptor">The descriptor containing identifiers for the dataset. </param>
        /// <param name="includedFields">The filter describing which fields to return populated. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is a <see cref="IDataset"/>. </returns>
        Task<IDataset> GetDatasetAsync(DatasetDescriptor datasetDescriptor, DatasetFields includedFields, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an <see cref="IDataset"/> with a specified tag from a specified asset version.
        /// </summary>
        /// <param name="assetDescriptor">The descriptor containing identifiers for the dataset. </param>
        /// <param name="systemTag">The id of the dataset to get. </param>
        /// <param name="includedFields">The filter describing which fields to return populated. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is a <see cref="IDataset"/>. </returns>
        Task<IDataset> GetDatasetBySystemTagAsync(AssetDescriptor assetDescriptor, string systemTag, DatasetFields includedFields, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an <see cref="IFile"/> from a specified dataset.
        /// </summary>
        /// <param name="fileDescriptor">The descriptor containing identifiers for the file. </param>
        /// <param name="includedFields">The filter describing which fields to return populated. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is a <see cref="IFile"/>. </returns>
        Task<IFile> GetFileAsync(FileDescriptor fileDescriptor, FileFields includedFields, CancellationToken cancellationToken);

        /// <summary>
        /// Implement this method to get an <see cref="AssetDescriptor"/> given a serialized json of asset identifiers.
        /// </summary>
        /// <param name="jsonSerialization">The serialization of an asset's identifiers. Accepts the result of <see cref="IAsset.SerializeIdentifiers"/>. </param>
        /// <returns>An <see cref="AssetDescriptor"/>. </returns>
        AssetDescriptor DeserializeAssetIdentifiers(string jsonSerialization);

        /// <summary>
        /// Retrieves an <see cref="IAsset"/> with a serialized JSON.
        /// </summary>
        /// <param name="jsonSerialization">The serialization of an asset. Accepts the result of <see cref="IAsset.Serialize"/>. </param>
        /// <returns>An <see cref="IAsset"/>. </returns>
        IAsset DeserializeAsset(string jsonSerialization);
    }
}
