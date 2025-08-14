using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Base class for api requests on assets.
    /// </summary>
    class LibraryRequest : ApiRequest
    {
        /// <summary>
        /// Creates an instance of a <see cref="LibraryRequest"/>.
        /// </summary>
        /// <param name="assetLibraryId">ID of the library.</param>
        protected LibraryRequest(AssetLibraryId assetLibraryId)
        {
            m_RequestUrl = $"/libraries/{assetLibraryId}";
        }

        LibraryRequest()
        {
            m_RequestUrl = "/libraries";
        }

        /// <summary>
        /// Creates an instance of a <see cref="LibraryRequest"/> for listing libraries.
        /// </summary>
        /// <param name="offset">The amount of entities to skip.</param>
        /// <param name="limit">The page size.</param>
        public static LibraryRequest ListLibrariesRequest(int? offset, int? limit)
        {
            var request = new LibraryRequest();
            request.AddParamToQuery("IncludeFields", "hasCollection");
            request.AddParamToQuery("offset", offset?.ToString());
            request.AddParamToQuery("limit", limit?.ToString());
            return request;
        }

        /// <summary>
        /// Creates an instance of a <see cref="LibraryRequest"/> for a library.
        /// </summary>
        /// <param name="assetLibraryId">ID of the library.</param>
        public static LibraryRequest GetLibraryRequest(AssetLibraryId assetLibraryId)
        {
            var request = new LibraryRequest(assetLibraryId);
            request.AddParamToQuery("IncludeFields", "hasCollection");
            return request;
        }
        
        /// <summary>
        /// Creates an instance of a <see cref="LibraryRequest"/> for a library.
        /// </summary>
        /// <param name="assetLibraryId"></param>
        /// <returns></returns>
        public static LibraryRequest GetAssetCountRequest(AssetLibraryId assetLibraryId)
        {
            var projectRequest = new LibraryRequest(assetLibraryId);
            projectRequest.m_RequestUrl += "/assets/count";
            return projectRequest;
        }
    }
}
