namespace Unity.Cloud.Documentation.Assets
{
#pragma warning disable S4487 // Unread "private" fields should be removed
#pragma warning disable S1186 // Methods should not be empty

    #region Example_UIClass

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Cloud.Assets;
    using Unity.Cloud.Common;
    using UnityEngine;

    public class UseCaseStartAssetLibraryJobExampleUI : IAssetManagementUI
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public UseCaseStartAssetLibraryJobExampleUI(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        public void OnGUI() { }
    }

    #endregion

#pragma warning restore S1186 // Methods should not be empty
#pragma warning restore S4487 // Unread "private" fields should be removed

    public class UseCaseStartAssetLibraryJobExample : IAssetManagementUI
    {
        readonly UseCaseStartAssetLibraryJobExampleBehaviour m_Behaviour;

        public UseCaseStartAssetLibraryJobExample(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = new UseCaseStartAssetLibraryJobExampleBehaviour(behaviour);
        }

        #region Example_UIContent

        string m_OrganizationId;
        string m_ProjectId;
        string m_CollectionPath;

        public void OnGUI()
        {
            if (!m_Behaviour.IsAssetLibrarySelected) return;

            if (m_Behaviour.CurrentAsset == null)
            {
                GUILayout.Label("! No asset selected. !");
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.Label("Provide the destination where the asset will be copied to");
            m_OrganizationId = AddTextField("Organization ID:", m_OrganizationId);
            m_ProjectId = AddTextField("Project ID:", m_ProjectId);
            m_CollectionPath = AddTextField("Collection Path:", m_CollectionPath);

            GUI.enabled = !string.IsNullOrEmpty(m_OrganizationId) && !string.IsNullOrEmpty(m_ProjectId);

            if (GUILayout.Button("Copy Selected Asset"))
            {
                var projectDescriptor = new ProjectDescriptor(new OrganizationId(m_OrganizationId), new ProjectId(m_ProjectId));
                var collectionDescriptor = new CollectionDescriptor(projectDescriptor, m_CollectionPath);

                _ = m_Behaviour.StartAssetLibraryJobAsync(collectionDescriptor);
            }

            GUI.enabled = true;

            GUILayout.EndVertical();
        }
        
        static string AddTextField(string label, string value, int width = 200)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(150));
            value = GUILayout.TextField(value, GUILayout.Width(width));
            GUILayout.EndHorizontal();
            return value;
        }

        #endregion
    }

    class UseCaseStartAssetLibraryJobExampleBehaviour
    {
        readonly AssetLibrariesBehaviour m_Behaviour;

        public bool IsAssetLibrarySelected => m_Behaviour.IsAssetLibrarySelected;
        public IAssetLibrary CurrentAssetLibrary => m_Behaviour.CurrentAssetLibrary;
        public IAsset CurrentAsset => m_Behaviour.CurrentAsset;

        public UseCaseStartAssetLibraryJobExampleBehaviour(AssetLibrariesBehaviour behaviour)
        {
            m_Behaviour = behaviour;
        }

        #region Example_Behaviour_CopyAsset

        public async Task StartAssetLibraryJobAsync(CollectionDescriptor collectionDescriptor)
        {
            var list = new AssetsToCopy();
            list.Add(CurrentAsset.Descriptor, collectionDescriptor);

            try
            {
                await foreach (var jobResult in CurrentAssetLibrary.StartCopyAssetsJobAsync(collectionDescriptor.ProjectDescriptor, list, CancellationToken.None))
                {
                    Debug.Log($"Started copy asset job with id {jobResult.Id} for asset {CurrentAsset.Descriptor.AssetId}.");
                }
            }
            catch (ServiceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        #endregion
    }
}
