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
        Type ISearchCriteria.SearchFieldType => typeof(IDeserializable);

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
            switch (value)
            {
                case null:
                    break;
                case KeyValuePair<string, object> kvp:
                    Include(kvp.Key, kvp.Value);
                    break;
                case IDeserializable deserializable:
                    var valueDictionary = deserializable.GetAs<Dictionary<string,object>>();
                    foreach (var kvp in valueDictionary)
                    {
                        Include(kvp.Key, kvp.Value);
                    }
                    break;
                default:
                    throw new InvalidArgumentException("MetadataSearchFilter can only filter KeyValuePair<string, object> or IDeserializable.");
            }
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
            switch (value)
            {
                case null:
                    break;
                case KeyValuePair<string, object> kvp:
                    Exclude(kvp.Key, kvp.Value);
                    break;
                case IDeserializable deserializable:
                    var valueDictionary = deserializable.GetAs<Dictionary<string,object>>();
                    foreach (var kvp in valueDictionary)
                    {
                        Exclude(kvp.Key, kvp.Value);
                    }
                    break;
                default:
                    throw new InvalidArgumentException("MetadataSearchFilter can only filter KeyValuePair<string, object> or IDeserializable.");
            }
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
            switch (value)
            {
                case null:
                    break;
                case KeyValuePair<string, object> kvp:
                    ForAny(kvp.Key, kvp.Value);
                    break;
                case IDeserializable deserializable:
                    var valueDictionary = deserializable.GetAs<Dictionary<string,object>>();
                    foreach (var kvp in valueDictionary)
                    {
                        ForAny(kvp.Key, kvp.Value);
                    }
                    break;
                default:
                    throw new InvalidArgumentException("MetadataSearchFilter can only filter KeyValuePair<string, object> or IDeserializable.");
            }
        }

        /// <inheritdoc/>
        void ISearchCriteria.ForAny(Dictionary<string, object> forAnyValues, string prefix)
        {
            AddValues(forAnyValues, prefix,
                (ISearchCriteria criterion, out object value) => criterion.TryGetAny(out value));
        }

        void AddValues(IDictionary<string, object> values, string prefix, TryGetValueDelegate getValue)
        {
            var searchKey = m_SearchKey.BuildSearchKey(prefix);
            foreach (var kvp in m_Values)
            {
                if (getValue(kvp.Value, out var value))
                {
                    values.Add($"{searchKey}.{kvp.Key}", value.ToString());
                }
            }
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsMatch(object input)
        {
            return input switch
            {
                null => IsEmpty(),
                KeyValuePair<string, object> kvp when m_Values.TryGetValue(kvp.Key, out var value) => value.IsMatch(kvp.Value),
                _ => IsEmpty()
            };
        }

        /// <inheritdoc/>
        bool ISearchCriteria.IsAny(object input)
        {
            if (input is KeyValuePair<string, object> kvp && m_Values.TryGetValue(kvp.Key, out var value))
            {
                return value.IsAny(kvp.Value);
            }

            return false;
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
                criterion = new SearchCriteria<string>(key, key);
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
