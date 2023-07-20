namespace Unity.Cloud.Assets
{
    /// <summary>
    /// This class contains all the information pertaining to a paged response.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PagedResponse<T> : IPagedResponse<T>
    {
        /// <inheritdoc/>
        public T[] Elements { get; }

        /// <inheritdoc/>
        public int PageEndIndex { get; set; }

        /// <inheritdoc/>
        public Pagination Pagination { get; protected set; }

        /// <inheritdoc/>
        public IPagedResponse<T> PreviousPage { get; protected set; }

        /// <inheritdoc/>
        public string NextPageToken { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResponse{T}"/> class.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="nextPageToken"></param>
        /// <param name="previousPage"></param>
        /// <param name="pageEndIndex"></param>
        protected PagedResponse(T[] elements, string nextPageToken, IPagedResponse<T> previousPage = null, int pageEndIndex = -1)
        {
            NextPageToken = nextPageToken;

            PreviousPage = previousPage;
            Elements = elements;

            Pagination = previousPage?.Pagination ?? default(Pagination);
            PageEndIndex = pageEndIndex >= 0
                ? pageEndIndex
                : (previousPage?.PageEndIndex ?? -1) + elements.Length;
        }
    }
}
