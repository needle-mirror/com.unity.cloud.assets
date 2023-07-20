using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Assets.Documentation.Scripting
{
public class AssetProviderExample
{
    IAssetProvider m_AssetProvider;

    void ConstructAssetDiscovery()
    {
    #region ConstructAssetDiscovery

    var httpClient = new UnityHttpClient();
    var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
    var playerSettings = UnityCloudPlayerSettings.Instance;
    var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

    var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver)
        .AddDefaultPkceAuthenticator(playerSettings)
        .Build();

    var authenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

    var serviceHttpClient = new ServiceHttpClient(httpClient, authenticator, playerSettings);

    m_AssetProvider = new CloudAssetDiscovery(serviceHttpClient, serviceHostResolver);

    #endregion
    }

    void ConstructAssetProvider()
    {
    #region ConstructAssetProvider

    var httpClient = new UnityHttpClient();
    var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
    var playerSettings = UnityCloudPlayerSettings.Instance;
    var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

    var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver)
        .AddDefaultPkceAuthenticator(playerSettings)
        .Build();

    var authenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

    var serviceHttpClient = new ServiceHttpClient(httpClient, authenticator, playerSettings);

    m_AssetProvider = new CloudAssetProvider(serviceHttpClient, serviceHostResolver);

    #endregion
    }

    #region GetAsset

    async Task<IAsset> GetAsset(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken cancellationToken)
    {
        var asset = await m_AssetProvider.GetAssetAsync(organization, project, assetId, assetVersion, cancellationToken);
        return asset;
    }

    #endregion

    #region GetAssetSpecifiedType

    async Task<Asset> GetAsset_GenericType(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken cancellationToken)
    {
        var asset = await m_AssetProvider.GetAssetAsync<Asset>(organization, project, assetId, assetVersion, cancellationToken);
        return asset;
    }

    #endregion

    #region SearchForAssets

    async Task<IAssetPage> SearchForAssets(IOrganization organization, IProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(organization, project);
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(nameof(IAsset.VersionName), 20);

        var assetPage = await m_AssetProvider.SearchAsync(assetSearchFilter, pagination, cancellationToken);
        return assetPage;
    }

    #endregion

    #region SearchForAssetSpecifiedType

    async Task<IAssetPage> SearchForAssets_GenericType(IOrganization organization, IProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(organization, project);
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(nameof(IAsset.VersionName), 20);

        var assetPage = await m_AssetProvider.SearchAsync<Asset>(assetSearchFilter, pagination, cancellationToken);
        return assetPage;
    }

    #endregion
}
}
