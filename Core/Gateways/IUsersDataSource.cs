using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Implement this interface to transform between user facing data and service DTOs
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
    }
}
