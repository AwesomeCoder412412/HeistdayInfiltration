using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Build.Tests.Common")]
[assembly: InternalsVisibleTo("Unity.Build.Classic.Tests")]

// Per platform access
[assembly: InternalsVisibleTo("Unity.Build.Android.Classic")]
[assembly: InternalsVisibleTo("Unity.Build.Android.Classic.Tests")]
[assembly: InternalsVisibleTo("Unity.Build.iOS.Classic")]
[assembly: InternalsVisibleTo("Unity.Build.iOS.Classic.Tests")]
[assembly: InternalsVisibleTo("Unity.Build.Linux.Classic")]
[assembly: InternalsVisibleTo("Unity.Build.Linux.Classic.Tests")]
[assembly: InternalsVisibleTo("Unity.Build.macOS.Classic")]
[assembly: InternalsVisibleTo("Unity.Build.macOS.Classic.Tests")]
[assembly: InternalsVisibleTo("Unity.Build.Web.Classic")]
[assembly: InternalsVisibleTo("Unity.Build.Web.Classic.Tests")]
[assembly: InternalsVisibleTo("Unity.Build.Windows.Classic")]
[assembly: InternalsVisibleTo("Unity.Build.Windows.Classic.Tests")]
