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
    var cloudConfiguration = UnityRuntimeServiceHostConfigurationFactory.Create();
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

    async Task CreateAssetFileAsync(IOrganization organization, IProject project, IAsset asset, IAssetFileCreation assetFileCreation, CancellationToken token)
    {
        await m_AssetFileManager.CreateAssetFileAsync(organization, project, asset, assetFileCreation, token);
    }

    #endregion

    #region FinalizeAssetFileUpload

    async Task FinalizeAssetFileUploadAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token)
    {
        await m_AssetFileManager.FinalizeAssetFileUploadAsync(organization, project, assetFile, token);
    }

    #endregion

    #region UpdateAssetFile

    async Task UpdateAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token)
    {
        await m_AssetFileManager.UpdateAssetFileAsync(organization, project, assetFile, token);
    }

    #endregion

    #region DeleteAssetFile

    async Task DeleteAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, CancellationToken token)
    {
        await m_AssetFileManager.DeleteAssetFileAsync(organization, project, assetFile, token);
    }

    #endregion

    #region GetAssetFileUrl

    async Task<string> GetAssetFileUrlAsync(IOrganization organization, IProject project, IAssetFile assetFile, AssetFileUrlType urlType, CancellationToken token)
    {
        return await m_AssetFileManager.GetAssetFileUrlAsync(organization, project, assetFile, urlType, token);
    }

    #endregion

    #region UploadAssetFile

    async Task<bool> UploadAssetFileAsync(IOrganization organization, IProject project, IAssetFile assetFile, Stream stream, CancellationToken token)
    {
        return await m_AssetFileManager.UploadAssetFileAsync(organization, project, assetFile, stream, token);
    }

    #endregion
}
}
