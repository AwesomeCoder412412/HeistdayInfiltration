#if ENABLE_PLAYMODE_EXTENSION
using System;
using Unity.Build.Classic;
using UnityEditor;
using UnityEditor.TestTools.TestRunner;
using UnityEngine;

namespace Unity.Build.Playmode.TestRunner
{
    /// <summary>
    /// Dummy component which purpose is to tell the PlaymodeTestRunnerBuildCustomizer that we're building a player for playmode tests
    /// </summary>
    [HideInInspector]
    class PlaymodeTestRunnerComponent : IBuildComponent
    {

    }

    class PlaymodeTestRunnerBuildCustomizer : ClassicBuildPipelineCustomizer
    {
        public override Type[] UsedComponents { get; } = new[] {typeof(PlaymodeTestRunnerComponent)};

        public override BuildOptions ProvideBuildOptions()
        {
            if (!Context.HasComponent<PlaymodeTestRunnerComponent>())
                return base.ProvideBuildOptions();
            
            var compressionOptions = PlayerLauncherBuildOptions.GetCompressionBuildOptions(BuildPipeline.GetBuildTargetGroup(BuildTarget), BuildTarget);
            // Replicate com.unity.test-framework@1.2.1-preview.4\UnityEditor.TestRunner\TestLaunchers\PlayerLauncher.cs behavior
            var options = BuildOptions.IncludeTestAssemblies | BuildOptions.StrictMode | compressionOptions | BuildOptions.ConnectToHost;

            if (EditorUserBuildSettings.waitForPlayerConnection)
                options |= BuildOptions.WaitForPlayerConnection;

            if (EditorUserBuildSettings.allowDebugging)
                options |= BuildOptions.AllowDebugging;

            if (EditorUserBuildSettings.installInBuildFolder)
                options |= BuildOptions.InstallInBuildFolder;
            /*
             // Don't ever pass AutoRunplayer since BuildConfiguration run players via dedicated Run method
            else
                options |= BuildOptions.AutoRunPlayer;
            */
            return options;
        }
    }
}
#endif
