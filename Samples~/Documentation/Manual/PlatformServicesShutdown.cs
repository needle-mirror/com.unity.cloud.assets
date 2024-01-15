namespace Unity.Cloud.Documentation.Assets
{
    #region PlatformServices_Shutdown

    using UnityEngine;

    /// <summary>
    /// A Mono behaviour class to shut down services and dependencies from the Unity Cloud platform.
    /// </summary>
    [DefaultExecutionOrder(int.MaxValue)]
    [AddComponentMenu("Assets/Manual/Platform Services Shutdown")]
    public class PlatformServicesShutdown : MonoBehaviour
    {
        void OnDestroy()
        {
            PlatformServices.ShutDownServices();
        }
    }

    #endregion
}
