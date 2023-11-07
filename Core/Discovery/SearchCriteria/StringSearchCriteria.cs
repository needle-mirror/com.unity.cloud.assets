using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A flexible string search: splits the string on whitespaces and performs a <see cref="string.Contains(string)"/> comparisons instead of equalities.
    /// </summary>
    public sealed class StringSearchCriteria : SearchCriteria<string>
    {
        internal StringSearchCriteria(string propertyName, string searchKey) : base(propertyName, searchKey)
        {
        }

        protected override bool SatisfiesMatch(object input)
        {
            return (IsValueEmpty(m_Included) || ContainsAll(m_Included, input))
                   && (IsValueEmpty(m_Excluded) || !ContainsAll(m_Excluded, input));
        }

        protected override bool SatisfiesAny(object input)
        {
            return !IsValueEmpty(m_Any) && ContainsAny(m_Any, input);
        }

        static bool ContainsAll(string criteria, object input)
        {
            if (input == null) return false;

            var sInput = input.ToString();
            var split = criteria.Split(' ');
            for (var i = 0; i < split.Length; ++i)
            {
                if (!sInput.Contains(split[i])) return false;
            }

            return true;
        }

        static bool ContainsAny(string criteria, object input)
        {
            if (input != null)
            {
                var sInput = input.ToString().ToUpper();

                var split = criteria.Split(' ');
                for (var i = 0; i < split.Length; ++i)
                {
                    if (sInput.Contains(split[i].ToUpper())) return true;
                }
            }

            return false;
        }
    }
}
