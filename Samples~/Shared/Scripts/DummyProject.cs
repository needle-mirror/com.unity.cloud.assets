using System.Collections.Generic;

namespace Unity.Cloud.Assets.Samples
{
    class DummyProject : IProject
    {
        public IOrganization Organization { get; }
        public string Id { get; set; }
        public string Name { get; set; }
        public IReadOnlyDictionary<string, IDeserializable> Metadata { get; }
        public IReadOnlyCollection<string> StorageIds { get; }
        public IProject.ProjectStatus Status { get; set; }
        public int UserCount { get; set; }
    }
}
