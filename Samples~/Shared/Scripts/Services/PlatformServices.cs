#if !UC_EXCLUDE_SAMPLES
using System;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Assets.Samples
{
    public static class PlatformServices
    {
        static CompositeAuthenticator s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="ICompositeAuthenticator"/>.
        /// </summary>
        public static ICompositeAuthenticator Authenticator => s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="IAuthenticationStateProvider"/>.
        /// </summary>
        public static IAuthenticationStateProvider AuthenticationStateProvider => s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="UserInfoProvider"/>.
        /// </summary>
        public static IUserInfoProvider UserInfoProvider { get; private set; }

        /// <summary>
        /// Returns an <see cref="IOrganizationProvider"/>.
        /// </summary>
        public static IOrganizationProvider OrganizationProvider { get; private set; }

        /// <summary>
        /// Returns an <see cref="IProjectProvider"/>.
        /// </summary>
        public static IProjectProvider ProjectProvider { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetProvider"/>
        /// </summary>
        public static IAssetProvider AssetProvider { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetManager"/>
        /// </summary>
        public static IAssetManager AssetManager { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetFileManager"/>
        /// </summary>
        public static IAssetFileManager AssetFileManager { get; private set; }

        /// <summary>
        /// Returns a <see cref="UnityHttpClient"/>
        /// </summary>
        public static IHttpClient HttpClient { get; private set; }

        public static void Create(bool isDiscovery)
        {
            HttpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(HttpClient, platformSupport, serviceHostResolver)
                .AddDefaultPkceAuthenticator(playerSettings)
                .Build();

            s_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            var serviceHttpClient = new ServiceHttpClient(HttpClient, s_CompositeAuthenticator, playerSettings);

            UserInfoProvider = new UserInfoProvider(serviceHttpClient, serviceHostResolver);

            OrganizationProvider = new CloudOrganizationProvider(serviceHttpClient, serviceHostResolver);
            ProjectProvider = new CloudProjectProvider(serviceHttpClient, serviceHostResolver);

            if (isDiscovery)
            {
                AssetProvider = new CloudAssetDiscovery(serviceHttpClient, serviceHostResolver);
                AssetManager = new CloudAssetManager(serviceHttpClient, serviceHostResolver);
                AssetFileManager = null;
            }
            else
            {
                var assetManager = new CloudAssetManager(serviceHttpClient, serviceHostResolver);
                AssetManager = assetManager;
                AssetProvider = assetManager;
                AssetFileManager = new CloudAssetFileManager(serviceHttpClient, serviceHostResolver);
            }
        }

        /// <summary>
        /// A Task that initializes all platform services.
        /// </summary>
        /// <returns>A Task.</returns>
        public static async Task InitializeAsync()
        {
            await s_CompositeAuthenticator.InitializeAsync();
        }

        /// <summary>
        /// Shuts down all platform services.
        /// </summary>
        public static void ShutDownServices()
        {
            (s_CompositeAuthenticator as IDisposable)?.Dispose();
            s_CompositeAuthenticator = null;
        }
    }
}
#endif
