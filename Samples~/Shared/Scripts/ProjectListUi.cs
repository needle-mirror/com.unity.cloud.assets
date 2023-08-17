#if !UC_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Cloud.Assets.Samples
{
    public class ProjectListUi : ListUi<ProjectListController, IProject>
    {
        static readonly Pagination m_DefaultPagination = new(nameof(IProject.Name), Range.All);

        public event Action ProjectSelected;

        IProject m_SelectedProject;

        CancellationTokenSource m_ListProjectsCancellationTokenSource = new();
        public IProject SelectedProject
        {
            get => m_SelectedProject;
            private set
            {
                m_SelectedProject = value;
                Debug.Log($"Project Selected: {m_SelectedProject?.Name}");
                ProjectSelected?.Invoke();
            }
        }
        public IProject ProjectAll => m_ListController.AllItem;


        protected override string VisualElementName => "ProjectsPanel";
        protected override string EmptyListMessage => "No projects available.";

        public async Task Populate(IOrganization organization)
        {
            Show();

            m_ListProjectsCancellationTokenSource.Cancel();
            m_ListProjectsCancellationTokenSource.Dispose();
            m_ListProjectsCancellationTokenSource = new CancellationTokenSource();

            var token = m_ListProjectsCancellationTokenSource.Token;
            await UpdateList(GetProjectsAsync(organization, token), token);
        }

        public override void AddAllItem()
        {
            m_ListController.AllItem = new DummyProject
            {
                Id = "All",
                Name = "All"
            };
        }

        static IAsyncEnumerable<IProject> GetProjectsAsync(IOrganization organization, CancellationToken token)
        {
            try
            {
                return PlatformServices.ProjectProvider.ListProjectsAsync(organization, m_DefaultPagination, token);
            }
            catch (OperationCanceledException oe)
            {
                Debug.LogException(oe);
                throw;
            }
            catch (AggregateException e)
            {
                Debug.LogException(e.InnerException);
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        protected override void OnSelectionChange(IEnumerable<object> selectedItems)
        {
            SelectedProject = selectedItems.FirstOrDefault() as IProject;
        }

        public IEnumerable<IProject> GetProjects()
        {
            return m_Entries;
        }
    }
}
#endif
