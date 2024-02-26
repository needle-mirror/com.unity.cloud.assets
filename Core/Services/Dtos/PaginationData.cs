using System;

namespace Unity.Cloud.Assets
{
    struct PaginationData
    {
        public string SortingField { get; set; }
        public SortingOrder SortingOrder { get; set; }
        public Range Range { get; set; }
    }
}
