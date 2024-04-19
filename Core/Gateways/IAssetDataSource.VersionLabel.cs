using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial interface IAssetDataSource
    {
        /// <summary>
        /// Lists the version labels for the specified organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="pagination">The range and order of results. </param>
        /// <param name="archived">Whether the results include archived labels. </param>
        /// <param name="systemLabels">Whether the results will include system labels. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>An async enumeration of <see cref="IVersionLabelData"/>. </returns>
        IAsyncEnumerable<IVersionLabelData> ListVersionLabelsAsync(OrganizationId organizationId, PaginationData pagination, bool? archived, bool? systemLabels, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the version label.
        /// </summary>
        /// <param name="versionLabelDescriptor">The object containing the necessary information to identify the version label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an <see cref="IVersionLabelData"/>. </returns>
        Task<IVersionLabelData> GetVersionLabelAsync(VersionLabelDescriptor versionLabelDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new version label.
        /// </summary>
        /// <param name="organizationId">The id of the organization. </param>
        /// <param name="versionLabelCreation">The object containing the necessary information to create a label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is an <see cref="IVersionLabelData"/>. </returns>
        Task<IVersionLabelData> CreateVersionLabelAsync(OrganizationId organizationId, IVersionLabelBaseData versionLabelCreation, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a version label.
        /// </summary>
        /// <param name="versionLabelDescriptor">The object containing the necessary information to identify the version label. </param>
        /// <param name="versionlabelUpdate">The object containing the information to update the label. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UpdateVersionLabelAsync(VersionLabelDescriptor versionLabelDescriptor, IVersionLabelBaseData versionlabelUpdate, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the status of a version label.
        /// </summary>
        /// <param name="versionLabelDescriptor">The object containing the necessary information to identify the version label. </param>
        /// <param name="archive">The status to update to. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UpdateVersionLabelStatusAsync(VersionLabelDescriptor versionLabelDescriptor, bool archive, CancellationToken cancellationToken);

        /// <summary>
        /// Lists the version labels for the asset by asset version.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="assetId">The id of the asset. </param>
        /// <param name="pagination">The range and order of results. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>An async enumeration of <see cref="AssetVersionLabelsDto"/>. </returns>
        IAsyncEnumerable<AssetVersionLabelsDto> ListLabelsAcrossAssetVersions(ProjectDescriptor projectDescriptor, AssetId assetId, PaginationData pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Assigns version labels to an asset.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="versionLabels">The collection of labels to assign. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task AssignVersionLabelsAsync(AssetDescriptor assetDescriptor, IEnumerable<string> versionLabels, CancellationToken cancellationToken);

        /// <summary>
        /// Unassigns version labels from an asset.
        /// </summary>
        /// <param name="assetDescriptor">The object containing the necessary information to identify the asset.</param>
        /// <param name="versionLabels">The collection of labels to remove. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result. </returns>
        Task UnassignVersionLabelsAsync(AssetDescriptor assetDescriptor, IEnumerable<string> versionLabels, CancellationToken cancellationToken);
    }
}
