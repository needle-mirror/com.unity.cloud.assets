using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    enum MetadataContainerSpecification
    {
        metadata,
    }

    abstract class MetadataContainerEntity : IMetadataContainer
    {
        private protected readonly IAssetDataSource m_AssetDataSource;
        private protected readonly MetadataContainerSpecification m_ContainerSpecification;
        protected Func<FieldsFilter> m_BuildFieldsFilter;

        private protected Dictionary<string, MetadataObject> m_Properties;

        protected abstract OrganizationId OrganizationId { get; }

        internal IDictionary<string, MetadataObject> Properties
        {
            get => m_Properties;
            set => m_Properties = value?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, MetadataObject>();
        }

        private protected MetadataContainerEntity(IAssetDataSource assetDataSource, MetadataContainerSpecification type)
        {
            m_AssetDataSource = assetDataSource;
            m_ContainerSpecification = type;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Refresh()
        {
            m_Properties = null;
        }

        /// <summary>
        /// Refreshes the metadata dictionary.
        /// </summary>
        /// <param name="keys">The subset of keys to include in the dictionary; if empty or null all keys will be included. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request.</param>
        /// <returns>A task with no result.</returns>
        public async Task<Dictionary<string, MetadataValue>> GetMetadataAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var keyList = keys?.ToHashSet() ?? new HashSet<string>();

            if (m_Properties == null)
            {
                var filter = m_BuildFieldsFilter?.Invoke();
                IMetadataInfo data;
                switch (m_ContainerSpecification)
                {
                    case MetadataContainerSpecification.metadata:
                        filter?.MetadataFields.AddRange(keyList);
                        data = await GetMetadataInfoAsync(filter, cancellationToken);
                        m_Properties = data.Metadata?.From(m_AssetDataSource, OrganizationId) ?? new Dictionary<string, MetadataObject>();
                        break;
                }

                m_Properties ??= new Dictionary<string, MetadataObject>();
            }

            var metadata = keyList.Count == 0 ? m_Properties : m_Properties.Where(kvp => keyList.Contains(kvp.Key));
            return metadata.ToDictionary(kvp => kvp.Key, kvp => (MetadataValue) kvp.Value);
        }

        protected abstract Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken);

        /// <inheritdoc />
        public Task AddOrUpdateAsync(IReadOnlyDictionary<string, MetadataValue> metadataObjects, CancellationToken cancellationToken)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var kvp in metadataObjects)
            {
                var value = kvp.Value.GetValue();

                ValidateMetadataValue(value);

                dictionary.Add(kvp.Key, value);
            }

            return AddOrUpdateAsync(dictionary, cancellationToken);
        }

        /// <inheritdoc />
        public Task AddOrUpdateAsync(string key, MetadataValue metadataValue, CancellationToken cancellationToken)
        {
            var value = metadataValue.GetValue();

            ValidateMetadataValue(value);

            return AddOrUpdateAsync(new Dictionary<string, object> {{key, value}}, cancellationToken);
        }

        async Task AddOrUpdateAsync(Dictionary<string, object> metadataValues, CancellationToken cancellationToken)
        {
            if (metadataValues == null || !metadataValues.Any()) return;

            await ExecuteAddOrUpdateAsync(metadataValues, cancellationToken);

            m_Properties = null;
        }

        protected abstract Task ExecuteAddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken);

        /// <inheritdoc />
        public async Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var keyHashSet = new HashSet<string>(keys);

            if (!keyHashSet.Any()) return;

            await DatasourceRemoveAsync(keyHashSet, cancellationToken);

            m_Properties = null;
        }

        /// <inheritdoc />
        public MetadataQueryBuilder Query()
        {
            return new MetadataQueryBuilder(this);
        }

        protected abstract Task DatasourceRemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        static void ValidateMetadataValue(object value)
        {
            switch (value)
            {
                case bool:
                case string:
                case IEnumerable<string>:
                case double or int or float or long or short or byte or sbyte or decimal:
                case DateTime:
                    return;
                default:
                    throw new ArgumentException($"Invalid metadata value type: {value.GetType()}");
            }
        }
    }
}
