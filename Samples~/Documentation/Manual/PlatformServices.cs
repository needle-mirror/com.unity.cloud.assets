namespace Unity.Cloud.Documentation.Assets
{
    #region PlatformServices

    using System;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.AppLinking.Runtime;
    using Unity.Cloud.Common;
    using Unity.Cloud.Common.Runtime;
    using Unity.Cloud.Identity;
    using Unity.Cloud.Identity.Runtime;

    public static class PlatformServices
    {
        /// <summary>
        /// Returns a <see cref="ICompositeAuthenticator"/>.
        /// </summary>
        public static ICompositeAuthenticator Authenticator { get; private set; }

        /// <summary>
        /// Returns a <see cref="IAuthenticationStateProvider"/>.
        /// </summary>
        public static IAuthenticationStateProvider AuthenticationStateProvider => Authenticator;

        /// <summary>
        /// Returns an <see cref="IOrganizationRepository"/>.
        /// </summary>
        public static IOrganizationRepository OrganizationRepository => Authenticator;

        /// <summary>
        /// Returns an <see cref="IAssetRepository"/>.
        /// </summary>
        public static IAssetRepository AssetRepository { get; private set; }

        public static void Create()
        {
            var playerSettings = UnityCloudPlayerSettings.Instance;

            var serviceConnector = ServiceConnectorFactory.Create(
                PlatformSupportFactory.GetAuthenticationPlatformSupport(),
                new UnityHttpClient(),
                playerSettings,
                playerSettings);

            Authenticator = serviceConnector.CompositeAuthenticator;

            AssetRepository = AssetRepositoryFactory.Create(serviceConnector.ServiceHttpClient, serviceConnector.ServiceHostResolver);
        }

        /// <summary>
        /// A Task that initializes all platform services.
        /// </summary>
        /// <returns>A Task.</returns>
        public static async Task InitializeAsync()
        {
            await Authenticator.InitializeAsync();
        }

        /// <summary>
        /// Shuts down all platform services.
        /// </summary>
        public static void ShutDownServices()
        {
            (Authenticator as IDisposable)?.Dispose();
            Authenticator = null;
        }
    }

    #endregion
}
