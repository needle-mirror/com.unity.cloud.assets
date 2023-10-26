using System;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A client for making requests to Assets Manager API endpoints in the UCF gateway.
    /// </summary>
    sealed class AssetHostConfiguration
    {
        public ServiceEnvironment ServiceEnvironment { get; }

        /// <summary>
        /// Creates an instance of <see cref="AssetHostConfiguration"/>.
        /// </summary>
        /// <param name="serviceEnvironment">The service environment to target for requests.</param>
        internal AssetHostConfiguration(ServiceEnvironment serviceEnvironment = ServiceEnvironment.Production)
        {
            ServiceEnvironment = serviceEnvironment;
        }

        /// <summary>
        /// Returns the service address of the Assets Manager API
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetServiceAddress()
        {
            const string domain = "services.unity.com";

            return GetServiceAddress(domain, ServiceEnvironment);
        }

        static string GetServiceAddress(string domain, ServiceEnvironment serviceEnvironment)
        {
            return serviceEnvironment switch
            {
                ServiceEnvironment.Production => $"https://{domain}",
                ServiceEnvironment.Staging => $"https://staging.{domain}",
                ServiceEnvironment.Test => $"https://staging.{domain}",
                _ => throw new ArgumentOutOfRangeException(nameof(serviceEnvironment), serviceEnvironment, $"Invalid environment for {nameof(GetServiceAddress)}")
            };
        }
    }
}
