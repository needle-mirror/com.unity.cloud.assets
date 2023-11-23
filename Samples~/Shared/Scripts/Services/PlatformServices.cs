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
        /// Returns a <see cref="IAuthenticatedUserInfoProvider"/>.
        /// </summary>
        public static IAuthenticatedUserInfoProvider AuthenticatedUserInfoProvider => s_CompositeAuthenticator;

        /// <summary>
        /// Returns an <see cref="IOrganizationProvider"/>.
        /// </summary>
        public static IOrganizationRepository OrganizationRepository { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetRepository"/>.
        /// </summary>
        public static IAssetRepository AssetRepository { get; private set; }

        /// <summary>
        /// Returns a <see cref="UnityHttpClient"/>
        /// </summary>
        public static IHttpClient HttpClient { get; private set; }

        public static void Create()
        {
            HttpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();

            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(HttpClient, platformSupport, serviceHostResolver, playerSettings)
                .AddDefaultPkceAuthenticator(playerSettings, playerSettings)
                .Build();

            s_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            var serviceHttpClient = new ServiceHttpClient(HttpClient, s_CompositeAuthenticator, playerSettings);

            OrganizationRepository = new AuthenticatorOrganizationRepository(serviceHttpClient, serviceHostResolver);

            AssetRepository = AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver);
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
