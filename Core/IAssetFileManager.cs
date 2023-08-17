using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface that provides all the methods to interact with an <see cref="IAssetFile"/>.
    /// </summary>
    public interface IAssetFileManager
    {
        /// <summary>
        /// Implement this method to create an asset file.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="asset">The asset the file will linked to.</param>
        /// <param name="assetFileCreation">The object containing the information necessary to create an asset file. </param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetFileManagerExample.cs" region="CreateAssetFile"/>
        /// </example>
        Task<IAssetFile> CreateAssetFileAsync(IProject project, IAsset asset, IAssetFileCreation assetFileCreation, CancellationToken token);

        /// <summary>
        /// Implement this method to finalize the upload of an asset file.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetFileManagerExample.cs" region="FinalizeAssetFileUpload"/>
        /// </example>
        Task FinalizeAssetFileUploadAsync(IProject project, IAssetFile assetFile, CancellationToken token);

        /// <summary>
        /// Implement this method to update an asset file.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetFileManagerExample.cs" region="UpdateAssetFile"/>
        /// </example>
        Task UpdateAssetFileAsync(IProject project, IAssetFile assetFile, CancellationToken token);

        /// <summary>
        /// Implement this method to delete an asset file.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetFileManagerExample.cs" region="DeleteAssetFile"/>
        /// </example>
        Task DeleteAssetFileAsync(IProject project, IAssetFile assetFile, CancellationToken token);

        /// <summary>
        /// Implement this method to get an asset file url.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="urlType">The asset file's url type</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task with no result. </returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetFileManagerExample.cs" region="GetAssetFileUrl"/>
        /// </example>
        Task<string> GetAssetFileUrlAsync(IProject project, IAssetFile assetFile, AssetFileUrlType urlType, CancellationToken token);

        /// <summary>
        /// Implement this method to upload an asset file.
        /// </summary>
        /// <param name="project">The project in which the asset resides. </param>
        /// <param name="assetFile">The asset file</param>
        /// <param name="contentStream">The stream to the file content</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>A task whose result signals the success of the upload.</returns>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/AssetFileManagerExample.cs" region="UploadAssetFile"/>
        /// </example>
        Task<bool> UploadAssetFileAsync(IProject project, IAssetFile assetFile, Stream contentStream, CancellationToken token);
    }
}
