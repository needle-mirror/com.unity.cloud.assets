using System;
using System.IO;
using System.Net.Http;
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
        public Task<IAssetFile> CreateAssetFileAsync(IOrganization organization, IProject project, IAsset asset, IAssetFileCreation assetFileCreation, CancellationToken token)
        {
            return m_AssetFileDataSource.CreateAssetFileAsync(organization, project, asset, assetFileCreation, token);
        }

        /// <inheritdoc />
        public Task FinalizeAssetFileUploadAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token)
        {
            return m_AssetFileDataSource.FinalizeAssetFileUploadAsync(organization, project, assetFile, token);
        }

        /// <inheritdoc />
        public Task UpdateAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token)
        {
            return m_AssetFileDataSource.UpdateAssetFileAsync(organization, project, assetFile, token);
        }

        /// <inheritdoc />
        public Task DeleteAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token)
        {
            return m_AssetFileDataSource.DeleteAssetFileAsync(organization, project, assetFile, token);
        }

        /// <inheritdoc />
        public Task<string> GetAssetFileUrlAsync(IOrganization organization, IProject project, IAssetFile assetFile, AssetFileUrlType urlType, CancellationToken token)
        {
            if (!string.IsNullOrEmpty(assetFile.DownloadUrl) && urlType == AssetFileUrlType.Download)
            {
                return Task.FromResult(assetFile.DownloadUrl);
            }

            if (!string.IsNullOrEmpty(assetFile.UploadUrl) && urlType == AssetFileUrlType.Upload)
            {
                return Task.FromResult(assetFile.UploadUrl);
            }

            return m_AssetFileDataSource.GetAssetFileUrlAsync(organization, project, assetFile, urlType, token);
        }

        /// <inheritdoc />
        public async Task<bool> UploadAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, Stream contentStream, CancellationToken token)
        {
            var response = await m_AssetFileDataSource.UploadAssetFileAsync(organization, project, assetFile, contentStream, token);

            var result = response.EnsureSuccessStatusCode();
            return result.IsSuccessStatusCode;
        }
    }
}
