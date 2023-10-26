#if !UC_EXCLUDE_SAMPLES
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples
{
    /// <summary>
    /// A Monobehaviour class to initialize services and dependencies for the Unity Cloud platform.
    /// </summary>
    [DefaultExecutionOrder(int.MinValue)]
    [AddComponentMenu("Assets/Samples/Platform Services Initialization")]
    public class PlatformServicesInitialization : MonoBehaviour
    {
        void Awake()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
#endif
            Initialize();
        }

        public void Initialize()
        {
            PlatformServices.Create();
        }

        async Task Start()
        {
            await PlatformServices.InitializeAsync();
        }
    }
}
#endif
