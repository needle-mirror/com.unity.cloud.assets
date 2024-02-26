using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface containing the information about an asset.
    /// </summary>
    public interface IAsset
    {
        /// <summary>
        /// The descriptor of the asset.
        /// </summary>
        AssetDescriptor Descriptor { get; }

        /// <summary>
        /// The source project of the asset.
        /// </summary>
        ProjectDescriptor SourceProject { get; }

        /// <summary>
        /// The list of projects the asset is linked to.
        /// </summary>
        IEnumerable<ProjectDescriptor> LinkedProjects { get; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        IEnumerable<string> SystemTags { get; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        /// Disabled until we have versioning support.
        // IEnumerable<string> Labels { get; }

        /// <summary>
        /// The type of the asset.
        /// </summary>
        AssetType Type { get; }

        /// <summary>
        /// The preview file ID of the asset.
        /// </summary>
        string PreviewFile { get; }

        /// <summary>
        /// The status of the asset.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Whether the asset is frozen.
        /// </summary>
        /// Disabled until we have versioning support.
        // bool IsFrozen { get; }

        /// <summary>
        /// The creation and update information of the asset.
        /// </summary>
        AuthoringInfo AuthoringInfo { get; }

        /// <summary>
        /// The searchable metadata of the asset.
        /// </summary>
        IMetadataContainer Metadata { get; }

        /// <summary>
        /// Returns an asset in the context of the specified project.
        /// </summary>
        /// <param name="projectDescriptor">The descriptor of the project. </param>
        /// <returns></returns>
        IAsset WithProject(ProjectDescriptor projectDescriptor);

        /// <summary>
        /// Refreshes the asset with the specified fields.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task RefreshAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asset.
        /// </summary>
        /// <param name="assetUpdate">The object containing the asset information to update. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task UpdateAsync(IAssetUpdate assetUpdate, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asset's status.
        /// </summary>
        /// <param name="statusAction">The new status of the asset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        Task UpdateStatusAsync(AssetStatusAction statusAction, CancellationToken cancellationToken);

        /// <summary>
        /// Returns an enumeration of the asset's linked <see cref="IAssetProject"/>.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IAssetProject"/>. </returns>
        IAsyncEnumerable<IAssetProject> GetLinkedProjectsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Creates a reference between an asset and the project.
        /// </summary>
        /// <param name="projectDescriptor">The descriptor of the project to link to. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../../Samples/Documentation/Scripting/AssetManagementExample.cs" region="LinkAssetToProject" title="Link Asset to Project"/>
        /// </example>
        Task LinkToProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the reference between an asset and the project.
        /// </summary>
        /// <param name="projectDescriptor">The descriptor of the project to unlink from. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../../Samples/Documentation/Scripting/AssetManagementExample.cs" region="UnlinkAssetFromProject" title="Unlink Asset from Project"/>
        /// </example>
        Task UnlinkFromProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the url to the preview image of the asset.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is downloadble url pointing to the preview image. </returns>
        Task<Uri> GetPreviewUrlAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the download URLs for the asset's files.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the download URLs for all the asset's files and attachments. </returns>
        Task<IDictionary<string, Uri>> GetAssetDownloadUrlsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns the <see cref="IAssetCollection"/> the asset belongs too.
        /// </summary>
        /// <param name="range">The range of collections to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="IAssetCollection"/>. </returns>
        IAsyncEnumerable<CollectionDescriptor> ListLinkedAssetCollectionsAsync(Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a <see cref="IDataset"/> with the specified creation information.
        /// </summary>
        /// <param name="datasetCreation">The object containing the necessary information to create a dataset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the newly created dataset. </returns>
        Task<IDataset> CreateDatasetAsync(DatasetCreation datasetCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the specified <see cref="IDataset"/>.
        /// </summary>
        /// <param name="datasetId">The id of the dataset. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is the requested dataset. </returns>
        Task<IDataset> GetDatasetAsync(DatasetId datasetId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all the <see cref="IDataset"/>.
        /// </summary>
        /// <param name="range">The range of datasets to return. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of datasets. </returns>
        IAsyncEnumerable<IDataset> ListDatasetsAsync(Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the specified <see cref="IFile"/>.
        /// </summary>
        /// <param name="filePath">The id of the file</param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an <see cref="IFile"/>. </returns>
        Task<IFile> GetFileAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all the <see cref="IFile"/>s for the asset.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of <see cref="IFile"/> referenced by the asset. </returns>
        IAsyncEnumerable<IFile> ListFilesAsync(Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a JSON serialized string of the asset's identifiers.
        /// </summary>
        /// <returns>The serialized identifiers of the asset. </returns>
        string SerializeIdentifiers();

        /// <summary>
        /// Returns a JSON serialized string of the asset.
        /// </summary>
        /// <returns>The serialized asset. </returns>
        string Serialize();
    }
}
