using System.Runtime.CompilerServices;
using Unity.Cloud.Common;

[assembly: ApiSourceVersion("com.unity.cloud.assets", "0.5.0")]

#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Runtime")]
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Project")]
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Project.Editor")]
#endif
