namespace Unity.Cloud.Assets
{
    interface ISearchValue
    {
        public string Type { get; }

        public bool IsEmpty();
        public bool Overlaps(ISearchValue other) => false;
    }
}
