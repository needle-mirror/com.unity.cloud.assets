using Newtonsoft.Json;

namespace Unity.Cloud.Assets
{
    struct UserAndOrganizationsDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("genesisId")]
        public string GenesisId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("organizations")]
        public CloudOrganization[] Organizations { get; set; }
    }
}
