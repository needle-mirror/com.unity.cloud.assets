namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Sets the cache configuration for an asset collection.
    /// </summary>
    public struct AssetCollectionCacheConfiguration
    {
        /// <summary>
        /// Whether to cache the properties of the asset collection.
        /// </summary>
        public bool CacheProperties { get; set; }

        public static AssetCollectionCacheConfiguration NoCaching => new()
        {
            CacheProperties = false,
        };

        internal static AssetCollectionCacheConfiguration Legacy => new()
        {
            CacheProperties = true,
        };

        internal bool HasCachingRequirements => CacheProperties;

        internal AssetCollectionCacheConfiguration(AssetRepositoryCacheConfiguration defaultCacheConfiguration)
        {
            CacheProperties = defaultCacheConfiguration.AssetCollectionCacheConfiguration.CacheProperties;
        }
    }
}
