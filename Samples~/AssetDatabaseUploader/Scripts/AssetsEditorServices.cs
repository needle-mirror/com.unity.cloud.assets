#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR
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

        public static bool IsInitialized { get; private set; }

        public static async Task Create(bool isDiscovery)
        {
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            s_Authenticator = new UnityEditorAuthenticator(new TargetClientIdTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver));
            var serviceHttpClient = new ServiceHttpClient(httpClient, s_Authenticator, playerSettings);

            OrganizationProvider = new CloudOrganizationProvider(serviceHttpClient, serviceHostResolver);
            ProjectProvider = new CloudProjectProvider(serviceHttpClient, serviceHostResolver);

            var assetServiceConfiguration = new AssetServiceConfiguration(isDiscovery);

            AssetProvider = new CloudAssetProvider(serviceHttpClient, serviceHostResolver, assetServiceConfiguration);
            AssetManager = new CloudAssetManager(serviceHttpClient, serviceHostResolver, assetServiceConfiguration);
            AssetFileManager = new CloudAssetFileManager(serviceHttpClient, serviceHostResolver);

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
