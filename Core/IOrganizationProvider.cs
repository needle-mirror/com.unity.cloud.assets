using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface that provdes the methods to fetch user's <see cref="IOrganization"/>.
    /// </summary>
    public interface IOrganizationProvider
    {
        /// <summary>
        /// Implement this method to get the collection of <see cref="IOrganization"/> the current user belongs to.
        /// </summary>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an array of <see cref="IOrganization"/>.</returns>
        Task<IOrganization[]> GetOrganizationsAsync(CancellationToken token);
    }
}
