using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="AssetAuthor"/> search request.
    /// </summary>
    public class AuthorSearchFilter : ComplexSearchCriteria<AssetAuthor>
    {
        /// <inheritdoc cref="AssetAuthor.Name"/>
        public SearchCriteria<string> Name { get; } = new(nameof(AssetAuthor.Name));
        /// <inheritdoc cref="AssetAuthor.Company"/>
        public SearchCriteria<string> Company { get; } = new(nameof(AssetAuthor.Company));
        /// <inheritdoc cref="AssetAuthor.EmailAddress"/>
        public SearchCriteria<string> EmailAddress { get; } = new(nameof(AssetAuthor.EmailAddress));
        /// <inheritdoc cref="AssetAuthor.Details"/>
        public SearchCriteria<string> Details { get; } = new(nameof(AssetAuthor.Details));

        public override string SearchKey => nameof(IAsset.Author);
    }
}
