using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class lists the cloud asset collections belonging to a project.
    /// </summary>
    class AssetCollectionListDto
    {
        [JsonProperty("collections")]
        public AssetCollection[] Collections { get; set; }
    }
}
