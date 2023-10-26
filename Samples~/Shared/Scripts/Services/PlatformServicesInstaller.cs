#if !UC_EXCLUDE_SAMPLES
using UnityEngine;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    /// <summary>
    /// A class used to inject services and dependencies in the Asset Discovery sample.
    /// </summary>
    public class PlatformServicesInstaller : MonoBehaviour
    {
        [SerializeField]
        UserController m_UserController;

        void Awake()
        {
            m_UserController.SetServices(PlatformServices.Authenticator, PlatformServices.AssetRepository, PlatformServices.OrganizationRepository);
        }
    }
}
#endif
