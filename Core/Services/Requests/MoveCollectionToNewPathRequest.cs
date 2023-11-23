using System.Net.Http;
using System.Text;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Move a collection to a new path request.
    /// </summary>
    class MoveCollectionToNewPathRequest : CollectionRequest
    {
        /// <summary>
        /// The new path to the collection.
        /// </summary>
        public string NewCollectionPath { get; }

        /// <summary>
        /// Move a collection request.
        /// </summary>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="newCollectionPath">The new path to the collection</param>
        public MoveCollectionToNewPathRequest(ProjectId projectId, CollectionPath collectionPath, string newCollectionPath)
            : base(projectId, collectionPath)
        {
            NewCollectionPath = newCollectionPath;

            m_PathAndQueryParams += $"/move";
        }

        /// <summary>
        /// Helper for constructing the request body.
        /// </summary>
        /// <returns>A </returns>
        public override HttpContent ConstructBody()
        {
            var body = $"{{\"destinationParentPath\":\"{NewCollectionPath}\"}}";

            return new StringContent(body, Encoding.UTF8, "application/json");
        }
    }
}
