using UnityEditor;

#if UNITY_INTERNAL
using System.IO;
using System.Linq;
#endif

namespace Unity.Build.Editor
{
    /// <summary>
    /// Utility class to handle creating build configuration assets from menu items.
    /// </summary>
    public static class BuildConfigurationMenuItem
    {
        public const string k_MenuPathName = "Assets/Create/Build Configuration/";
        const string k_AssetName = "Empty Build Configuration";

#if UNITY_INTERNAL
        [MenuItem("INTERNAL/Upgrade All Build Assets")]
        static void UpgradeAllBuildAssets()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(BuildConfiguration)}");
            var paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
            try
            {
                for (var i = 0; i < paths.Length; ++i)
                {
                    var assetPath = paths[i];
                    EditorUtility.DisplayProgressBar($"Upgrading Asset ({i + 1} of {paths.Length})", Path.GetFileName(assetPath), (float)i / paths.Length);
                    var asset = AssetDatabase.LoadAssetAtPath<BuildConfiguration>(assetPath);
                    asset.SerializeToPath(assetPath);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }
#endif

        [MenuItem(k_MenuPathName + k_AssetName, false, 0)]
        static void CreateBuildConfigurationAsset()
        {
            CreateAssetInActiveDirectory(k_AssetName);
        }

        /// <summary>
        /// Create a new build configuration asset saved to disk, in the active directory, and focus it for renaming.
        /// </summary>
        /// <remarks>
        /// If a build configuration asset is set as the current active object of selection, it will be added to the build configuration dependencies.
        /// </remarks>
        /// <param name="name">The asset name.</param>
        /// <param name="components">Build components to add to the new build configuration asset.</param>
        public static void CreateAssetInActiveDirectory(string name, params IBuildComponent[] components)
        {
            var dependency = Selection.activeObject as BuildConfiguration;
            BuildConfiguration.CreateAssetInActiveDirectory(name + BuildConfiguration.AssetExtension, (config) =>
            {
                if (dependency != null && dependency)
                    config.AddDependency(dependency);

                foreach (var component in components)
                    config.SetComponent(component);
            });
        }
    }
}
