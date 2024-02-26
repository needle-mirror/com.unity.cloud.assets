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
        /// Retrieves a list of <see cref="IProjectData"/> for an organization for the current user.
        /// </summary>
        /// <param name="organizationId">The organization id. </param>
        /// <param name="pagination">An object containing the necessary information return a range of projects. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is an async enumeration of projects. </returns>
        IAsyncEnumerable<IProjectData> ListProjectsAsync(OrganizationId organizationId, PaginationData pagination, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an <see cref="IProjectData"/> for an organization.
        /// </summary>
        /// <param name="projectDescriptor">The object containing the necessary information to identify the project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a project. </returns>
        Task<IProjectData> GetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new project in an organization.
        /// </summary>
        /// <param name="organizationId">The organization id. </param>
        /// <param name="projectCreation">The object containing the necessary information to create a project. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task whose result is a newly created project. </returns>
        Task<IProjectData> CreateProjectAsync(OrganizationId organizationId, IProjectBaseData projectCreation, CancellationToken cancellationToken);
    }
}
