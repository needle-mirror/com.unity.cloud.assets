#if UNITY_EDITOR
#if !USE_UIELEMENTS
#error Missing dependency to com.unity.modules.uielements
#else

using System.Collections;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples.AssetDatabaseUploader.Editor
{
    /// <summary>
    /// UI for <see cref="OrgAndProjectSelectorEditor"/>
    /// </summary>
    public class OrgAndProjectSelectorEditorUI : VisualElement
    {
        const string k_NoOrganizationsMessage = "No organizations available.";
        const string k_NoProjectsMessage = "No projects available.";

        readonly OrgAndProjectSelector m_OrgAndProjectSelector;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serializedObject"></param>
        public OrgAndProjectSelectorEditorUI(SerializedObject serializedObject)
        {
            m_OrgAndProjectSelector = (OrgAndProjectSelector)serializedObject.targetObject;

            DrawUI();
        }

        void Refresh()
        {
            Clear();
            DrawUI();
        }

        void DrawUI()
        {
            Add(EditorUIUtils.CreateSpaceBox());

            DrawFetchButton();

            Add(EditorUIUtils.CreateSpaceBox());

            DrawOrganizations();

            Add(EditorUIUtils.CreateSpaceBox());

            DrawProjects();
        }

        void DrawFetchButton()
        {
            var fetchBtn = new Button(OnFetchButtonClick);
            fetchBtn.text = "Fetch Organizations and Projects";

            Add(fetchBtn);
        }

        void OnFetchButtonClick()
        {
            EditorCoroutineUtility.StartCoroutine(FetchActionCoroutine(), this);
        }

        IEnumerator FetchActionCoroutine()
        {
            var fetchingTask = m_OrgAndProjectSelector.FetchOrganizationsAndProjectsAsync();

            yield return new WaitUntil(() => fetchingTask.IsCompleted);

            Refresh();
        }

        void DrawOrganizations()
        {
            if (m_OrgAndProjectSelector == null ||
                m_OrgAndProjectSelector.Organizations == null ||
                m_OrgAndProjectSelector.Organizations.Count == 0)
            {
                var noOrganizationsLabel = new Label(k_NoOrganizationsMessage);
                Add(noOrganizationsLabel);

                return;
            }

            var defaultIndexSelection = m_OrgAndProjectSelector.SelectedOrganization != null ? m_OrgAndProjectSelector.Organizations.IndexOf(m_OrgAndProjectSelector.SelectedOrganization) : 0;
            var dropDown = new DropdownField
            (
                m_OrgAndProjectSelector.Organizations.Select(o => o.Name).ToList(),
                defaultIndexSelection,
                (orgName) =>
                {
                    if (m_OrgAndProjectSelector.ChangeSelectedOrganization(m_OrgAndProjectSelector.Organizations.FirstOrDefault(o => o.Name == orgName)))
                    {
                        EditorUtility.SetDirty(m_OrgAndProjectSelector);
                        RefreshProjects();
                    }

                    return orgName;
                }
            );

            Add(dropDown);
        }

        void RefreshProjects()
        {
            EditorCoroutineUtility.StartCoroutine(RefreshProjectsCoroutine(), this);
        }

        IEnumerator RefreshProjectsCoroutine()
        {
            var fetchingTask = m_OrgAndProjectSelector.FetchProjectsAsync();

            yield return new WaitUntil(() => fetchingTask.IsCompleted);

            Refresh();
        }

        void DrawProjects()
        {
            if (m_OrgAndProjectSelector == null ||
                m_OrgAndProjectSelector.Projects == null ||
                m_OrgAndProjectSelector.Projects.Count == 0)
            {
                var noProjectsLabel = new Label(k_NoProjectsMessage);
                Add(noProjectsLabel);

                return;
            }

            var defaultIndexSelection = m_OrgAndProjectSelector.SelectedProject != null ? m_OrgAndProjectSelector.Projects.IndexOf(m_OrgAndProjectSelector.SelectedProject) : 0;
            var dropDown = new DropdownField
            (
                m_OrgAndProjectSelector.Projects.Select(o => o.Name).ToList(),
                defaultIndexSelection,
                (projectName) =>
                {
                    if(m_OrgAndProjectSelector.ChangeSelectedProject(m_OrgAndProjectSelector.Projects.FirstOrDefault(o => o.Name == projectName)))
                        EditorUtility.SetDirty(m_OrgAndProjectSelector);

                    return projectName;
                }
            );

            Add(dropDown);
        }
    }
}
#endif
#endif
