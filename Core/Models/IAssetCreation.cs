namespace Unity.Cloud.Assets
{
    public interface IAssetCreation
    {
        IProject Project { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        int Version { get; set; }
        string VersionName { get; set; }
        string Type { get; set; }
    }
}
