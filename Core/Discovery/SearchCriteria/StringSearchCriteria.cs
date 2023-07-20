using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A flexible string search; splits the string on whitespaces and performs a <see cref="string.Contains(string)"/> comparisons instead of equalities.
    /// </summary>
    [Serializable]
    public sealed class StringSearchCriteria : SearchCriteria<string>
    {
        internal StringSearchCriteria(string key) : base(key)
        {
        }

        protected override bool SatisfiesMatch(object input)
        {
            return (IsValueEmpty(m_Included) || input != null && ContainsAll(m_Included, input.ToString()))
                   && (IsValueEmpty(m_Excluded) || input != null && !ContainsAll(m_Excluded, input.ToString()));
        }

        protected override bool SatisfiesAny(object input)
        {
            return !IsValueEmpty(m_Any) && ContainsAny(m_Any, input.ToString());
        }

        bool ContainsAll(string criteria, string input)
        {
            var split = criteria.Split(' ');
            for (var i = 0; i < split.Length; ++i)
            {
                if (!input.Contains(split[i])) return false;
            }

            return true;
        }

        bool ContainsAny(string criteria, string input)
        {
            input = input.ToUpper();

            var split = criteria.Split(' ');
            for (var i = 0; i < split.Length; ++i)
            {
                if (input.Contains(split[i].ToUpper())) return true;
            }

            return false;
        }
    }
}
