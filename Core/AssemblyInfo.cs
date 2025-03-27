using System.Runtime.CompilerServices;
using Unity.Cloud.Common;

[assembly: ApiSourceVersion("com.unity.cloud.assets", "1.7.0")]

#if !(UC_NUGET)
[assembly: InternalsVisibleTo("Unity.Cloud.Assets.Tests.Editor")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
