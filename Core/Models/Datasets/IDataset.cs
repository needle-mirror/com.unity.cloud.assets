using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public interface IDataset
    {
        /// <summary>
        /// The descriptor of the dataset.
        /// </summary>
        DatasetDescriptor Descriptor { get; }

        /// <summary>
        /// The name of the dataset.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the dataset.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The user tags of the dataset.
        /// </summary>
        IEnumerable<string> Tags { get; }

        /// <summary>
        /// The system tags of the dataset.
        /// </summary>
        IEnumerable<string> SystemTags { get; }

        /// <summary>
        /// The status of the dataset.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// The authoring info of the dataset.
        /// </summary>
        AuthoringInfo AuthoringInfo { get; }

        /// <summary>
        /// The portal metadata of the dataset.
        /// </summary>
        IDeserializable PortalMetadata { get; }

        /// <summary>
        /// The user metadata of the dataset.
        /// </summary>
        IDeserializable Metadata { get; }

        /// <summary>
        /// The system metadata of the dataset.
        /// </summary>
        IDeserializable SystemMetadata { get; }

        /// <summary>
        /// The order of the files in the dataset.
        /// </summary>
        IEnumerable<string> FileOrder { get; }

        /// <summary>
        /// Indicates whether the dataset is visible or not.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Refreshes the dataset with the specified fields.
        /// </summary>
        /// <param name="includeFields">The fields to refresh. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        // Task RefreshAsync(DatasetFields includeFields, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the asset that this dataset is associated with.
        /// </summary>
        /// <param name="includedFieldsFilter">The filter describing which fields to return populated. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is the enclosing asset. </returns>
        Task<IAsset> GetAssetAsync(FieldsFilter includedFieldsFilter, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the dataset.
        /// </summary>
        /// <param name="datasetUpdate">The object containing the necessary information to update the dataset. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task UpdateAsync(IDatasetUpdate datasetUpdate, CancellationToken cancellationToken);

        /// <summary>
        /// Creates and uploads a new file to the dataset.
        /// </summary>
        /// <param name="fileCreation">The object containing the necessary information to create a new file. </param>
        /// <param name="sourceStream">The stream from which to uplaod the new file. </param>
        /// <param name="progress">The progress of the upload. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is a newly created file. </returns>
        Task<IFile> UploadFileAsync(IFileCreation fileCreation, Stream sourceStream, IProgress<HttpProgress> progress, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a file from the specified dataset to the current dataset.
        /// </summary>
        /// <param name="filePath">The path to the file. </param>
        /// <param name="sourceDatasetId">The id of the source dataset.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task whose result is the linked file. </returns>
        Task<IFile> AddExistingFileAsync(string filePath, DatasetId sourceDatasetId, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a file from the dataset.
        /// </summary>
        /// <param name="filePath">The path to the file. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task RemoveFileAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the files in the dataset.
        /// </summary>
        /// <param name="range">The range of files to return. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is an async enumeration of file. </returns>
        IAsyncEnumerable<IFile> ListFilesAsync(Range range, CancellationToken cancellationToken);

        /// <summary>
        /// Returns a file in the dataset.
        /// </summary>
        /// <param name="filePath">The path to the file. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task whose result is the file at <paramref name="filePath"/>. </returns>
        Task<IFile> GetFileAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified user metadata fields from the dataset.
        /// </summary>
        /// <param name="keys">The metadata files to remove. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task RemoveUserMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified system metadata fields from the dataset.
        /// </summary>
        /// <param name="keys">The metadata files to remove. </param>
        /// <param name="cancellationToken">The cancellation token. </param>
        /// <returns>A task with no result. </returns>
        Task RemoveSystemMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Returns the download URL of the file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Uri GetFileUrl(string filePath);
    }
}
