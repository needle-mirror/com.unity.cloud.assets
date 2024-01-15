using UnityEngine;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    /// <summary>
    /// A class used to inject services and dependencies in the Asset Discovery sample.
    /// </summary>
    public class PlatformServicesInstaller : MonoBehaviour
    {
        [SerializeField]
        OrganizationController m_OrganizationController;

        void Awake()
        {
            m_OrganizationController.SetServices(PlatformServices.Authenticator, PlatformServices.AssetRepository, PlatformServices.OrganizationRepository);
        }
    }
}
