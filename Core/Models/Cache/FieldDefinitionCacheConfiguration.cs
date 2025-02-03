namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Sets the cache configuration for a field definition.
    /// </summary>
    public struct FieldDefinitionCacheConfiguration
    {
        /// <summary>
        /// Whether to cache the properties of the field definition.
        /// </summary>
        public bool CacheProperties { get; set; }

        public static FieldDefinitionCacheConfiguration NoCaching => new()
        {
            CacheProperties = false,
        };

        internal static FieldDefinitionCacheConfiguration Legacy => new()
        {
            CacheProperties = true,
        };

        internal bool HasCachingRequirements => CacheProperties;

        internal FieldDefinitionCacheConfiguration(AssetRepositoryCacheConfiguration defaultCacheConfiguration)
        {
            CacheProperties = defaultCacheConfiguration.FieldDefinitionCacheConfiguration.CacheProperties;
        }
    }
}
