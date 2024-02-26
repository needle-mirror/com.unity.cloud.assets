using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all the information about a cloud project.
    /// </summary>
    sealed class AssetProjectEntity : IAssetProject
    {
        readonly IAssetDataSource m_DataSource;

        /// <inheritdoc />
        public ProjectDescriptor Descriptor { get; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public IDeserializable Metadata { get; set; }

        internal AssetProjectEntity(string id, string name)
        {
            Name = name;
            Descriptor = new ProjectDescriptor(OrganizationId.None, new ProjectId(id));
        }

        internal AssetProjectEntity(IAssetDataSource dataSource, ProjectDescriptor projectDescriptor)
        {
            m_DataSource = dataSource;
            Descriptor = projectDescriptor;
        }

        /// <inheritdoc />
        public async Task<IAsset> GetAssetAsync(AssetId assetId, AssetVersion assetVersion, CancellationToken cancellationToken)
        {
            var data = await m_DataSource.GetAssetAsync(new AssetDescriptor(Descriptor, assetId, assetVersion), FieldsFilter.DefaultAssetIncludes, cancellationToken);
            return data.From(m_DataSource, Descriptor, FieldsFilter.DefaultAssetIncludes);
        }

        /// <inheritdoc />
        public async Task<IAsset> CreateAssetAsync(IAssetCreation assetCreation, CancellationToken cancellationToken)
        {
            var data = await m_DataSource.CreateAssetAsync(Descriptor, assetCreation.From(), cancellationToken);
            return data.From(m_DataSource, Descriptor, FieldsFilter.All);
        }

        /// <inheritdoc />
        public AssetQueryBuilder QueryAssets()
        {
            return new AssetQueryBuilder(m_DataSource, Descriptor);
        }

        /// <inheritdoc />
        public GroupAndCountAssetsQueryBuilder GroupAndCountAssets()
        {
            return new GroupAndCountAssetsQueryBuilder(m_DataSource, Descriptor);
        }

        /// <inheritdoc />
        public CollectionQueryBuilder QueryCollections()
        {
            return new CollectionQueryBuilder(m_DataSource, Descriptor);
        }

        /// <inheritdoc />
        public async Task<IAssetCollection> GetCollectionAsync(CollectionPath collectionPath, CancellationToken cancellationToken)
        {
            var collectionData = await m_DataSource.GetCollectionAsync(new CollectionDescriptor(Descriptor, collectionPath), cancellationToken);
            return collectionData.From(m_DataSource, Descriptor);
        }

        /// <inheritdoc />
        public async Task<IAssetCollection> CreateCollectionAsync(IAssetCollectionCreation assetCollectionCreation, CancellationToken cancellationToken)
        {
            assetCollectionCreation.Validate();

            var creationPath = CollectionPath.CombinePaths(assetCollectionCreation.ParentPath, assetCollectionCreation.Name);
            var assetCollection = new AssetCollection(m_DataSource, new CollectionDescriptor(Descriptor, creationPath), assetCollectionCreation.Name, assetCollectionCreation.Description, assetCollectionCreation.ParentPath);

            var collectionPath = await m_DataSource.CreateCollectionAsync(Descriptor, assetCollection.From(), cancellationToken);
            if (creationPath != collectionPath)
            {
                throw new CreateCollectionFailedException($"Failed to create a collection at path {creationPath}");
            }

            return assetCollection;
        }

        /// <inheritdoc />
        public Task DeleteCollectionAsync(CollectionPath collectionPath, CancellationToken cancellationToken)
        {
            return m_DataSource.DeleteCollectionAsync(new CollectionDescriptor(Descriptor, collectionPath), cancellationToken);
        }

        /// <inheritdoc />
        public TransformationQueryBuilder QueryTransformations()
        {
            return new TransformationQueryBuilder(m_DataSource, Descriptor);
        }
    }
}
