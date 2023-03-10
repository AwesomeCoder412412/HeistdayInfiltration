using System;
using System.Reflection;
using Unity.Build.Bridge;
using UnityEditor;

namespace Unity.Build.Classic
{
    /// <summary>
    /// <see cref="Platform"/> class extensions.
    /// </summary>
    public static class PlatformExtensions
    {
        delegate bool IsEditorInstalledWithHubDelegate();
        delegate string GetUnityHubModuleDownloadURLDelegate(string moduleName);
        delegate string GetPlaybackEngineDownloadURLDelegate(string moduleName);

        static readonly IsEditorInstalledWithHubDelegate s_IsEditorInstalledWithHubMethod;
        static readonly GetUnityHubModuleDownloadURLDelegate s_GetUnityHubModuleDownloadURLMethod;
        static readonly GetPlaybackEngineDownloadURLDelegate s_GetPlaybackEngineDownloadURLMethod;

        static PlatformExtensions()
        {
            s_IsEditorInstalledWithHubMethod = GetMethod<BuildPlayerWindow, IsEditorInstalledWithHubDelegate>("IsEditorInstalledWithHub", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            s_GetUnityHubModuleDownloadURLMethod = GetMethod<BuildPlayerWindow, GetUnityHubModuleDownloadURLDelegate>("GetUnityHubModuleDownloadURL", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            s_GetPlaybackEngineDownloadURLMethod = GetMethod<BuildPlayerWindow, GetPlaybackEngineDownloadURLDelegate>("GetPlaybackEngineDownloadURL", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// Determine if the editor was installed with Unity Hub.
        /// </summary>
        public static bool IsEditorInstalledWithHub => s_IsEditorInstalledWithHubMethod();

        /// <summary>
        /// Determine if a platform module is installed.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <returns><see langword="true"/> if the platform module is installed, <see langword="false"/> otherwise.</returns>
        public static bool IsModuleInstalled(this Platform platform)
        {
            var buildTarget = platform.GetBuildTarget();
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            return BuildPipeline.IsBuildTargetSupported(buildTargetGroup, buildTarget);
        }

        /// <summary>
        /// Install a platform module from Unity Hub or download page.
        /// </summary>
        /// <param name="platform">The platform.</param>
        public static void InstallModule(this Platform platform)
        {
            var buildTarget = platform.GetBuildTarget();
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var moduleName = ModuleManagerBridge.GetTargetStringFrom(buildTargetGroup, buildTarget);
            var url = IsEditorInstalledWithHub && platform.IsPublic ? s_GetUnityHubModuleDownloadURLMethod(moduleName) : s_GetPlaybackEngineDownloadURLMethod(moduleName);
            Help.BrowseURL(url);
        }

        /// <summary>
        /// Determine if a platform is currently the active build target.
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static bool IsActiveBuildTarget(this Platform platform)
        {
            var buildTarget = platform.GetBuildTarget();
            return EditorUserBuildSettings.activeBuildTarget == buildTarget;
        }

        /// <summary>
        /// Set a platform as the current active build target.
        /// </summary>
        /// <param name="platform"></param>
        public static void SetActiveBuildTargetAsync(this Platform platform)
        {
            var buildTarget = platform.GetBuildTarget();
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(buildTargetGroup, buildTarget);
        }

        static TDelegate GetMethod<TDelegate>(Type type, string name, BindingFlags flags) where TDelegate : Delegate
        {
            var method = type.GetMethod(name, flags);
            if (method == null)
                throw new NullReferenceException($"Could not find method {name} in {type.FullName}.");

            return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), type.GetMethod(name, flags));
        }

        static TDelegate GetMethod<TType, TDelegate>(string name, BindingFlags flags) where TDelegate : Delegate
        {
            return GetMethod<TDelegate>(typeof(TType), name, flags);
        }
    }
}
