using System.Reflection;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public static class AssetRepositoryFactory
    {
        public static IAssetRepository Create(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
        {
            IAssetDataSource dataSource = null;
#if UC_MOCK_ASSETS
            dataSource = new MockDataSource();
#else
            serviceHttpClient = serviceHttpClient.WithApiSourceHeadersFromAssembly(Assembly.GetExecutingAssembly());
            dataSource = new AssetDataSource(serviceHttpClient, serviceHostResolver);
#endif
            return new AssetRepository(dataSource);
        }
    }
}
