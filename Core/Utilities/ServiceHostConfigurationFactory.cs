using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Internalizes and unifies the creation of <see cref="IServiceHttpClient"/> and configurations for all asset providers and controllers.
    /// </summary>
    static class ServiceHostConfigurationFactory
    {
        static AssetHostConfiguration m_AssetHostConfiguration;

        /// <summary>
        /// Creates an <see cref="AssetHostConfiguration"/> if one does not exist.
        /// </summary>
        /// <param name="serviceHostResolver">The default service environment setting</param>
        /// <returns>An <see cref="AssetHostConfiguration"/>. </returns>
        internal static AssetHostConfiguration Create(IServiceHostResolver serviceHostResolver)
        {
            // Resolve the environment of the original configuration.
            var serviceEnvironment = serviceHostResolver.GetResolvedEnvironment();

            if (m_AssetHostConfiguration != null && serviceEnvironment != m_AssetHostConfiguration.ServiceEnvironment)
            {
                m_AssetHostConfiguration = null;
            }

            // Use the resolved environment to create an instance of the custom configuration for assets.
            m_AssetHostConfiguration ??= new AssetHostConfiguration(serviceEnvironment);
            return m_AssetHostConfiguration;
        }
    }
}
