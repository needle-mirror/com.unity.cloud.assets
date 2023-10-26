namespace Unity.Cloud.Assets.Documentation.Management
{
    #region PlatformServices

    using System;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
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
        public static IOrganizationRepository OrganizationRepository { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetRepository"/>.
        /// </summary>
        public static IAssetRepository AssetRepository { get; private set; }

        public static void Create()
        {
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver, playerSettings)
                .AddDefaultPkceAuthenticator(playerSettings, playerSettings)
                .Build();

            Authenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            var serviceHttpClient = new ServiceHttpClient(httpClient, Authenticator, playerSettings);

            OrganizationRepository = new AuthenticatorOrganizationRepository(serviceHttpClient, serviceHostResolver);

            AssetRepository = AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver);
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
