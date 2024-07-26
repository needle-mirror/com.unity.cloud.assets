namespace Unity.Cloud.Assets
{
    internal readonly struct StatusPredicate
    {
        public string Id { get; }
        public string Name { get; }

        internal StatusPredicate(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
