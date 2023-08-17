using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Class for an asset file controller data source.
    /// </summary>
    class AssetFileDataSource : IAssetFileDataSource
    {
        const string k_BlobTypeHeaderKey = "X-Ms-Blob-Type";
        const string k_BlobTypeHeaderValue = "BlockBlob";
        readonly IAssetHttpClient m_Client;

        /// <summary>
        /// Creates a new instance of the <see cref="AssetFileDataSource"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"></param>
        /// <param name="serviceAddress"></param>
        internal AssetFileDataSource(IServiceHttpClient serviceHttpClient, string serviceAddress)
            : this(new AssetHttpClient(serviceHttpClient, serviceAddress))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AssetFileDataSource"/> class.
        /// </summary>
        /// <param name="client"></param>
        internal AssetFileDataSource(IAssetHttpClient client)
        {
            m_Client = client;
        }

        /// <inheritdoc />
        public async Task<IAssetFile> CreateAssetFileAsync(IProject project, IAsset asset, IAssetFileCreation assetFileCreation, CancellationToken token)
        {
            var assetFile = assetFileCreation.MapFrom();

            var request = new CreateAssetFileRequest(project.Organization.GenesisId, project.Id, asset.Id, asset.Version, assetFile);
            var response = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);

            var createdAssetFileDto = IsolatedJsonConvert.DeserializeObject<CreatedAssetFileDto>(response, IsolatedJsonConvert.jsonSerializerSettingsWithoutType);

            assetFile.StorageId = createdAssetFileDto.StorageId;
            assetFile.AssetId = createdAssetFileDto.AssetId;
            assetFile.AssetVersion = createdAssetFileDto.AssetVersion;
            assetFile.Id = createdAssetFileDto.FileId;
            assetFile.UploadUrl = createdAssetFileDto.UploadUrl;

            return assetFile;
        }

        /// <inheritdoc />
        public async Task FinalizeAssetFileUploadAsync(IProject project, IAssetFile assetFile, CancellationToken token)
        {
            var request = new FinalizeUploadAssetFileRequest(project.Organization.GenesisId, project.Id, assetFile.AssetId, assetFile.AssetVersion, assetFile.Id);
            _ = await m_Client.PostAsync(request, ServiceHttpClientOptions.NoRetryOption(), token);
        }

        /// <inheritdoc />
        public async Task<IAssetFile> UpdateAssetFileAsync(IProject project, IAssetFile assetFile, CancellationToken token)
        {
            var request = new UpdateAssetFileRequest(project.Organization.GenesisId, project.Id, assetFile.AssetId, assetFile.AssetVersion, assetFile.MapFrom());
            _ = await m_Client.PatchAsync(request, ServiceHttpClientOptions.Default(), token);

            return assetFile;
        }

        /// <inheritdoc />
        public async Task DeleteAssetFileAsync(IProject project, IAssetFile assetFile, CancellationToken token)
        {
            var request = new DeleteAssetFileRequest(project.Organization.GenesisId, project.Id, assetFile.AssetId, assetFile.AssetVersion, assetFile.Id);
            _ = await m_Client.DeleteAsync(request, ServiceHttpClientOptions.Default(), token);
        }

        /// <inheritdoc />
        public async Task<string> GetAssetFileUrlAsync(IProject project, IAssetFile assetFile, AssetFileUrlType urlType, CancellationToken token)
        {
            var request = new GetAssetFileUrlRequest(project.Organization.GenesisId, project.Id, assetFile.AssetId, assetFile.AssetVersion, assetFile.Id, urlType);
            var response = await m_Client.GetAsync(request, ServiceHttpClientOptions.Default(), token);

            var url = IsolatedJsonConvert.DeserializeObject<UrlDto>(response);

            switch (urlType)
            {
                case AssetFileUrlType.Download:
                    assetFile.DownloadUrl = url.Url;
                    break;
                case AssetFileUrlType.Upload:
                    assetFile.UploadUrl = url.Url;
                    break;
                case AssetFileUrlType.Delete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(urlType), urlType, null);
            }

            return response;
        }

        /// <inheritdoc />
        public async Task<bool> UploadAssetFileAsync(IProject project, IAssetFile assetFile, Stream contentStream, CancellationToken token)
        {
            var uploadUrl = assetFile.UploadUrl ?? await GetAssetFileUrlAsync(
                project,
                assetFile,
                AssetFileUrlType.Upload,
                token
            );

            if (string.IsNullOrEmpty(uploadUrl))
            {
                throw new InvalidUploadUrlException("Upload url is null or empty");
            }

            using var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(uploadUrl),
                Content = new StreamContent(contentStream)
            };

            httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            httpRequestMessage.Headers.Add(k_BlobTypeHeaderKey, k_BlobTypeHeaderValue);

            var httpClientOptions = new ServiceHttpClientOptions(true, false, false, false, retryPolicy: new NoRetryPolicy());

            var response = await m_Client.SendAsync(httpRequestMessage, httpClientOptions, token);

            var result = response.EnsureSuccessStatusCode();
            return result.IsSuccessStatusCode;
        }
    }
}
