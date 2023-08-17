using System;
using System.Collections.Generic;
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
    var assetServiceConfiguration = new AssetServiceConfiguration(true);

    m_AssetProvider = new CloudAssetProvider(serviceHttpClient, serviceHostResolver, assetServiceConfiguration);

    #endregion
    }

    #region GetAsset

    async Task<IAsset> GetAsset(IProject project, string assetId, int assetVersion, CancellationToken cancellationToken)
    {
        var asset = await m_AssetProvider.GetAssetAsync(project, assetId, assetVersion, cancellationToken);
        return asset;
    }

    #endregion

    #region GetAssetSpecifiedType

    async Task<Asset> GetAsset_GenericType(IProject project, string assetId, int assetVersion, CancellationToken cancellationToken)
    {
        var asset = await m_AssetProvider.GetAssetAsync<Asset>(project, assetId, assetVersion, cancellationToken);
        return asset;
    }

    #endregion

    #region SearchForAssets

    IAsyncEnumerable<IAsset> SearchForAssets(IProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(project);
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(nameof(IAsset.VersionName), Range.All);

        var assets = m_AssetProvider.SearchAsync(assetSearchFilter, pagination, cancellationToken);
        return assets;
    }

    IAsyncEnumerable<IAsset> SearchForAssets(IOrganization organization, IEnumerable<IProject> projects, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(null);
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(nameof(IAsset.VersionName), Range.All);

        var assets = m_AssetProvider.SearchAsync(organization, projects, assetSearchFilter, pagination, cancellationToken);
        return assets;
    }

    #endregion

    #region SearchForAssetSpecifiedType

    IAsyncEnumerable<IAsset> SearchForAssets_GenericType(IProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(project);
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(nameof(IAsset.VersionName), Range.All);

        var assets = m_AssetProvider.SearchAsync<Asset>(assetSearchFilter, pagination, cancellationToken);
        return assets;
    }

    IAsyncEnumerable<IAsset> SearchForAssets_GenericType(IOrganization organization, IEnumerable<IProject> projects,string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(null);
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(nameof(IAsset.VersionName), Range.All);

        var assets = m_AssetProvider.SearchAsync<Asset>(organization, projects, assetSearchFilter, pagination, cancellationToken);
        return assets;
    }

    #endregion
}
}
