using System;
using System.Text.RegularExpressions;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A class for manipulating a url metadata value.
    /// </summary>
    public sealed class UrlMetadata : MetadataObject
    {
        /// <summary>
        /// The url value of a metadata field.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The label value of a metadata field.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <inheritdoc />
        public override object GetValue()
        {
            return string.IsNullOrEmpty(Label) ? Uri.ToString() : $"[{Label}]({Uri})";
        }

        /// <inheritdoc />
        internal override void SetValue(object value)
        {
            if (value != null && TryParse(value.ToString(), out var uri, out var label))
            {
                Uri = uri;
                Label = label;
            }
            else
            {
                throw new FormatException($"Cannot convert {value} to url.");
            }
        }

        internal static bool TryParse(string value, out Uri uri, out string label)
        {
            label = string.Empty;
            if (Uri.TryCreate(value, UriKind.Absolute, out uri))
            {
                return true;
            }

            const string pattern = @"\[(?<label>.*)\]\((?<url>.*)\)";
            var rgx = new Regex(pattern, RegexOptions.Singleline, TimeSpan.FromSeconds(1));
            var match = rgx.Match(value);
            if (match.Success)
            {
                label = match.Groups["label"].Value;
                return Uri.TryCreate(match.Groups["url"].Value, UriKind.Absolute, out uri);
            }

            return false;
        }
    }
}
