using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A simple string search but wrapped in a type.
    /// </summary>
    public sealed class DeserializableSearchCriteria : SearchCriteria<string>
    {
        internal DeserializableSearchCriteria(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }

        protected override bool IsValidType(object input)
        {
            return input is IDeserializable || base.IsValidType(input);
        }

        protected override object TransformValue(string value)
        {
            return new JsonObject(value);
        }

        protected override string TransformValue(object value)
        {
            return value is IDeserializable deserializable ? deserializable.GetAsString() : base.TransformValue(value);
        }

        protected override bool SatisfiesMatch(object input)
        {
            if (input is IDeserializable deserializable)
            {
                return base.SatisfiesMatch(deserializable.GetAsString());
            }
            return base.SatisfiesMatch(input);
        }

        protected override bool SatisfiesAny(object input)
        {
            if (input is IDeserializable deserializable)
            {
                return base.SatisfiesAny(deserializable.GetAsString());
            }
            return base.SatisfiesAny(input);
        }
    }
}
