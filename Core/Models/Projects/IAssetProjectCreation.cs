namespace Unity.Cloud.Assets
{
    public interface IAssetProjectCreation
    {
        /// <inheritdoc cref="IAssetProject.Name"/>
        string Name { get; }

        /// <inheritdoc cref="IAssetProject.Metadata"/>
        IDeserializable Metadata { get; }
    }
}
