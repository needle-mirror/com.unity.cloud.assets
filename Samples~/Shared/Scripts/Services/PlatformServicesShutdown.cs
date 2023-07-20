#if !UC_EXCLUDE_SAMPLES
using UnityEngine;

namespace Unity.Cloud.Assets.Samples
{
    /// <summary>
    /// A Monobehaviour class to shut down services and dependencies from the Unity Cloud platform.
    /// </summary>
    [DefaultExecutionOrder(int.MaxValue)]
    [AddComponentMenu("Assets/Samples/Platform Services Shutdown")]
    public class PlatformServicesShutdown : MonoBehaviour
    {
        void OnDestroy()
        {
            PlatformServices.ShutDownServices();
        }
    }
}
#endif
