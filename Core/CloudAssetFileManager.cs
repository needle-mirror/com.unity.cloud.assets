using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class that implements <see cref="IAssetFileManager"/> to provide asset file controller functionality.
    /// <remarks>Users of this class will require a minimum <c>Asset Manager Consumer</c> role.</remarks>
    /// </summary>
    public sealed class CloudAssetFileManager : IAssetFileManager
    {
        readonly IAssetFileDataSource m_AssetFileDataSource;

        /// <summary>
        /// Initializes and returns an instance of <see cref="CloudAssetFileManager"/>
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostResolver"> The <see cref="IServiceHostResolver"/> object. </param>
        public CloudAssetFileManager(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
            : this(serviceHttpClient, ServiceHostConfigurationFactory.Create(serviceHostResolver))
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudAssetFileManager"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"></param>
        /// <param name="serviceHostConfiguration"></param>
        CloudAssetFileManager(IServiceHttpClient serviceHttpClient, AssetHostConfiguration serviceHostConfiguration)
            : this(new AssetFileDataSource(serviceHttpClient, serviceHostConfiguration.GetServiceAddress())) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudAssetFileManager"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        internal CloudAssetFileManager(IAssetFileDataSource dataSource)
        {
            m_AssetFileDataSource = dataSource;
        }

        /// <inheritdoc />
        public Task<IAssetFile> CreateAssetFileAsync(IProject project, IAsset asset, IAssetFileCreation assetFileCreation, CancellationToken token)
        {
            return m_AssetFileDataSource.CreateAssetFileAsync(project, asset, assetFileCreation, token);
        }

        /// <inheritdoc />
        public Task FinalizeAssetFileUploadAsync(IProject project, IAssetFile assetFile, CancellationToken token)
        {
            return m_AssetFileDataSource.FinalizeAssetFileUploadAsync(project, assetFile, token);
        }

        /// <inheritdoc />
        public Task UpdateAssetFileAsync(IProject project, IAssetFile assetFile, CancellationToken token)
        {
            return m_AssetFileDataSource.UpdateAssetFileAsync(project, assetFile, token);
        }

        /// <inheritdoc />
        public Task DeleteAssetFileAsync(IProject project, IAssetFile assetFile, CancellationToken token)
        {
            return m_AssetFileDataSource.DeleteAssetFileAsync(project, assetFile, token);
        }

        /// <inheritdoc />
        public Task<string> GetAssetFileUrlAsync(IProject project, IAssetFile assetFile, AssetFileUrlType urlType, CancellationToken token)
        {
            if (!string.IsNullOrEmpty(assetFile.DownloadUrl) && urlType == AssetFileUrlType.Download)
            {
                return Task.FromResult(assetFile.DownloadUrl);
            }

            if (!string.IsNullOrEmpty(assetFile.UploadUrl) && urlType == AssetFileUrlType.Upload)
            {
                return Task.FromResult(assetFile.UploadUrl);
            }

            return m_AssetFileDataSource.GetAssetFileUrlAsync(project, assetFile, urlType, token);
        }

        /// <inheritdoc />
        public Task DownloadAssetFileAsync(IProject project, IAssetFile assetFile, Stream destinationStream, IProgress<HttpProgress> progress, CancellationToken token)
        {
            return m_AssetFileDataSource.DownloadAssetFileAsync(project, assetFile, destinationStream, progress, token);
        }

        /// <inheritdoc />
        [Obsolete("Use UploadAssetFileAsync(IProject project, IAssetFile assetFile, Stream contentStream, IProgress<HttpProgress> progress, CancellationToken token) instead.")]
        public Task<bool> UploadAssetFileAsync(IProject project, IAssetFile assetFile, Stream contentStream, CancellationToken token)
        {
            return m_AssetFileDataSource.UploadAssetFileAsync(project, assetFile, contentStream, token);
        }

        /// <inheritdoc />
        public Task<bool> UploadAssetFileAsync(IProject project, IAssetFile assetFile, Stream contentStream, IProgress<HttpProgress> progress, CancellationToken token)
        {
            return m_AssetFileDataSource.UploadAssetFileAsync(project, assetFile, contentStream, progress, token);
        }
    }
}
