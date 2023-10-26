#if UC_MOCK_ASSETS
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class MockDataSource : IAssetDataSource
    {
        const string k_DefaultDescription = "Default description";
        const string k_DefaultName = "Default name";
        AssetCollectionData GetDefaultAssetCollectionData()
        {
            return new AssetCollectionData()
            {
                Description = k_DefaultDescription,
                Name = k_DefaultName,
                ParentPath = new CollectionPath(null)
            };
        }

        Dictionary<ProjectDescriptor, List<AssetCollectionData>> m_Collections = new Dictionary<ProjectDescriptor, List<AssetCollectionData>>();
        Dictionary<AssetDescriptor, HashSet<AssetCollectionData>> m_Assets_Collections = new Dictionary<AssetDescriptor, HashSet<AssetCollectionData>>();

        AssetCollectionData EnsureCollectionData(ProjectDescriptor projectDescriptor, string name, CollectionPath parentCollectionPath, CollectionPath? fullPath = null)
        {
            EnsureProjectData(projectDescriptor.OrganizationGenesisId, projectDescriptor.ProjectId);
            List<AssetCollectionData> collectionList = null;
            if (!m_Collections.TryGetValue(projectDescriptor, out collectionList))
            {
                collectionList = new List<AssetCollectionData>();
                m_Collections.Add(projectDescriptor, collectionList);
            }

            var collectionData = collectionList.Find(a => (a.Name == name && a.ParentPath == parentCollectionPath)
                                                           || a.GetFullCollectionPath() == fullPath);
            if (collectionData == null)
            {
                collectionData = GetDefaultAssetCollectionData();
                collectionData.Name = name ?? fullPath;
                collectionData.ParentPath = name != null ? parentCollectionPath : null;
                collectionList.Add(collectionData);
            }
            return collectionData;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IAssetCollectionData>> GetAssetCollectionsAsync(AssetDescriptor assetDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            EnsureAssetData(assetDescriptor);
            HashSet<AssetCollectionData> collectionHash = EnsureAssetCollectionHash(assetDescriptor);
            return collectionHash.ToArray();
        }

        HashSet<AssetCollectionData> EnsureAssetCollectionHash(AssetDescriptor assetDescriptor)
        {
            HashSet<AssetCollectionData> collectionHash = null;
            if (!m_Assets_Collections.TryGetValue(assetDescriptor, out collectionHash))
            {
                collectionHash = new HashSet<AssetCollectionData>();
                m_Assets_Collections.Add(assetDescriptor, collectionHash);
                collectionHash.Add(EnsureCollectionData(assetDescriptor.ProjectDescriptor, "Collection 1", null));
                collectionHash.Add(EnsureCollectionData(assetDescriptor.ProjectDescriptor, "Collection 2", null));
            }
            return collectionHash;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IAssetCollectionData>> ListCollectionsAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            List<AssetCollectionData> collectionList = null;
            if (!m_Collections.TryGetValue(projectDescriptor, out collectionList))
            {
                EnsureCollectionData(projectDescriptor, "Collection 1", null);
                EnsureCollectionData(projectDescriptor, "Collection 2", null);
                collectionList = m_Collections[projectDescriptor];
            }

            return collectionList.ToArray();
        }

        /// <inheritdoc/>
        public async Task<IAssetCollectionData> GetCollectionAsync(CollectionDescriptor descriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return EnsureCollectionData(descriptor.ProjectDescriptor, null, null, descriptor.CollectionPath);
        }

        /// <inheritdoc/>
        public async Task<CollectionPath> CreateCollectionAsync(ProjectDescriptor projectDescriptor, IAssetCollectionData assetCollection, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var created = EnsureCollectionData(projectDescriptor, assetCollection.Name, assetCollection.ParentPath);
            created.Description = assetCollection.Description;
            return created.GetFullCollectionPath();
        }

        /// <inheritdoc/>
        public async Task UpdateCollectionAsync(CollectionDescriptor descriptor, IAssetCollectionData assetCollection, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var toUpdate= EnsureCollectionData(descriptor.ProjectDescriptor, null, null, descriptor.CollectionPath);
            toUpdate.Name = assetCollection.Name;
            toUpdate.Description = assetCollection.Description;
            toUpdate.ParentPath = assetCollection.ParentPath;
        }

        /// <inheritdoc/>
        public async Task DeleteCollectionAsync(CollectionDescriptor descriptor, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            AssetCollectionData collection = null;
            if (m_Collections.TryGetValue(descriptor.ProjectDescriptor, out var collectionList))
            {
                collection = collectionList.Find(x => x.GetFullCollectionPath() == descriptor.CollectionPath);
                collectionList.Remove(collection);
            }

            if (collection != null)
            {
                _ = m_Assets_Collections.Select(pair => pair.Value.Remove(collection));
            }
        }

        /// <inheritdoc />
        public async Task AddAssetsToCollectionAsync(CollectionDescriptor descriptor, IEnumerable<AssetId> assets, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var collection = EnsureCollectionData(descriptor.ProjectDescriptor, null, null, descriptor.CollectionPath);
            foreach (var asset in assets)
            {
                var assetData = EnsureAssetData(descriptor.ProjectDescriptor, asset, new AssetVersion(1));
                var hashSet = EnsureAssetCollectionHash(new AssetDescriptor(descriptor.ProjectDescriptor, assetData.Id, assetData.Version));
                hashSet.Add(collection);
            }
        }

        /// <inheritdoc />
        public async Task RemoveAssetsFromCollectionAsync(CollectionDescriptor descriptor, IEnumerable<AssetId> assets, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var collection = EnsureCollectionData(descriptor.ProjectDescriptor, null, null, descriptor.CollectionPath);
            foreach (var asset in assets)
            {
                var assetData = EnsureAssetData(descriptor.ProjectDescriptor, asset, new AssetVersion(1));
                var hashSet = EnsureAssetCollectionHash(new AssetDescriptor(descriptor.ProjectDescriptor, assetData.Id, assetData.Version));
                hashSet.Remove(collection);
            }
        }

        /// <inheritdoc />
        public async Task<CollectionPath> MoveCollectionToNewPathAsync(CollectionDescriptor descriptor, CollectionPath newCollectionPath, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return newCollectionPath;
        }
    }
}
#endif
