using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    public interface IAssetPage : IPagedResponse<IAsset>
    {
        /// <summary>
        /// Implement this property to return the organization id of the page.
        /// </summary>
        IOrganization Organization { get; }

        /// <summary>
        /// Implement this property to return the project id of the page.
        /// </summary>
        IProject Project { get; }

        /// <summary>
        /// Implement this method to return the next set of assets.
        /// </summary>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is an <see cref="IAssetPage"/>. </returns>
        Task<IAssetPage> GetNextAsync<TAsset>(CancellationToken token) where TAsset : IAsset, new();
    }
}
