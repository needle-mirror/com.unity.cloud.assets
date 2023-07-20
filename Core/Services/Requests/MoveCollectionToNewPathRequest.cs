using System.Net.Http;
using System.Text;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Move a collection to a new path request.
    /// </summary>
    class MoveCollectionToNewPathRequest : AssetRequest
    {
        /// <summary>
        /// The path to the collection.
        /// </summary>
        public string CollectionPath { get; }

        /// <summary>
        /// The new path to the collection.
        /// </summary>
        public string NewCollectionPath { get; }

        /// <summary>
        /// Move a collection request.
        /// </summary>
        /// <param name="organizationId">Genesis ID of the organization</param>
        /// <param name="projectId">ID of the project</param>
        /// <param name="collectionPath">The path to the collection</param>
        /// <param name="newCollectionPath">The new path to the collection</param>
        /// <param name="xCorrelationId">Correlation id of the request.</param>
        public MoveCollectionToNewPathRequest(ulong organizationId, string projectId, string collectionPath, string newCollectionPath, string xCorrelationId = default(string))
            : base(organizationId, projectId, xCorrelationId)
        {
            CollectionPath = collectionPath;
            NewCollectionPath = newCollectionPath;

            m_PathAndQueryParams += $"/collections/{collectionPath}/move";
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
