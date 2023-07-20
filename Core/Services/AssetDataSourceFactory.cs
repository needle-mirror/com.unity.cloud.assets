using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    static class AssetDataSourceFactory
    {
        internal static IAssetDataSource CreateDiscoveryDataSource(IServiceHttpClient serviceHttpClient, AssetHostConfiguration serviceHostConfiguration)
        {
            var httpClient = new AssetDiscoveryHttpClient(serviceHttpClient, serviceHostConfiguration.GetServiceAddress());
            return new AssetDataSource(httpClient);
        }

        internal static IAssetDataSource CreateManagementDataSource(IServiceHttpClient serviceHttpClient, AssetHostConfiguration serviceHostConfiguration)
        {
            var httpClient = new AssetHttpClient(serviceHttpClient, serviceHostConfiguration.GetServiceAddress());
            return new AssetDataSource(httpClient, "/assets");
        }
    }
}
