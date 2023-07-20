using System;
using System.Collections.Generic;

namespace Unity.Cloud.Assets
{
    static class ProjectMapper
    {
        public static IProject[] MapFrom(this ProjectPageDto dto)
        {
            if (dto.Projects == null || dto.Projects.Length == 0) return Array.Empty<IProject>();

            var projects = new IProject[dto.Projects.Length];
            for (var i = 0; i < projects.Length; ++i)
            {
                dto.Projects[i].Initialize();
                projects[i] = dto.Projects[i];
            }

            return projects;
        }
    }
}
