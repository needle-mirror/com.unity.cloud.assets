using System.Collections.Generic;
using System.Linq;

namespace Unity.Cloud.Assets
{
    public sealed class HashsetSearchCriteria<U> : CollectionSearchCriteria<U, HashSet<U>>
    {
        internal HashsetSearchCriteria(string key) : base(key)
        {
        }

        protected override HashSet<U> TransformValue(object value)
        {
            return ConvertInput(value);
        }

        private protected override HashSet<U> CreateCollection()
        {
            return new HashSet<U>();
        }
    }

    public abstract class CollectionSearchCriteria<U, T> : SearchCriteria<T> where T : ICollection<U>
    {
        private protected CollectionSearchCriteria(string key) : base(key)
        {
        }

        public void Include(params U[] values)
        {
            m_Included ??= CreateCollection();
            UpdateCollection(m_Included, values);
        }

        public void Exclude(params U[] values)
        {
            m_Excluded ??= CreateCollection();
            UpdateCollection(m_Excluded, values);
        }

        public void ForAny(params U[] values)
        {
            m_Any ??= CreateCollection();
            UpdateCollection(m_Any, values);
        }

        protected override bool IsValidType(object input)
        {
            return input is null or ICollection<U>;
        }

        protected override bool IsValueEmpty(T value)
        {
            return value == null || value.Count == 0;
        }

        protected override object TransformValue(T value)
        {
            return value?.ToArray();
        }

        protected override bool SatisfiesMatch(object input)
        {
            var hashSet = ConvertInput(input);
            return (IsValueEmpty(m_Included) || hashSet.IsSupersetOf(m_Included))
                   && (IsValueEmpty(m_Excluded) || !hashSet.Overlaps(m_Excluded));
        }

        protected override bool SatisfiesAny(object input)
        {
            var hashSet = ConvertInput(input);
            return !IsValueEmpty(m_Any) && hashSet.Overlaps(m_Any);
        }

        private protected static HashSet<U> ConvertInput(object input)
        {
            return input switch
            {
                ICollection<U> collection => new HashSet<U>(collection),
                U value => new HashSet<U> {value},
                _ => new HashSet<U>()
            };
        }

        static void UpdateCollection(T collection, params U[] values)
        {
            if (values.Length == 0)
            {
                collection.Clear();
            }
            else
            {
                for (var i = 0; i < values.Length; ++i)
                {
                    collection.Add(values[i]);
                }
            }
        }

        private protected abstract T CreateCollection();
    }
}
