using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Interface for an asset file data source.
    /// </summary>
    interface IAssetFileDataSource
    {
        /// <summary>
        /// Implement this method to create an asset file.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="asset">The asset the file will linked to.</param>
        /// <param name="assetFileCreation">The object containing the information necessary to create an asset file. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with whose result is the created <see cref="IAssetFile"/>. </returns>
        Task<IAssetFile> CreateAssetFileAsync(IOrganization organization, IProject project, IAsset asset, IAssetFileCreation assetFileCreation, CancellationToken token);

        /// <summary>
        /// Implement this method to finalize the upload of an asset file.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        Task FinalizeAssetFileUploadAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token);

        /// <summary>
        /// Implement this method to update an asset file.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the updated <see cref="IAssetFile"/></returns>
        Task<IAssetFile> UpdateAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token);

        /// <summary>
        /// Implement this method to delete an asset file.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        Task DeleteAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token);

        /// <summary>
        /// Implement this method to get an asset file url.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="urlType">The asset file's url type</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result is the url of the <paramref name="assetFile"/>'s id. </returns>
        Task<string> GetAssetFileUrlAsync(IOrganization organization, IProject project, IAssetFile assetFile, AssetFileUrlType urlType, CancellationToken token);

        /// <summary>
        /// Implement this method to upload an asset file.
        /// </summary>
        /// <param name="organization">The organization in which the <paramref name="project"/> resides. </param>
        /// <param name="project">The project in which the asset resides. It must exist within the <paramref name="organization"/>. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="contentStream">The stream to the file content</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The http response message</returns>
        Task<HttpResponseMessage> UploadAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, Stream contentStream, CancellationToken token);
    }
}
