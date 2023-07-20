using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Implement this interface to transform user facing data like <see cref="IOrganization"/> and <see cref="IAssetPage"/> into service DTOs
    /// </summary>
    interface IUsersDataSource
    {
        /// <summary>
        /// Implement this method to return the collection of <see cref="IOrganization"/> the current user belongs to.
        /// </summary>
        /// <param name="userId">ID of the user, can be null to get the current user. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is a tuple of the <see cref="IUser"/> and a collection of its available <see cref="IOrganization"/>.</returns>
        Task<(IUser user, IOrganization[] organizations)> GetUserOrganizationsAsync(string userId, CancellationToken token);

        /// <summary>
        /// Implement this method to get a list of <see cref="IProject"/> for an organization for current user.
        /// </summary>
        /// <param name="organization">The organization. </param>
        /// <param name="userId">ID of the user, can be null to get the current user. </param>
        /// <param name="pagination">An object containing the necessary information create an <see cref="IAssetPage"/>. </param>
        /// <param name="token"></param>
        /// <param name="enrichWithUsersCount">A flag indicating whether the projects should have user count. </param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        /// <returns>A task whose result is an <see cref="IProjectPage"/>. </returns>
        Task<IProjectPage> ListProjectsAsync(IOrganization organization, string userId, Pagination pagination, CancellationToken token, bool enrichWithUsersCount = false, string xCorrelationId = default);
    }
}
