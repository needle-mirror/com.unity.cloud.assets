namespace Unity.Cloud.Documentation.Assets
{
    #region Example

    using UnityEngine;

    public class AssetLibraryJobSelectionExampleUI : IAssetManagementUI
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        Vector2 m_ListScrollPosition;

        public AssetLibraryJobSelectionExampleUI(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI()
        {
            // Refresh the library list
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                _ = m_Behaviour.GetAssetLibraryJobsAsync();
                return;
            }

            GUILayout.Space(15f);

            GUILayout.BeginVertical();

            GUILayout.Label("Available Library Jobs:");
            GUILayout.Space(5f);
            ListAssetLibraryJobs();

            GUILayout.EndVertical();
        }

        void ListAssetLibraryJobs()
        {
            var jobs = m_Behaviour.AvailableAssetLibraryJobs.ToArray();
            if (jobs.Length == 0)
            {
                GUILayout.Label("No jobs found.");
                return;
            }

            m_ListScrollPosition = GUILayout.BeginScrollView(m_ListScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(250));

            foreach (var job in jobs)
            {
                GUI.enabled = job.Id != m_Behaviour.CurrentAssetLibraryJob?.Id;

                if (GUILayout.Button(m_Behaviour.GetAssetLibraryJobName(job.Id)))
                {
                    m_Behaviour.SetSelectedAssetLibraryJob(job);
                }

                GUI.enabled = true;
            }

            GUILayout.EndScrollView();
        }
    }

    #endregion
}
