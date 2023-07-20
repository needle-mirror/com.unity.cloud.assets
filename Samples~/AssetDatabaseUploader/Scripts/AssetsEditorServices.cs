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

        /// <summary>
        /// Returns a <see cref="UnityHttpClient"/>
        /// </summary>
        public static IServiceHttpClient HttpClient { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static async Task Create(bool isDiscovery)
        {
            var httpClient = new UnityHttpClient();
            var serviceHostResolver = UnityRuntimeServiceHostResolverFactory.Create();
            var playerSettings = UnityCloudPlayerSettings.Instance;

            s_Authenticator = new UnityEditorAuthenticator(new TargetClientIdTokenToUnityServicesTokenExchanger(httpClient, serviceHostResolver));
            HttpClient = new ServiceHttpClient(httpClient, s_Authenticator, playerSettings);

            OrganizationProvider = new CloudOrganizationProvider(HttpClient, serviceHostResolver);
            ProjectProvider = new CloudProjectProvider(HttpClient, serviceHostResolver);

            if (isDiscovery)
            {
                AssetProvider = new CloudAssetDiscovery(HttpClient, serviceHostResolver);
                AssetManager = new CloudAssetManager(HttpClient, serviceHostResolver);
                AssetFileManager = null;
            }
            else
            {
                var assetManager = new CloudAssetManager(HttpClient, serviceHostResolver);
                AssetManager = assetManager;
                AssetProvider = assetManager;
                AssetFileManager = new CloudAssetFileManager(HttpClient, serviceHostResolver);
            }

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
