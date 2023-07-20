#if !UC_EXCLUDE_SAMPLES
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDiscovery
{
    class ProjectsListController
    {
        List<IProject> m_ProjectsList;
        ListView m_ProjectsListView;
        VisualTreeAsset m_ProjectsListItemTemplate;

        internal void Init(ListView projectsListView, IProject[] projects, VisualTreeAsset projectItemTemplate)
        {
            m_ProjectsListView = projectsListView;
            m_ProjectsListItemTemplate = projectItemTemplate;

            m_ProjectsList = new List<IProject>(projects.Length);
            foreach (var proj in projects)
                m_ProjectsList.Add(proj);

            PopulateProjectsList();
        }

        void PopulateProjectsList()
        {
            m_ProjectsListView.itemsSource = m_ProjectsList;
            m_ProjectsListView.makeItem = () => m_ProjectsListItemTemplate.Instantiate();
            m_ProjectsListView.bindItem = (element, i) => element.Q<Label>("ProjectItemNameLabel").text = m_ProjectsList[i].Name;
            m_ProjectsListView.RefreshItems();

            m_ProjectsListView.selectionType = SelectionType.Single;
        }
    }
}
#endif
