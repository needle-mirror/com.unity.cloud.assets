using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Cloud.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Cloud.Assets.Samples
{
    public class ProjectController : OrganizationController
    {
        [SerializeField]
        VisualTreeAsset m_ListItemTemplate;

        [SerializeField]
        bool m_IncludeAllProject = true;

        readonly ProjectListUi m_ProjectListUi = new();

        public IAssetProject SelectedProject => m_ProjectListUi.SelectedProject;
        public bool IsAllProjectSelected => m_ProjectListUi.IsAllProjectSelected;

        public event Action ProjectSelected
        {
            add => m_ProjectListUi.ProjectSelected += value;
            remove => m_ProjectListUi.ProjectSelected -= value;
        }

        protected override void Start()
        {
            base.Start();

            m_ProjectListUi.Initialize(RootVisualElement, m_ListItemTemplate);
            m_ProjectListUi.Hide();

            var contextMenu = new ContextMenuController(RootVisualElement.Q("LeftPanelContextMenu"));
            contextMenu?.SetEnabled(false);

            OrganizationSelected += PopulateProjectList;
        }

        public IAsyncEnumerable<IAsset> GetAssetsAcrossAllProjectsAsync(CancellationToken cancellationToken)
        {
            try
            {
                return AssetRepository.QueryAssets(m_ProjectListUi.GetProjects().Select(p => p.Descriptor)).ExecuteAsync(cancellationToken);
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException();
                return null;
            }
            catch (Exception e)
            {
                e.LogException();
                throw;
            }
        }

        public IAsyncEnumerable<IAsset> GetAssetsAsync(CancellationToken cancellationToken)
        {
            try
            {
                return SelectedProject.QueryAssets().ExecuteAsync(cancellationToken);
            }
            catch (OperationCanceledException oe)
            {
                oe.LogException();
                return null;
            }
            catch (Exception e)
            {
                e.LogException();
                throw;
            }
        }

        public IEnumerable<IAssetProject> GetAllProjects()
        {
            return m_ProjectListUi.GetProjects();
        }

        protected override void Hide()
        {
            base.Hide();

            m_ProjectListUi.Hide();
        }

        async void PopulateProjectList(OrganizationId organizationId)
        {
            m_ProjectListUi.ClearSelection();
            await m_ProjectListUi.Populate(AssetRepository, organizationId, m_IncludeAllProject);
        }
    }
}
