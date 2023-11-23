#if UNITY_EDITOR

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader
{
    [Serializable]
    [ExecuteInEditMode]
    [RequireComponent(typeof(OrgAndProjectSelector))]
    [RequireComponent(typeof(AssetsUploader))]
    public class AssetDatabaseUploaderSample : MonoBehaviour
    {
        [SerializeField]
        int m_CancellationTokenTimeout = 5000;

        OrgAndProjectSelector m_OrgAndProjectSelector;
        AssetsUploader m_AssetsUploader;

        public int CancellationTokenTimeout
        {
            get => m_CancellationTokenTimeout;
            set => m_CancellationTokenTimeout = value;
        }

        public async Task Initialize()
        {
            if (!AssetsEditorServices.IsInitialized)
            {
                await AssetsEditorServices.Create(false);
            }

            if (m_OrgAndProjectSelector)
            {
                await m_OrgAndProjectSelector.Initialize(this, AssetsEditorServices.OrganizationRepository,
                    AssetsEditorServices.AssetRepository);
            }

            if (m_AssetsUploader)
            {
                m_AssetsUploader.Initialize(this);
            }
        }

        void Awake()
        {
            TryGetComponent(out m_OrgAndProjectSelector);
            TryGetComponent(out m_AssetsUploader);
        }

        async Task OnEnable()
        {
            TryGetComponent(out m_OrgAndProjectSelector);
            TryGetComponent(out m_AssetsUploader);

            await Initialize();
        }

        void OnDestroy()
        {
            AssetsEditorServices.ShutDownServices();
        }
    }
}
#endif
