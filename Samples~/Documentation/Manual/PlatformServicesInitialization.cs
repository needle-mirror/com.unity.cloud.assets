namespace Unity.Cloud.Documentation.Assets
{
    #region PlatformServices_Initialization

    using System.Threading.Tasks;
    using UnityEngine;

    /// <summary>
    /// A Mono behaviour class to initialize services and dependencies for the Unity Cloud platform.
    /// </summary>
    [DefaultExecutionOrder(int.MinValue)]
    [AddComponentMenu("Assets/Manual/Platform Services Initialization")]
    public class PlatformServicesInitialization : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            PlatformServices.Create();
        }

        async Task Start()
        {
            await PlatformServices.InitializeAsync();
        }
    }

    #endregion
}
