#if !UC_EXCLUDE_SAMPLES && UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader
{
    [Serializable]
    [ExecuteInEditMode]
    [RequireComponent(typeof(OrgAndProjectSelector))]
    public class AssetsUploader : MonoBehaviour
    {
        IAssetManager m_AssetManager;
        IAssetFileManager m_AssetFileManager;
        OrgAndProjectSelector m_OrgAndProjectSelector;
        AssetDatabaseUploaderSample m_AssetDatabaseUploaderSample;

        [SerializeField]
        int m_UploadTimeout = 30000;

        [SerializeField]
        string m_AssetsSourcePath;

        [SerializeField]
        bool m_StepByStep = false;

        Dictionary<string, IAsset> m_AssetsByPath;
        Dictionary<string, string> m_UploadUrlByPath;

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

        public List<IAsset> Assets => m_AssetsByPath?.Values.ToList();

        public event Action AssetsUpdated;

        void Awake()
        {
            TryGetComponent(out m_OrgAndProjectSelector);

            if (m_OrgAndProjectSelector != null)
            {
                m_OrgAndProjectSelector.OnOrgOrProjectChanged += Clear;
            }

            m_AssetsByPath ??= new Dictionary<string, IAsset>();
            m_UploadUrlByPath ??= new Dictionary<string, string>();
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
        /// <param name="assetManager"></param>
        /// <param name="assetFileManager"></param>
        public void Initialize(AssetDatabaseUploaderSample assetDatabaseUploaderSample, IAssetManager assetManager, IAssetFileManager assetFileManager)
        {
            m_AssetDatabaseUploaderSample = assetDatabaseUploaderSample;

            m_AssetManager = assetManager;
            if (m_AssetManager == null)
            {
                Debug.LogError($"An {nameof(IAssetManager)} is required to initialize {nameof(AssetsUploader)}");
            }

            m_AssetFileManager = assetFileManager;
            if (m_AssetFileManager == null)
            {
                Debug.LogError($"An {nameof(IAssetFileManager)} is required to initialize {nameof(AssetsUploader)}");
            }
        }

        /// <summary>
        /// Create assets, files and Upload them existing in the given path to the selected cloud project.
        /// </summary>
        public async Task CreateAndUploadAssetsAsync()
        {
            if(m_AssetsByPath.Count == 0)
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

            if(assetUploadIndex > 0) AssetsUpdated?.Invoke();
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

            if(isUpdated) AssetsUpdated?.Invoke();
        }

        /// <summary>
        /// Create all assets in the given path to the selected cloud project.
        /// </summary>
        public async Task CreateAssetsAsync()
        {
            if (!GetAssetPathsFromSource(out var assetPaths))
                return;

            bool isUpdated = false;
            foreach (var assetPath in assetPaths)
            {
                if(!m_AssetsByPath.TryGetValue(assetPath, out var asset))
                {
                    asset = await CreateAssetAsync(assetPath, Path.GetFileNameWithoutExtension(assetPath));
                    if (asset != null)
                    {
                        isUpdated = true;

                        m_AssetsByPath[assetPath] = asset;

                        Debug.Log($"Asset created: {assetPath}");
                    }
                }
            }

            if(isUpdated) AssetsUpdated?.Invoke();
        }

        /// <summary>
        /// Create all asset files in the given path to the selected cloud project.
        /// </summary>
        public async Task CreateAssetFilesAsync()
        {
            if (!GetAssetPathsFromSource(out var assetPaths))
                return;

            bool fileCreated = false;
            m_UploadUrlByPath ??= new Dictionary<string, string>();

            foreach (var assetPath in assetPaths)
            {
                if(!m_AssetsByPath.TryGetValue(assetPath, out var asset))
                {
                    var assetName = Path.GetFileNameWithoutExtension(assetPath);
                    asset = await SearchAssetFromName(assetName);
                    if(asset == null)
                        continue;

                    m_AssetsByPath[assetPath] = asset;
                    Debug.Log($"Asset added: {asset.Name}");
                }

                if(asset.Files.FirstOrDefault(f => f.Name == asset.Name) != null)
                    continue;

                var assetFile = await CreateAssetFileAsync(assetPath, asset);
                if (assetFile != null)
                {
                    fileCreated = true;
                    m_UploadUrlByPath[assetPath] = assetFile.UploadUrl;

                    Debug.Log($"Asset file created: {assetPath}");
                }
            }

            if (fileCreated)
                await SearchAssetsAsync();
        }

        /// <summary>
        /// Upload last created assets and files in the given path to the selected cloud project.
        /// </summary>
        public async Task UploadAssetsAsync()
        {
            var assetUploadIndex = 0;
            var assetUploadCount = m_AssetsByPath.Count;

            foreach (var assetWithPath in m_AssetsByPath)
            {
                var assetFile = assetWithPath.Value.Files.FirstOrDefault(f => f.Name == assetWithPath.Value.Name);
                if(assetFile == null)
                    continue;

                var uploadingAssetName = assetWithPath.Value.Name;
                assetUploadIndex++;

                Debug.Log($"Uploading asset : {uploadingAssetName} ({assetUploadIndex}/{assetUploadCount})");

                await UploadAssetAsync(assetWithPath.Key, assetFile);
            }
        }

        bool GetAssetPathsFromSource(out List<string> assetPaths)
        {
            assetPaths = null;

            if (string.IsNullOrEmpty(m_AssetsSourcePath))
            {
                Debug.LogError("Assets to upload path is null or empty.");
                return false;
            }

            var assetGuids = AssetDatabase.FindAssets("", new[] { m_AssetsSourcePath });
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
            if(createdAsset == null)
                return;

            m_AssetsByPath[assetPath] = createdAsset;
            Debug.Log($"Asset created: {assetPath}");

            var createdAssetFile = await CreateAssetFileAsync(assetPath, createdAsset);
            if(createdAssetFile == null)
                return;

            Debug.Log($"Asset file created: {assetPath}");

            await UploadAssetAsync(assetPath, createdAssetFile);
        }

        async Task<IAsset> CreateAssetAsync(string assetPath, string assetName)
        {
            IAsset createdAsset = null;

            var cancellationTokenSource = new CancellationTokenSource(m_AssetDatabaseUploaderSample.CancellationTokenTimeout);

            try
            {
                var assetCreation = new AssetCreation
                {
                    Project = m_OrgAndProjectSelector.SelectedProject,
                    Name = assetName,
                    Description = $"Uploaded using {nameof(AssetDatabaseUploaderSample)}",
                    Type = GetAssetType(assetPath),
                    Version = 1,
                    VersionName = "1.0.0",
                };

                createdAsset = await m_AssetManager.CreateAssetAsync(assetCreation, cancellationTokenSource.Token);
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

        async Task<IAssetFile> CreateAssetFileAsync(string assetPath, IAsset createdAsset)
        {
            var cancellationTokenSource = new CancellationTokenSource(m_AssetDatabaseUploaderSample.CancellationTokenTimeout);

            IAssetFile assetFile = null;
            try
            {
                var fileInfo = new FileInfo(assetPath);
                var assetFileType = GetAssetType(assetPath);

                var fileCreation = new AssetFileCreation
                {
                    Name = Path.GetFileName(assetPath),
                    Description = $"Uploaded using {nameof(AssetDatabaseUploaderSample)}",
                    Type = assetFileType,
                    FileSize = fileInfo.Length,
                    Tags = GetAssetFileTags(assetFileType)
                };

                assetFile = await m_AssetFileManager.CreateAssetFileAsync(m_OrgAndProjectSelector.SelectedProject, createdAsset, fileCreation, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                Debug.LogError($"Failed to create asset file: {createdAsset.Name} from path: {assetPath}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Failed to create asset file: {createdAsset.Name} from path: {assetPath}");
            }

            return assetFile;
        }

        async Task UploadAssetAsync(string assetPath, IAssetFile assetFile)
        {
            var cancellationTokenSource = new CancellationTokenSource(m_UploadTimeout);

            try
            {
                if (string.IsNullOrEmpty(assetFile.UploadUrl))
                {
                    if(!m_UploadUrlByPath.TryGetValue(assetPath, out string uploadUrl))
                        return;

                    assetFile.UploadUrl = uploadUrl;//Restore AssetFile upload url
                }

                var fileStream = File.OpenRead(Application.dataPath + assetPath.Replace("Assets/","/"));

                var didUpload = await m_AssetFileManager.UploadAssetFileAsync(
                    m_OrgAndProjectSelector.SelectedProject,
                    assetFile,
                    fileStream,
                    null,// will be done in another PR
                    cancellationTokenSource.Token);

                if (didUpload)
                {
                    await m_AssetFileManager.FinalizeAssetFileUploadAsync(
                        m_OrgAndProjectSelector.SelectedProject,
                        assetFile,
                        cancellationTokenSource.Token);

                    Debug.Log($"Asset file upload: {assetPath} finalized.");
                }
                else
                {
                    Debug.LogError($"Failed to upload asset file: {assetFile.Name} from path: {assetPath}");
                }
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                Debug.LogError($"Failed to upload asset file: {assetFile.Name} from path: {assetPath}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"The specified file {assetPath} raised an error. For more details, see InnerExceptions");
            }
        }

        async Task<IAsset> SearchAssetFromName(string assetName)
        {
            var cancellationTokenSource = new CancellationTokenSource(m_AssetDatabaseUploaderSample.CancellationTokenTimeout);

            try
            {
                var assetSearchFilter = new AssetSearchFilter(m_OrgAndProjectSelector.SelectedProject);
                assetSearchFilter.Name.ForAny(assetName);

                var pagination = new Pagination(nameof(IAsset.Name), new Range(0, 1));

                var assetsEnumerator = m_AssetManager.SearchAsync(assetSearchFilter, pagination, cancellationTokenSource.Token).GetAsyncEnumerator(cancellationTokenSource.Token);
                try
                {
                    await assetsEnumerator.MoveNextAsync();
                    return assetsEnumerator.Current;
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

        string GetAssetType(string assetPath)
        {
            var assetExtension = Path.GetExtension(assetPath).ToLower();
            switch (assetExtension)
            {
                case ".mat":
                    return "Material";
                case ".prefab":
                case ".fbx":
                    return "Model";
                case ".unity":
                    return "Unity_Scene";
                case ".shader":
                    return "Shader";
            }

            return "Other";
        }

        List<string> GetAssetFileTags(string assetFileType)
        {
            switch (assetFileType)
            {
                case "Material":
                    return new List<string> { "Material" };
                case "Model":
                    return new List<string> { "Model" };
                case "Unity_Scene":
                    return new List<string> { "Unity_Scene" };
                case "Shader":
                    return new List<string> { "Shader" };
            }

            return new List<string>();
        }

        void Clear()
        {
            if (m_AssetsByPath != null)
            {
                m_AssetsByPath.Clear();
            }
            else
            {
                m_AssetsByPath ??= new Dictionary<string, IAsset>();
                m_UploadUrlByPath ??= new Dictionary<string, string>();
            }

            AssetsUpdated?.Invoke();
        }
    }
}
#endif
