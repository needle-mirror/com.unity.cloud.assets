#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Range = System.Range;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader
{
    [Serializable]
    [ExecuteInEditMode]
    [RequireComponent(typeof(OrgAndProjectSelector))]
    public class AssetsUploader : MonoBehaviour
    {
        OrgAndProjectSelector m_OrgAndProjectSelector;
        AssetDatabaseUploaderSample m_AssetDatabaseUploaderSample;

        [SerializeField]
        int m_UploadTimeout = 30000;

        [SerializeField]
        string m_AssetsSourcePath;

        [SerializeField]
        bool m_StepByStep;

        readonly Dictionary<string, IAsset> m_AssetsByPath = new();

        public int UploadTimeout
        {
            get => m_UploadTimeout;
            set => m_UploadTimeout = value;
        }

        public string AssetsSourcePath
        {
            get => m_AssetsSourcePath;
            set => m_AssetsSourcePath = value;
        }

        public bool StepByStep
        {
            get => m_StepByStep;
            set => m_StepByStep = value;
        }

        public List<IAsset> Assets => ToAssetList();

        public event Action AssetsUpdated;

        void Awake()
        {
            TryGetComponent(out m_OrgAndProjectSelector);

            if (m_OrgAndProjectSelector != null)
            {
                m_OrgAndProjectSelector.OnOrgOrProjectChanged += Clear;
            }
        }

        void OnDestroy()
        {
            if (m_OrgAndProjectSelector != null)
            {
                m_OrgAndProjectSelector.OnOrgOrProjectChanged -= Clear;
            }
        }

        /// <summary>
        /// Initialize the <see cref="OrgAndProjectSelector"/> with the given providers.
        /// </summary>
        /// <param name="assetDatabaseUploaderSample"></param>
        public void Initialize(AssetDatabaseUploaderSample assetDatabaseUploaderSample)
        {
            m_AssetDatabaseUploaderSample = assetDatabaseUploaderSample;
        }

        /// <summary>
        /// Create assets, files and Upload them existing in the given path to the selected cloud project.
        /// </summary>
        public async Task CreateAndUploadAssetsAsync()
        {
            if (m_AssetsByPath.Count == 0)
                await SearchAssetsAsync();

            var assetUploadIndex = 0;

            if (!GetAssetPathsFromSource(out var assetPaths))
                return;

            var pathToCreateAndUpload = assetPaths.Except(m_AssetsByPath.Keys).ToArray();

            var assetUploadCount = pathToCreateAndUpload.Length;

            foreach (var assetPath in pathToCreateAndUpload)
            {
                var uploadingAssetName = Path.GetFileNameWithoutExtension(assetPath);
                assetUploadIndex++;

                Debug.Log($"Processing asset : {uploadingAssetName} ({assetUploadIndex}/{assetUploadCount})");

                await CreateAssetAndFileEntriesAndUploadAsync(assetPath, uploadingAssetName);
            }

            if (assetUploadIndex > 0) AssetsUpdated?.Invoke();
        }

        /// <summary>
        /// Search Assets in the selected cloud project from the file names in the given path.
        /// </summary>
        public async Task SearchAssetsAsync()
        {
            Clear();

            if (!GetAssetPathsFromSource(out var assetPaths))
                return;

            bool isUpdated = false;
            foreach (var assetPath in assetPaths)
            {
                var searchAssetName = Path.GetFileNameWithoutExtension(assetPath);
                var asset = await SearchAssetFromName(searchAssetName);
                if (asset != null)
                {
                    isUpdated = true;

                    m_AssetsByPath[assetPath] = asset;

                    Debug.Log("Added asset to list: " + asset.Name);
                }
            }

            if (isUpdated) AssetsUpdated?.Invoke();
        }

        /// <summary>
        /// Create all assets in the given path to the selected cloud project.
        /// </summary>
        public async Task CreateAssetsAsync()
        {
            if (!GetAssetPathsFromSource(out var assetPaths))
                return;

            var assetCreated = false;
            foreach (var assetPath in assetPaths)
            {
                if (!m_AssetsByPath.TryGetValue(assetPath, out var asset))
                {
                    asset = await CreateAssetAsync(assetPath, Path.GetFileNameWithoutExtension(assetPath));
                    if (asset != null)
                    {
                        assetCreated = true;

                        m_AssetsByPath[assetPath] = asset;

                        Debug.Log($"Asset created: {assetPath}");
                    }
                }
            }

            if (assetCreated) AssetsUpdated?.Invoke();
            else Debug.Log("No new assets found.");
        }

        /// <summary>
        /// Create all asset files in the given path to the selected cloud project.
        /// </summary>
        public async Task CreateAssetFilesAsync()
        {
            if (!GetAssetPathsFromSource(out var assetPaths))
                return;

            var fileCreated = false;
            foreach (var assetPath in assetPaths)
            {
                var asset = await GetAsset(assetPath);

                if (asset == null)
                    continue;

                m_AssetsByPath[assetPath] = asset;
                Debug.Log($"Asset added: {asset.Name}");

                if (await FileExists(assetPath, asset))
                    continue;

                var datasets = new List<IDataset>();
                await foreach (var dataset in asset.ListDatasetsAsync(Range.All, CancellationToken.None))
                {
                    datasets.Add(dataset);
                }

                var sourceDataset = datasets.FirstOrDefault();
                if (sourceDataset == null)
                {
                    Debug.LogError($"No datasets found for asset {asset.Name}.");
                }

                if (sourceDataset != null)
                {
                    await UploadFileAsync(assetPath, sourceDataset);
                    fileCreated = true;
                    Debug.Log($"Asset file created and uploaded: {assetPath}");
                }
            }

            if (fileCreated) await SearchAssetsAsync();
            else Debug.Log("No new asset files found.");
        }

        async Task<IAsset> GetAsset(string assetPath)
        {
            if (m_AssetsByPath.TryGetValue(assetPath, out var asset))
            {
                return asset;
            }

            var assetName = Path.GetFileNameWithoutExtension(assetPath);
            return await SearchAssetFromName(assetName);
        }

        async Task<bool> FileExists(string assetPath, IAsset asset)
        {
            var fileName = Path.GetFileName(assetPath);
            await foreach (var file in asset.ListFilesAsync(Range.All, CancellationToken.None))
            {
                if (file.Descriptor.Path == fileName)
                {
                    return true;
                }
            }

            return false;
        }

        bool GetAssetPathsFromSource(out List<string> assetPaths)
        {
            assetPaths = null;

            if (string.IsNullOrEmpty(m_AssetsSourcePath))
            {
                Debug.LogError("Assets to upload path is null or empty.");
                return false;
            }

            var assetGuids = AssetDatabase.FindAssets("", new[] {m_AssetsSourcePath});
            if (assetGuids.Length == 0)
            {
                Debug.Log("No assets found to create and upload.");
                return false;
            }

            assetPaths = new List<string>();
            foreach (var assetGuid in assetGuids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogError($"Failed to get asset path from guid: {assetGuid}");
                    continue;
                }

                if (File.Exists(assetPath))
                {
                    assetPaths.Add(assetPath);
                }
            }

            return true;
        }

        async Task CreateAssetAndFileEntriesAndUploadAsync(string assetPath, string assetName)
        {
            var createdAsset = await CreateAssetAsync(assetPath, assetName);
            if (createdAsset == null)
                return;

            m_AssetsByPath[assetPath] = createdAsset;
            Debug.Log($"Asset created: {assetPath}");


            var datasets = new List<IDataset>();
            await foreach (var dataset in createdAsset.ListDatasetsAsync(Range.All, CancellationToken.None))
            {
                datasets.Add(dataset);
            }

            var sourceDataset = datasets.FirstOrDefault();
            if (sourceDataset == null)
            {
                Debug.LogError($"No datasets found for created asset {createdAsset.Name}.");
            }

            await UploadFileAsync(assetPath, sourceDataset);

            Debug.Log($"Asset file created and uploaded: {assetPath}");
        }

        async Task<IAsset> CreateAssetAsync(string assetPath, string assetName)
        {
            IAsset createdAsset = null;

            var cancellationTokenSource = new CancellationTokenSource(m_AssetDatabaseUploaderSample.CancellationTokenTimeout);

            try
            {
                var assetCreation = new AssetCreation(assetName)
                {
                    Description = $"Uploaded using {nameof(AssetDatabaseUploaderSample)}",
                    Type = GetAssetType(assetPath),
                };

                createdAsset = await m_OrgAndProjectSelector.SelectedProject.CreateAssetAsync(assetCreation, cancellationTokenSource.Token);
                if (createdAsset == null)
                {
                    Debug.LogError($"Failed to create asset: {assetName} from path: {assetPath}");
                }
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return createdAsset;
        }

        async Task<IFile> UploadFileAsync(string assetPath, IDataset dataset)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var filePath = Path.GetFileName(assetPath);

            IFile file = null;
            try
            {
                var assetFileType = GetAssetType(assetPath);

                var fileCreation = new FileCreation
                {
                    Path = filePath,
                    Description = $"Uploaded using {nameof(AssetDatabaseUploaderSample)}",
                    Tags = GetAssetFileTags(assetFileType)
                };

                var fileStream = File.OpenRead(Application.dataPath + assetPath.Replace("Assets/", "/"));

                file = await dataset.UploadFileAsync(fileCreation, fileStream, null, cancellationTokenSource.Token);
            }
            catch (UploadFailedException)
            {
                Debug.LogError($"Failed to upload asset file: {filePath} from path: {assetPath}");
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                Debug.LogError($"Failed to upload asset file: {filePath} from path: {assetPath}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"The specified file {assetPath} raised an error. For more details, see InnerExceptions");
            }

            return file;
        }

        async Task<IAsset> SearchAssetFromName(string assetName)
        {
            var cancellationTokenSource = new CancellationTokenSource(m_AssetDatabaseUploaderSample.CancellationTokenTimeout);

            try
            {
                var assetSearchFilter = new AssetSearchFilter();
                assetSearchFilter.Name.Include(assetName);

                var pagination = new Pagination(nameof(IAsset.Name), Range.All);

                var assetsEnumerator = m_OrgAndProjectSelector.SelectedProject.SearchAssetsAsync(assetSearchFilter, pagination, cancellationTokenSource.Token).GetAsyncEnumerator(cancellationTokenSource.Token);
                try
                {
                    while (await assetsEnumerator.MoveNextAsync())
                    {
                        var asset = assetsEnumerator.Current;
                        if (asset.Name == assetName)
                            return asset;
                    }
                }
                catch (Exception)
                {
                    Debug.Log($"Asset: {assetName} does not exist in the project: {m_OrgAndProjectSelector.SelectedProject.Name}");
                }
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }

        AssetType GetAssetType(string assetPath)
        {
            var assetExtension = Path.GetExtension(assetPath).ToLower();
            switch (assetExtension)
            {
                case ".mat":
                    return AssetType.Material;
                case ".prefab":
                case ".fbx":
                    return AssetType.Model_3D;
                case ".unity":
                    return AssetType.Other;
                case ".shader":
                    return AssetType.Other;
            }

            return AssetType.Other;
        }

        List<string> GetAssetFileTags(AssetType assetFileType)
        {
            return new List<string> { assetFileType.GetValueAsString() };
        }

        void Clear()
        {
            m_AssetsByPath.Clear();
            AssetsUpdated?.Invoke();
        }

        List<IAsset> ToAssetList()
        {
            return m_AssetsByPath.Values.ToList();
        }
    }
}
#endif
