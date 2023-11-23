namespace Unity.Cloud.Assets
{
    public interface IAssetProjectCreation
    {
        string Name { get; }
        IDeserializable Metadata { get; }
    }
}
