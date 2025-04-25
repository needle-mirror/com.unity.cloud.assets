using System;
using System.Threading.Tasks;
using Unity.Cloud.AppLinking.Runtime;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity;
using Unity.Cloud.Identity.Runtime;

namespace Unity.Cloud.Assets.Samples
{
    public static class PlatformServices
    {
        static ICompositeAuthenticator s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="ICompositeAuthenticator"/>.
        /// </summary>
        public static ICompositeAuthenticator Authenticator => s_CompositeAuthenticator;

        /// <summary>
        /// Returns a <see cref="IAuthenticationStateProvider"/>.
        /// </summary>
        public static IAuthenticationStateProvider AuthenticationStateProvider => s_CompositeAuthenticator;

        /// <summary>
        /// Returns an <see cref="IOrganizationRepository"/>.
        /// </summary>
        public static IOrganizationRepository OrganizationRepository => s_CompositeAuthenticator;

        /// <summary>
        /// Returns an <see cref="IAssetRepository"/>.
        /// </summary>
        public static IAssetRepository AssetRepository { get; private set; }

        public static void Create()
        {
            var platformSupport = PlatformSupportFactory.GetAuthenticationPlatformSupport();
            var httpClient = new UnityHttpClient();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var compositeAuthenticatorSettings = new CompositeAuthenticatorSettingsBuilder(httpClient, platformSupport, serviceHostResolver, playerSettings)
                .AddDefaultPkceAuthenticator(playerSettings)
                .Build();

            s_CompositeAuthenticator = new CompositeAuthenticator(compositeAuthenticatorSettings);

            var serviceHttpClient = new ServiceHttpClient(httpClient, s_CompositeAuthenticator, playerSettings);
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
