using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A simple string search but wrapped in a type.
    /// </summary>
    public sealed class IdSearchCriteria<T> : SearchCriteria<string> where T : new()
    {
        internal IdSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        protected override bool IsValidType(object input)
        {
            return input is T || base.IsValidType(input);
        }

        protected override object TransformValue(string value)
        {
            return string.IsNullOrEmpty(value) ? new T() : Activator.CreateInstance(typeof(T), value);
        }

        protected override string TransformValue(object value)
        {
            return value is T ? value.ToString() : base.TransformValue(value);
        }

        protected override bool SatisfiesMatch(object input)
        {
            return base.SatisfiesMatch(input.ToString());
        }

        protected override bool SatisfiesAny(object input)
        {
            return base.SatisfiesAny(input.ToString());
        }
    }
}
