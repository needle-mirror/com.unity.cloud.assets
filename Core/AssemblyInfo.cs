using System.Runtime.CompilerServices;
using Unity.Cloud.Common;

[assembly: ApiSourceVersion("com.unity.cloud.assets", "1.0.0-exp.1")]

#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Runtime")]
#endif
