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
        /// Returns an <see cref="IOrganizationProvider"/>.
        /// </summary>
        public static IOrganizationProvider OrganizationProvider { get; private set; }

        /// <summary>
        /// Returns an <see cref="IProjectProvider"/>.
        /// </summary>
        public static IProjectProvider ProjectProvider { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetManager"/>
        /// </summary>
        public static IAssetManager AssetManager { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetFileManager"/>
        /// </summary>
        public static IAssetFileManager AssetFileManager { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetCollectionManager"/>
        /// </summary>
        public static IAssetCollectionManager AssetCollectionManager { get; private set; }

        public static void Create()
        {
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver)
                .AddDefaultPkceAuthenticator(playerSettings)
                .Build();

            Authenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            var serviceHttpClient = new ServiceHttpClient(httpClient, Authenticator, playerSettings);

            OrganizationProvider = new CloudOrganizationProvider(serviceHttpClient, serviceHostResolver);
            ProjectProvider = new CloudProjectProvider(serviceHttpClient, serviceHostResolver);

            var assetServiceConfiguration = new AssetServiceConfiguration();

            AssetManager = new CloudAssetManager(serviceHttpClient, serviceHostResolver, assetServiceConfiguration);
            AssetFileManager = new CloudAssetFileManager(serviceHttpClient, serviceHostResolver);
            AssetCollectionManager = new CloudAssetCollectionManager(serviceHttpClient, serviceHostResolver);
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
