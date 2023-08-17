namespace Unity.Cloud.Assets
{
    public sealed class AssetServiceConfiguration
    {
        public bool IsDiscovery { get; set; }

        public AssetServiceConfiguration(bool isDiscovery = false)
        {
            IsDiscovery = isDiscovery;
        }
    }
}
