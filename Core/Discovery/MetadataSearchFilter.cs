using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    public sealed class MetadataSearchFilter : ISearchCriteria
    {
        readonly string m_PropertyName;
        readonly string m_SearchKey;
        readonly Dictionary<string, ISearchCriteria> m_Values = new();

        delegate bool TryGetValueDelegate(ISearchCriteria criterion, out object value);

        /// <inheritdoc/>
        string ISearchCriteria.PropertyName => m_PropertyName;

        /// <inheritdoc/>
        Type ISearchCriteria.SearchFieldType => typeof(IMetadataValue);

        public MetadataSearchFilter(string propertyName, string searchKey)
        {
            m_PropertyName = propertyName;
            m_SearchKey = searchKey;
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetIncluded(out object includedValue)
        {
            return TryGetValue(out includedValue,
                (ISearchCriteria criterion, out object value) => criterion.TryGetIncluded(out value));
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetExcluded(out object excludedValue)
        {
            return TryGetValue(out excludedValue,
                (ISearchCriteria criterion, out object value) => criterion.TryGetExcluded(out value));
        }

        /// <inheritdoc/>
        bool ISearchCriteria.TryGetAny(out object anyValue)
        {
            return TryGetValue(out anyValue,
                (ISearchCriteria criterion, out object value) => criterion.TryGetAny(out value));
        }

        bool TryGetValue(out object outValue, TryGetValueDelegate getValue)
        {
            var kvps = new Dictionary<string, object>();
            foreach (var kvp in m_Values)
            {
                if (getValue(kvp.Value, out var value))
                {
                    kvps.Add($"{m_SearchKey}.{kvp.Key}", value);
                }
            }

            outValue = new JsonObject(kvps);
            return kvps.Count > 0;
        }

        /// <inheritdoc/>
        void ISearchCriteria.Include(object value)
        {
            TryAdd(value, Include);
        }

        /// <inheritdoc/>
        void ISearchCriteria.Include(Dictionary<string, object> includedValues, string prefix)
        {
            AddValues(includedValues, prefix,
                (ISearchCriteria criterion, out object value) => criterion.TryGetIncluded(out value));
        }

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(object value)
        {
            TryAdd(value, Exclude);
        }

        /// <inheritdoc/>
        void ISearchCriteria.Exclude(Dictionary<string, object> excludedValues, string prefix)
        {
            AddValues(excludedValues, prefix,
                (ISearchCriteria criterion, out object value) => criterion.TryGetExcluded(out value));
        }

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(object value)
        {
            TryAdd(value, ForAny);
        }

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(Dictionary<string, object> forAnyValues, string prefix)
        {
            AddValues(forAnyValues, prefix,
                (ISearchCriteria criterion, out object value) => criterion.TryGetAny(out value));
        }

        static void TryAdd(object value, Action<string, object> add)
        {
            switch (value)
            {
                case null:
                    break;
                case KeyValuePair<string, object> kvp:
                    add(kvp.Key, kvp.Value);
                    break;
                case KeyValuePair<string, IMetadataValue> metadataKvp:
                    add(metadataKvp.Key, metadataKvp.Value.ToObject());
                    break;
                case IEnumerable<KeyValuePair<string, object>> kvps:
                    foreach (var kvp in kvps)
                    {
                        add(kvp.Key, kvp.Value);
                    }
                    break;
                case IEnumerable<KeyValuePair<string, IMetadataValue>> metadataKvps:
                    foreach (var kvp in metadataKvps)
                    {
                        add(kvp.Key, kvp.Value.ToObject());
                    }
                    break;
                default:
                    throw new InvalidArgumentException($"{nameof(MetadataSearchFilter)} can only filter KeyValuePairs of string and object/IMetadata .");
            }
        }

        void AddValues(IDictionary<string, object> values, string prefix, TryGetValueDelegate getValue)
        {
            var searchKey = m_SearchKey.BuildSearchKey(prefix);
            foreach (var kvp in m_Values)
            {
                if (getValue(kvp.Value, out var value))
                {
                    values.Add($"{searchKey}.{kvp.Key}", value);
                }
            }
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsMatch(object input)
        {
            if (IsEmpty()) return true;

            return input switch
            {
                null => IsEmpty(),
                KeyValuePair<string, object> kvp when m_Values.TryGetValue(kvp.Key, out var value) => value.IsMatch(kvp.Value),
                KeyValuePair<string, IMetadataValue> metadataKvp when m_Values.TryGetValue(metadataKvp.Key, out var value) => value.IsMatch(metadataKvp.Value.ToObject()),
                IEnumerable<KeyValuePair<string, object>> kvps => !kvps.Any() || kvps.All(x => (this as ISearchCriteria).IsMatch(x)),
                IEnumerable<KeyValuePair<string, IMetadataValue>> metadataKvps => !metadataKvps.Any() || metadataKvps.All(x => (this as ISearchCriteria).IsMatch(x)),
                _ => IsEmpty()
            };
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsAny(object input)
        {
            return input switch
            {
                KeyValuePair<string, object> kvp when m_Values.TryGetValue(kvp.Key, out var value) => value.IsAny(kvp.Value),
                KeyValuePair<string, IMetadataValue> metadataKvp when m_Values.TryGetValue(metadataKvp.Key, out var value) => value.IsAny(metadataKvp.Value.ToObject()),
                IEnumerable<KeyValuePair<string, object>> kvps => kvps.Any(x => (this as ISearchCriteria).IsAny(x)),
                IEnumerable<KeyValuePair<string, IMetadataValue>> metadataKvps => metadataKvps.Any(x => (this as ISearchCriteria).IsAny(x)),
                _ => false
            };
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsEmpty() => IsEmpty();

        /// <inheritdoc/>
        public void Clear()
        {
            m_Values.Clear();
        }

        public void Include(string metadataFieldName, object value)
        {
            var criterion = GetCriteria(metadataFieldName);
            criterion.Include(value);
        }

        public void Exclude(string metadataFieldName, object value)
        {
            var criterion = GetCriteria(metadataFieldName);
            criterion.Exclude(value);
        }

        public void ForAny(string metadataFieldName, object value)
        {
            var criterion = GetCriteria(metadataFieldName);
            criterion.ForAny(value);
        }

        ISearchCriteria GetCriteria(string key)
        {
            if (!m_Values.TryGetValue(key, out var criterion))
            {
                criterion = new SearchCriteria<object>(key, key);
                m_Values.Add(key, criterion);
            }

            return criterion;
        }

        bool IsEmpty()
        {
            return m_Values.Count == 0 || m_Values.Values.Any(x => !x.IsEmpty());
        }
    }
}
