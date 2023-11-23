#if UNITY_EDITOR
using System;
using System.Threading.Tasks;
using Unity.Cloud.Common;
using Unity.Cloud.Common.Runtime;
using Unity.Cloud.Identity;
using Unity.Cloud.Identity.Editor;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader
{
    public static class AssetsEditorServices
    {
        static IAuthenticator s_Authenticator;

        /// <summary>
        /// Returns an <see cref="IOrganizationRepository"/>.
        /// </summary>
        public static IOrganizationRepository OrganizationRepository { get; private set; }

        /// <summary>
        /// Returns an <see cref="IAssetRepository"/>.
        /// </summary>
        public static IAssetRepository AssetRepository { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static async Task Create(bool isDiscovery)
        {
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            s_Authenticator = new UnityEditorAuthenticator(new TargetClientIdTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver));
            var serviceHttpClient = new ServiceHttpClient(httpClient, s_Authenticator, playerSettings);

            OrganizationRepository = new AuthenticatorOrganizationRepository(serviceHttpClient, serviceHostResolver);

            AssetRepository = AssetRepositoryFactory.Create(serviceHttpClient, serviceHostResolver);

            IsInitialized = true;

            await s_Authenticator.InitializeAsync();
        }

        /// <summary>
        /// Shuts down all platform services.
        /// </summary>
        public static void ShutDownServices()
        {
            (s_Authenticator as IDisposable)?.Dispose();
            s_Authenticator = null;

            IsInitialized = false;
        }
    }
}
#endif
