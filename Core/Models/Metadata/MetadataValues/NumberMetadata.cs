using System;

namespace Unity.Cloud.Assets
{
    public sealed class NumberMetadata : MetadataValue
    {
        public double Value { get; set; }

        public NumberMetadata(double value = default)
            : base(MetadataValueType.Number)
        {
            Value = value;
        }

        internal NumberMetadata(MetadataValueType valueType, object value)
            : base(valueType, value)
        {
            Value = value as double? ?? double.Parse(value?.ToString() ?? "0");
        }

        internal override object GetValue()
        {
            return Value;
        }
    }
}
