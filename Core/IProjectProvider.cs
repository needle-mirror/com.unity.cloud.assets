using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface that provides all the methods to interact with an <see cref="IProject"/>.
    /// </summary>
    public interface IProjectProvider
    {
        /// <summary>
        /// Implement this method to get a list of <see cref="IProject"/> for an organization for current user.
        /// </summary>
        /// <param name="organization">The genesis id of the organization. </param>
        /// <param name="userId">ID of the user</param>
        /// <param name="pagination">An object containing the necessary information create an <see cref="IAssetPage"/>. </param>
        /// <param name="token">The cancellation token</param>
        /// <param name="enrichWithUsersCount">A flag indicating whether the projects should have user count. </param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <returns>A task whose result is an <see cref="IProjectPage"/>. </returns>
        Task<IProjectPage> GetProjectsByOrganizationAndUserIdsAsync(IOrganization organization, string userId, Pagination pagination, CancellationToken token, bool enrichWithUsersCount = false, string xCorrelationId = default);

        /// <summary>
        /// Implement this method to get a list of <see cref="IProject"/> for an organization for current user.
        /// </summary>
        /// <param name="organization">The genesis id of the organization. </param>
        /// <param name="pagination">The pagination parameters. </param>
        /// <param name="token">The cancellation token</param>
        /// <param name="enrichWithUsersCount">A flag indicating whether the projects should have user count. </param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <returns>A task whose result is an <see cref="IProjectPage"/>. </returns>
        Task<IProjectPage> GetCurrentUserProjectList(IOrganization organization, Pagination pagination, CancellationToken token, bool enrichWithUsersCount = false, string xCorrelationId = default);
    }
}
