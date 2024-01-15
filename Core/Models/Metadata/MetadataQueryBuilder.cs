using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// Allows building and executing queries on a <see cref="IMetadataContainer"/>.
    /// </summary>
    public sealed class MetadataQueryBuilder
    {
        readonly MetadataContainerEntity m_MetadataContainerEntity;

        IEnumerable<string> m_Select;

        internal MetadataQueryBuilder(MetadataContainerEntity metadataContainer)
        {
            m_MetadataContainerEntity = metadataContainer;
        }

        /// <summary>
        /// Sets the query to return the metadata for the specified keys.
        /// </summary>
        /// <param name="keys">The collection of desired keys. </param>
        /// <returns>The called <see cref="MetadataQueryBuilder"/></returns>
        public MetadataQueryBuilder Select(params string[] keys)
        {
            m_Select = keys;
            return this;
        }

        /// <summary>
        /// Sets the query to return all metadata.
        /// </summary>
        /// <returns>The called <see cref="MetadataQueryBuilder"/></returns>
        public MetadataQueryBuilder SelectAll()
        {
            m_Select = null;
            return this;
        }

        /// <summary>
        /// Executes the built query.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A dictionary of metadata. </returns>
        public async Task<IReadOnlyDictionary<string, IMetadataValue>> ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var metadata = await m_MetadataContainerEntity.GetMetadataAsync(m_Select, cancellationToken);
            return metadata.ToDictionary(x => x.Key, x => (IMetadataValue)x.Value);
        }
    }
}
