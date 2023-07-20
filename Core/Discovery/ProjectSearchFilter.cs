using System;

namespace Unity.Cloud.Assets
{
    public sealed class ProjectSearchFilter : ComplexSearchCriteria<IProject>
    {
        SearchCriteria<string> Id { get; } = new(nameof(IProject.Id));

        IProject m_Project;

        public override string SearchKey => nameof(IAsset.Project);
        public override Type SearchFieldType => typeof(CloudProject);

        private protected override Type InstantiatedType => typeof(CloudProject);

        private protected override bool IncludeInSearch => false;

        protected override bool IsMatch(object input)
        {
            return input switch
            {
                string => IsMatch(Id, input),
                IProject project => IsMatch(Id, project.Id),
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
                IProject project => IsAny(Id, project.Id),
                _ => false
            };
        }

        static bool IsAny(ISearchCriteria searchCriteria, object input)
        {
            return searchCriteria.IsAny(input);
        }

        public override void Include(IProject value)
        {
            base.Include(value);

            m_Project = value;
        }

        internal IProject GetProject()
        {
            if (m_Project != null) return m_Project;
            if (((ISearchCriteria) this).TryGetIncluded(out var project)) return (IProject) project;
            return null;
        }
    }
}
