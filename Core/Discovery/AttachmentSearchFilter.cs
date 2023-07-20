using System;

namespace Unity.Cloud.Assets
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="Unity.Cloud.Assets.IAssetAttachement"/> search request.
    /// </summary>
    public class AttachmentSearchFilter : FileSearchFilter
    {
        public override string SearchKey => nameof(IAsset.Attachments);
    }
}
