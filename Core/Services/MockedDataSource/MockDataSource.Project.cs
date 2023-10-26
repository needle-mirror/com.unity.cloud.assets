#if UC_MOCK_ASSETS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.Common;

namespace Unity.Cloud.Assets
{
    partial class MockDataSource : IAssetDataSource
    {
        Dictionary<OrganizationId, List<ProjectData>> m_projects = new Dictionary<OrganizationId, List<ProjectData>>();

        ProjectData EnsureProjectData(OrganizationId organizationId, ProjectId projectId)
        {
            ProjectData projectData = null;

            List<ProjectData> projectList = null;
            if (!m_projects.TryGetValue(organizationId, out projectList))
            {
                projectList = new List<ProjectData>();
                m_projects.Add(organizationId, projectList);
            }

            projectData = projectList.Find(p => p.Id == projectId);
            if (projectData == null)
            {
                projectData = new ProjectData(projectId)
                {
                    Name = k_DefaultName,
                    Metadata = null
                };
                projectList.Add(projectData);
            }
            return projectData;
        }

        public async Task<IProjectData> CreateProjectAsync(OrganizationId organizationId, IProjectBaseData projectCreation, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var createdProjectData = EnsureProjectData(organizationId, new ProjectId(Guid.NewGuid()));
            createdProjectData.Name = projectCreation.Name;
            createdProjectData.Metadata = projectCreation.Metadata;

            return createdProjectData;
        }


        public async Task<IProjectData> GetProjectAsync(ProjectDescriptor projectDescriptor, CancellationToken cancellationToken)
        {
            return EnsureProjectData(projectDescriptor.OrganizationGenesisId, projectDescriptor.ProjectId);
        }

        public async IAsyncEnumerable<IProjectData> ListProjectsAsync(OrganizationId organizationId, Pagination pagination, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            await Task.CompletedTask;
            if (m_projects.Count == 0)
            {
                var p1 = EnsureProjectData(organizationId, new ProjectId(Guid.NewGuid()));
                p1.Name = "Project 1";

                var p2 = EnsureProjectData(organizationId, new ProjectId(Guid.NewGuid()));
                p2.Name = "Project 2";
            }


            if (m_projects.TryGetValue(organizationId, out var projectList))
            {
                var projectsArray = ListItems(projectList, pagination);

                foreach (var projectData in projectsArray)
                {
                    yield return projectData;
                }
            }
        }
    }
}
#endif
