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
        systemMetadata
    }

    abstract class MetadataContainerEntity : IMetadataContainer
    {
        private protected readonly IAssetDataSource m_AssetDataSource;
        private protected readonly MetadataContainerSpecification m_ContainerSpecification;
        protected Func<FieldsFilter> m_BuildFieldsFilter;

        private protected Dictionary<string, MetadataValue> m_Properties;

        protected abstract OrganizationId OrganizationId { get; }

        internal IDictionary<string, MetadataValue> Properties
        {
            get => m_Properties;
            set => m_Properties = value?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, MetadataValue>();
        }

        private protected MetadataContainerEntity(IAssetDataSource assetDataSource, MetadataContainerSpecification type, Dictionary<string, MetadataValue> properties)
        {
            m_AssetDataSource = assetDataSource;
            m_ContainerSpecification = type;
            m_Properties = properties ?? new Dictionary<string, MetadataValue>();
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

            var missingKeys = new HashSet<string>(keyList);
            missingKeys.ExceptWith(m_Properties.Keys);

            var metadata = m_Properties.Where(x => keyList.Contains(x.Key));

            if (missingKeys.Count > 0 || keyList.Count == 0)
            {
                Dictionary<string, MetadataValue> missingMetadata = null;

                var filter = m_BuildFieldsFilter?.Invoke();
                IMetadataInfo data;
                switch (m_ContainerSpecification)
                {
                    case MetadataContainerSpecification.metadata:
                        filter?.MetadataFields.AddRange(missingKeys);
                        data = await GetMetadataInfoAsync(filter, cancellationToken);
                        missingMetadata = data.Metadata?.From(m_AssetDataSource, OrganizationId) ?? new Dictionary<string, MetadataValue>();
                        break;
                    case MetadataContainerSpecification.systemMetadata:
                        filter?.SystemMetadataFields.AddRange(missingKeys);
                        data = await GetMetadataInfoAsync(filter, cancellationToken);
                        missingMetadata = data.SystemMetadata?.From(m_AssetDataSource, OrganizationId) ?? new Dictionary<string, MetadataValue>();
                        break;
                }

                if (missingMetadata != null)
                    metadata = metadata.Concat(missingMetadata);
            }

            return metadata.ToDictionary(x => x.Key, x => x.Value);
        }

        protected abstract Task<IMetadataInfo> GetMetadataInfoAsync(FieldsFilter filter, CancellationToken cancellationToken);

        /// <inheritdoc />
        public Task AddOrUpdateAsync(IDictionary<string, object> metadataValues, CancellationToken cancellationToken)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var metadataValue in metadataValues)
            {
                var value = metadataValue.Value;
                if (value is MetadataObject metadataValueObject)
                {
                    value = metadataValueObject.GetValue();
                }

                value = ValidateMetadataValue(value);

                dictionary.Add(metadataValue.Key, value);
            }

            return AddOrUpdateAsync(dictionary, cancellationToken);
        }

        /// <inheritdoc />
        public Task AddOrUpdateAsync(string key, object metadataValue, CancellationToken cancellationToken)
        {
            if (metadataValue is MetadataObject metadataValueObject)
            {
                metadataValue = metadataValueObject.GetValue();
            }

            metadataValue = ValidateMetadataValue(metadataValue);

            return AddOrUpdateAsync(new Dictionary<string, object> {{key, metadataValue}}, cancellationToken);
        }

        async Task AddOrUpdateAsync(Dictionary<string, object> metadataValues, CancellationToken cancellationToken)
        {
            if (metadataValues == null || !metadataValues.Any()) return;

            await ExecuteAddOrUpdateAsync(metadataValues, cancellationToken);

            m_Properties.Clear();
        }

        protected abstract Task ExecuteAddOrUpdateAsync(Dictionary<string, object> properties, CancellationToken cancellationToken);

        /// <inheritdoc />
        public async Task RemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var keyHashSet = new HashSet<string>(keys);

            if (!keyHashSet.Any()) return;

            await DatasourceRemoveAsync(keyHashSet, cancellationToken);

            m_Properties.Clear();
        }

        /// <inheritdoc />
        public MetadataQueryBuilder Query()
        {
            return new MetadataQueryBuilder(this);
        }

        protected abstract Task DatasourceRemoveAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        static object ValidateMetadataValue(object value)
        {
            switch (value)
            {
                case bool:
                case string:
                case IEnumerable<string>:
                case double or int or float or long or short or byte or sbyte or decimal:
                    return value;
                case DateTime d:
                    return d.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                default:
                    throw new ArgumentException($"Invalid metadata value type: {value.GetType()}");
            }
        }
    }
}
