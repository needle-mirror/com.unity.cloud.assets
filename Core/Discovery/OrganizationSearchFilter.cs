using System;

namespace Unity.Cloud.Assets
{
    public sealed class OrganizationSearchFilter : ComplexSearchCriteria<IOrganization>
    {
        SearchCriteria<string> Id { get; } = new(nameof(IOrganization.Id));
        NullableSearchCriteria<ulong> GenesisId { get; } = new(nameof(IOrganization.GenesisId));

        IOrganization m_Organization;

        public override string SearchKey => nameof(IAsset.Organization);
        public override Type SearchFieldType => typeof(CloudOrganization);

        private protected override Type InstantiatedType => typeof(CloudOrganization);

        private protected override bool IncludeInSearch => false;

        protected override bool IsMatch(object input)
        {
            return input switch
            {
                string => IsMatch(Id, input),
                ulong => IsMatch(GenesisId, input),
                IOrganization organization => IsMatch(Id, organization.Id) && IsMatch(GenesisId, organization.GenesisId),
                _ => false
            };
        }

        static bool IsMatch(ISearchCriteria searchCriteria, object input)
        {
            return searchCriteria.IsMatch(input);
        }

        protected override bool IsAny(object input)
        {
            return input switch
            {
                string => IsAny(Id, input),
                ulong => IsAny(GenesisId, input),
                IOrganization organization => IsAny(Id, organization.Id) && IsAny(GenesisId, organization.GenesisId),
                _ => false
            };
        }

        static bool IsAny(ISearchCriteria searchCriteria, object input)
        {
            return searchCriteria.IsAny(input);
        }

        public override void Include(IOrganization value)
        {
            base.Include(value);

            m_Organization = value;
        }

        internal IOrganization GetOrganization()
        {
            if (m_Organization != null) return m_Organization;
            if (((ISearchCriteria) this).TryGetIncluded(out var organization)) return (IOrganization) organization;
            return null;
        }
    }
}
