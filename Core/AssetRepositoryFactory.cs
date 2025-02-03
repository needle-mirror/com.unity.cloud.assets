using System.Reflection;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public static class AssetRepositoryFactory
    {
        public static IAssetRepository Create(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            return Create(serviceHttpClient, serviceHostResolver, AssetRepositoryCacheConfiguration.Legacy);
        }

        public static IAssetRepository Create(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver, AssetRepositoryCacheConfiguration assetRepositoryCacheConfiguration)
        {
            serviceHttpClient = serviceHttpClient.WithApiSourceHeadersFromAssembly(Assembly.GetExecutingAssembly());
            var dataSource = new AssetDataSource(serviceHttpClient, serviceHostResolver);
            return new AssetRepository(dataSource, assetRepositoryCacheConfiguration);
        }
    }
}
