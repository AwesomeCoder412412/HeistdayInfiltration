using System.Linq;
using Unity.Build.Common;
using Unity.Build.Editor;

namespace Unity.Build.Classic
{
    /// <summary>
    /// Utility class to handle creating classic build configuration assets from menu items.
    /// </summary>
    public static class ClassicBuildConfigurationMenuItem
    {
        public const string k_ItemNameSuffix = " Classic Build Configuration";

        /// <summary>
        /// Create a new classic build configuration asset saved to disk, in the active directory, and focus it for renaming.
        /// </summary>
        /// <remarks>
        /// If a build configuration asset is set as the current active object of selection, it will be added to the build configuration dependencies.
        /// </remarks>
        /// <param name="name">The asset name.</param>
        /// <param name="components">Build components to add to the new build configuration asset.</param>
        public static void CreateAssetInActiveDirectory(Platform platform, params IBuildComponent[] components)
        {
            BuildConfigurationMenuItem.CreateAssetInActiveDirectory(platform.DisplayName + k_ItemNameSuffix, new IBuildComponent[]
            {
                new GeneralSettings(),
                new SceneList(),
                new ClassicBuildProfile
                {
                    Platform = platform
                }
            }.Concat(components).Distinct().ToArray());
        }
    }
}
