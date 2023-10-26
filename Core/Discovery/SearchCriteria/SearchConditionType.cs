using System;

namespace Unity.Cloud.Assets
{
    public readonly struct SearchConditionType : IEquatable<SearchConditionType>, IEquatable<string>
    {
        public static readonly SearchConditionType GreaterThanOrEqual = new("greaterThanOrEqual");
        public static readonly SearchConditionType LessThan = new("lessThan");

        readonly string m_Value;

        SearchConditionType(string value)
        {
            m_Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is SearchConditionType other && Equals(other);
        }

        public bool Equals(string str)
        {
            return m_Value.Equals(str);
        }

        public bool Equals(SearchConditionType other)
        {
            return Equals(other.m_Value);
        }

        public override int GetHashCode()
        {
            return m_Value != null ? m_Value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return m_Value;
        }

        public static bool operator ==(SearchConditionType a, SearchConditionType b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SearchConditionType a, SearchConditionType b)
        {
            return !(a == b);
        }

        public static implicit operator string(SearchConditionType a) => a.m_Value;

        public static implicit operator SearchConditionType(string a) => new(a);
    }
}
