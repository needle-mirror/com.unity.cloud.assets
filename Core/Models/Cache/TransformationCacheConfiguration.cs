namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Sets the cache configuration for a transformation.
    /// </summary>
    public struct TransformationCacheConfiguration
    {
        /// <summary>
        /// Whether to cache the properties of the transformation.
        /// </summary>
        public bool CacheProperties { get; set; }

        public static TransformationCacheConfiguration NoCaching => new()
        {
            CacheProperties = false,
        };

        internal static TransformationCacheConfiguration Legacy => new()
        {
            CacheProperties = true,
        };

        internal bool HasCachingRequirements => CacheProperties;

        internal TransformationCacheConfiguration(AssetRepositoryCacheConfiguration defaultCacheConfiguration)
        {
            CacheProperties = defaultCacheConfiguration.TransformationCacheConfiguration.CacheProperties;
        }
    }
}
