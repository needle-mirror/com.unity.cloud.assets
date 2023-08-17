namespace Unity.Cloud.Assets
{
    public class AssetCreation : IAssetCreation
    {
        public IProject Project { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public string VersionName { get; set; }
        public string Type { get; set; }
    }
}
