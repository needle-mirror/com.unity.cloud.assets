namespace Unity.Cloud.Assets
{
    /// <summary>
    /// An interface that represents a paged response of T
    /// </summary>
    /// <typeparam name="T">Type contained in the paged response</typeparam>
    public interface IPagedResponse<T>
    {
        /// <summary>
        /// Implement this property to return a collection of <typeparam name="T"/> elements.
        /// </summary>
        T[] Elements { get; }

        /// <summary>
        /// Implement this property to return the search index of the last result on this page.
        /// </summary>
        int PageEndIndex { get; }

        /// <summary>
        /// Implement this property to return the page format.
        /// </summary>
        Pagination Pagination { get; }

        /// <summary>
        /// Implement this property to return the previous <see cref="U"/>.
        /// </summary>
        IPagedResponse<T> PreviousPage { get; }

        /// <summary>
        /// Implement this property to return the token for retrieving the next <see cref="U"/>.
        /// </summary>
        string NextPageToken { get; }
    }
}
