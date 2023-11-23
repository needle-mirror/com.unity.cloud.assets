namespace Unity.Cloud.Assets
{
    public class AssetProjectCreation : IAssetProjectCreation
    {
        public string Name { get; set; }
        public IDeserializable Metadata { get; set; }
    }
}
