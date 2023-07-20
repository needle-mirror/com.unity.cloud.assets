using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface for an access token provider for the Assets SDK Unity Cloud platform.
    /// </summary>
    public interface IAssetsAccessTokenProvider : IAccessTokenProvider
    {
        /// <summary>
        /// Gets the service environment setting.
        /// </summary>
        ServiceEnvironment ServiceEnvironmentSetting { get; }
    }
}
