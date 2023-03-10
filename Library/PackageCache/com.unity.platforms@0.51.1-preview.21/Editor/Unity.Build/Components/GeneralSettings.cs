using System;
using Unity.Serialization;
using UnityEditor;

namespace Unity.Build.Common
{
    [FormerName("Unity.Build.Common.GeneralSettings, Unity.Build.Common")]
    public sealed partial class GeneralSettings : IBuildComponent, IBuildComponentInitialize
    {
        public void Initialize(BuildConfiguration.ReadOnly config)
        {
            ProductName = PlayerSettings.productName;
            CompanyName = PlayerSettings.companyName;
            if (Version.TryParse(PlayerSettings.bundleVersion, out var version))
            {
                // Note: Not assigning Build or Revision, so they would stay -1, -1. That way when version is converted to string, it generations 1.0 and not 1.0.0.0 which is not acceptable on some platforms
                Version = new Version(Math.Max(version.Major, 0),
                    Math.Max(version.Minor, 0));
            }
            else
            {
                Version = new Version(1, 0);
            }
        }
    }
}
