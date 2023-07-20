using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Assets.Documentation.Scripting
{
public class AssetManagerExample
{
    IAssetManager m_AssetManager;

    void ConstructAssetManager()
    {
    #region AssetManagerConstruction

        var httpClient = new UnityHttpClient();
        var cloudConfiguration = UnityRuntimeServiceHostConfigurationFactory.Create();
        var playerSettings = UnityCloudPlayerSettings.Instance;
        var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

        var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, cloudConfiguration)
            .AddDefaultPkceAuthenticator(playerSettings)
            .Build();

        var authenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

        var serviceHttpClient = new ServiceHttpClient(httpClient, authenticator, playerSettings);

        m_AssetManager = new CloudAssetManager(serviceHttpClient, cloudConfiguration);

    #endregion
    }

    #region GetAsset

    async Task<IAsset> GetAsset(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken cancellationToken)
    {
        var asset = await m_AssetManager.GetAssetAsync(organization, project, assetId, assetVersion, cancellationToken);
        return asset;
    }

    #endregion

    #region GetAssetSpecifiedType

    async Task<Asset> GetAsset_GenericType(IOrganization organization, IProject project, string assetId, int assetVersion, CancellationToken cancellationToken)
    {
        var asset = await m_AssetManager.GetAssetAsync<Asset>(organization, project, assetId, assetVersion, cancellationToken);
        return asset;
    }

    #endregion

    #region SearchForAssets

    async Task<IAssetPage> SearchForAssets(IOrganization organization, IProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(organization, project);
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(nameof(IAsset.VersionName), 20);

        var assetPage = await m_AssetManager.SearchAsync(assetSearchFilter, pagination, cancellationToken);
        return assetPage;
    }

    #endregion

    #region SearchForAssetSpecifiedType

    async Task<IAssetPage> SearchForAssets_GenericType(IOrganization organization, IProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(organization, project);
        assetSearchFilter.Name.Include(assetName);

        var pagination = new Pagination(nameof(IAsset.VersionName), 20);

        var assetPage = await m_AssetManager.SearchAsync<Asset>(assetSearchFilter, pagination, cancellationToken);
        return assetPage;
    }

    #endregion

    #region AggregateAssets

    async Task<Aggregation> AggregateAssets(IOrganization organization, IProject project, string assetName, CancellationToken cancellationToken)
    {
        var assetSearchFilter = new AssetSearchFilter(organization, project);
        assetSearchFilter.Project.Include(project);

        var aggregationParameters = new AggregationParameters(nameof(IAsset.Project), 20);

        var aggregation = await m_AssetManager.AggregateAsync(assetSearchFilter, aggregationParameters, cancellationToken);
        return aggregation;
    }

    #endregion

    #region CreateAsset

    async Task<IAsset> CreateAsset(AssetCreation assetCreation, CancellationToken cancellationToken)
    {
        var asset = await m_AssetManager.CreateAssetAsync(assetCreation, cancellationToken);
        return asset;
    }

    #endregion

    #region UpdateAsset

    async Task UpdateAsset(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.UpdateAssetAsync(asset, cancellationToken);
    }

    #endregion

    #region DeleteAsset

    async Task DeleteAsset(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.DeleteAssetAsync(asset, cancellationToken);
    }

    #endregion

    #region GetAssetDownloadUrls

    async Task GetAssetDownloadUrls(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.GetAssetDownloadUrlsAsync(asset, cancellationToken);
    }

    #endregion

    #region GetAssetCollections

    async Task GetAssetCollectionsAsync(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.GetAssetCollectionsAsync(asset, cancellationToken);
    }

    #endregion

    #region LinkAnAssetToProject

    async Task LinkAnAssetToProject(IAsset asset, IOrganization destinationOrganization, IProject destinationProject, CancellationToken cancellationToken)
    {
        await m_AssetManager.LinkAnAssetToProjectAsync(asset, destinationOrganization.GenesisId, destinationProject.Id, cancellationToken);
    }

    #endregion

    #region UnlinkAnAssetFromProject

    async Task UnlinkAnAssetFromProject(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.UnlinkAssetFromProjectAsync(asset, cancellationToken);
    }

    #endregion

    #region CheckProjectIsAssetSourceProject

    async Task<bool> CheckProjectIsAssetSourceProject(IAsset asset, CancellationToken cancellationToken)
    {
        var isAssetSourceProject = await m_AssetManager.CheckProjectIsAssetSourceProjectAsync(asset, cancellationToken);
        return isAssetSourceProject;
    }

    #endregion

    #region PublishApprovedAsset

    async Task PublishApprovedAsset(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.PublishApprovedAssetAsync(asset, cancellationToken);
    }

    #endregion

    #region WithdrawPublishedAsset

    async Task WithdrawPublishedAsset(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.WithdrawPublishedAssetAsync(asset, cancellationToken);
    }

    #endregion

    #region SendAssetToReview

    async Task SendAssetToReviewAsync(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.SendAssetToReviewAsync(asset, cancellationToken);
    }

    #endregion

    #region ApproveAsset

    async Task ApproveAssetAsync(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.ApproveAssetAsync(asset, cancellationToken);
    }

    #endregion

    #region RejectAsset

    async Task RejectAssetAsync(IAsset asset, CancellationToken cancellationToken)
    {
        await m_AssetManager.RejectAssetAsync(asset, cancellationToken);
    }

    #endregion
}
}
