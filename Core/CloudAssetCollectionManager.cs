using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An class that provides the methods to interact with an <see cref="IAssetCollection"/>.
    /// </summary>
    public class CloudAssetCollectionManager : IAssetCollectionManager
    {
        readonly IAssetCollectionDataSource m_DataSource;

        /// <summary>
        /// Initializes and returns an instance of <see cref="CloudAssetCollectionManager"/>
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostResolver"> The <see cref="IServiceHostResolver"/> object. </param>
        /// <example>
        /// <code source="../Samples/Documentation/Scripting/CollectionManagerExample.cs" region="ConstructAssetCollectionManager"/>
        /// </example>
        public CloudAssetCollectionManager(IServiceHttpClient serviceHttpClient, IServiceHostResolver serviceHostResolver)
            : this(serviceHttpClient, ServiceHostConfigurationFactory.Create(serviceHostResolver))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CloudAssetCollectionManager"/> class.
        /// </summary>
        /// <param name="serviceHttpClient"> The <see cref="IServiceHttpClient"/> used to fetch the data. </param>
        /// <param name="serviceHostConfiguration"> The configuration object. </param>
        CloudAssetCollectionManager(IServiceHttpClient serviceHttpClient, AssetHostConfiguration serviceHostConfiguration)
            : this(new AssetCollectionDataSource(serviceHttpClient, serviceHostConfiguration.GetServiceAddress()))
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CloudAssetCollectionManager"/> class.
        /// </summary>
        /// <param name="dataSource"></param>
        internal CloudAssetCollectionManager(IAssetCollectionDataSource dataSource)
        {
            m_DataSource = dataSource;
        }

        /// <inheritdoc/>
        public Task<IAssetCollection[]> ListCollectionsAsync(IOrganization organization, IProject project, CancellationToken token)
        {
            return m_DataSource.ListCollectionsAsync(organization, project, token);
        }

        /// <inheritdoc/>
        public Task<IAssetCollection> GetCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, CancellationToken token)
        {
            return m_DataSource.GetCollectionAsync(organization, project, collectionPath, token);
        }

        /// <inheritdoc />
        public Task<CollectionPath> CreateCollectionAsync(IOrganization organization, IProject project, IAssetCollection assetCollection, CancellationToken token)
        {
            AssetCollection.VerifyArguments(assetCollection.Name, assetCollection.Description);

            return m_DataSource.CreateCollectionAsync(organization, project, assetCollection, token);
        }

        /// <inheritdoc/>
        public Task UpdateCollectionAsync(IAssetCollection assetCollection, CancellationToken token)
        {
            return m_DataSource.UpdateCollectionAsync(assetCollection.Organization, assetCollection.Project, assetCollection, token);
        }

        /// <inheritdoc/>
        public Task DeleteCollectionAsync(IAssetCollection assetCollection, CancellationToken token)
        {
            return m_DataSource.DeleteCollectionAsync(assetCollection.Organization, assetCollection.Project, assetCollection.GetFullCollectionPath(), token);
        }

        /// <inheritdoc />
        public Task<string> MoveCollectionToNewPathAsync(IAssetCollection assetCollection, CollectionPath newCollectionPath, CancellationToken token)
        {
            return m_DataSource.MoveCollectionToNewPathAsync(assetCollection.Organization, assetCollection.Project, assetCollection.GetFullCollectionPath(), newCollectionPath, token);
        }

        /// <inheritdoc />
        public Task InsertAssetsToCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token)
        {
            return m_DataSource.InsertAssetsToCollectionAsync(organization, project, collectionPath, assets, token);
        }

        /// <inheritdoc />
        public Task RemoveAssetsFromCollectionAsync(IOrganization organization, IProject project, CollectionPath collectionPath, IEnumerable<IAsset> assets, CancellationToken token)
        {
            return m_DataSource.RemoveAssetsFromCollectionAsync(organization, project, collectionPath, assets, token);
        }
    }
}
