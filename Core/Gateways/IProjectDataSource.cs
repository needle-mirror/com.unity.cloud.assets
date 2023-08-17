using System.Collections.Generic;
using System.Threading;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Implement this interface to transform between user facing data and service DTOs
    /// </summary>
    interface IProjectDataSource
    {
        /// <summary>
        /// Implement this method to get a list of <see cref="IProject"/> for an organization for current user.
        /// </summary>
        /// <param name="organization">The organization. </param>
        /// <param name="userId">ID of the user, can be null to get the current user. </param>
        /// <param name="pagination">An object containing the necessary information return a range of <see cref="IProject"/>. </param>
        /// <param name="cancellationToken"></param>
        /// <param name="enrichWithUsersCount">A flag indicating whether the projects should have user count. </param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <returns>A task whose result is an async enumeration of <see cref="IProject"/>. </returns>
        IAsyncEnumerable<IProject> ListProjectsAsync(IOrganization organization, string userId, Pagination pagination, CancellationToken cancellationToken, bool enrichWithUsersCount = false, string xCorrelationId = default);
    }
}
