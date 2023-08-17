using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Assets.Documentation.Scripting
{
public class AssetFileManagerExample
{
    IAssetFileManager m_AssetFileManager;

    void ConstructAssetFileManager()
    {
    #region AssetFileManagerConstruction

    var httpClient = new UnityHttpClient();
    var cloudConfiguration = UnityRuntimeServiceHostResolverFactory.Create();
    var playerSettings = UnityCloudPlayerSettings.Instance;
    var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

    var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, cloudConfiguration)
        .AddDefaultPkceAuthenticator(playerSettings)
        .Build();

    var authenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

    var serviceHttpClient = new ServiceHttpClient(httpClient, authenticator, playerSettings);

    m_AssetFileManager = new CloudAssetFileManager(serviceHttpClient, cloudConfiguration);

    #endregion
    }

    #region CreateAssetFile

    async Task CreateAssetFileAsync(IProject project, IAsset asset, IAssetFileCreation assetFileCreation, CancellationToken token)
    {
        await m_AssetFileManager.CreateAssetFileAsync(project, asset, assetFileCreation, token);
    }

    #endregion

    #region FinalizeAssetFileUpload

    async Task FinalizeAssetFileUploadAsync(IProject project, IAssetFile assetFile, CancellationToken token)
    {
        await m_AssetFileManager.FinalizeAssetFileUploadAsync(project, assetFile, token);
    }

    #endregion

    #region UpdateAssetFile

    async Task UpdateAssetFileAsync(IProject project, IAssetFile assetFile, CancellationToken token)
    {
        await m_AssetFileManager.UpdateAssetFileAsync(project, assetFile, token);
    }

    #endregion

    #region DeleteAssetFile

    async Task DeleteAssetFileAsync(IProject project, IAssetFile assetFile, CancellationToken token)
    {
        await m_AssetFileManager.DeleteAssetFileAsync(project, assetFile, token);
    }

    #endregion

    #region GetAssetFileUrl

    async Task<string> GetAssetFileUrlAsync(IProject project, IAssetFile assetFile, AssetFileUrlType urlType, CancellationToken token)
    {
        return await m_AssetFileManager.GetAssetFileUrlAsync(project, assetFile, urlType, token);
    }

    #endregion

    #region UploadAssetFile

    async Task<bool> UploadAssetFileAsync(IProject project, IAssetFile assetFile, Stream stream, CancellationToken token)
    {
        return await m_AssetFileManager.UploadAssetFileAsync(project, assetFile, stream, token);
    }

    #endregion
}
}
