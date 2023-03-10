using System;
using System.IO;
using System.Linq;
using Unity.Build.Common;
using Unity.Build.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.Build.Classic.Private
{
    public abstract class ClassicPipelineBase : BuildPipelineBase
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            BuildConfigurationInspectorEvents.AfterBuildAction += (config, result) =>
            {
                if (!result)
                {
                    if (result is ResultModuleNotInstalled)
                    {
                        var platform = config.GetPlatform();
                        var title = "Missing Platform Module";
                        var installFromHub = PlatformExtensions.IsEditorInstalledWithHub && platform.IsPublic;
                        if (EditorUtility.DisplayDialog(title, result.Message, installFromHub ? "Install with Unity Hub" : "Open Download Page", "Close"))
                            platform.InstallModule();

                        return true;
                    }

                    if (result is ResultBuildTargetNotActive)
                    {
                        var platform = config.GetPlatform();
                        var title = "Build Target Not Active";
                        if (EditorUtility.DisplayDialog(title, result.Message, "Set Build Target Active", "Close"))
                            platform.SetActiveBuildTargetAsync();

                        return true;
                    }
                }
                return false;
            };
        }

        ClassicBuildPipelineCustomizer[] CreateCustomizers() =>
            TypeConstructionUtility.ConstructTypesDerivedFrom<ClassicBuildPipelineCustomizer>().ToArray();

        Type[] CustomizersUsedComponents =>
            CreateCustomizers().SelectMany(customizer => customizer.UsedComponents).Distinct().ToArray();

        public override Type[] UsedComponents =>
            base.UsedComponents.Concat(CustomizersUsedComponents).Distinct().ToArray();

        public abstract Platform Platform { get; }

        protected virtual void PrepareContext(BuildContext context)
        {
            var buildType = context.GetComponentOrDefault<ClassicBuildProfile>().Configuration;
            bool isDevelopment = buildType == BuildType.Debug || buildType == BuildType.Develop;
            var buildTarget = context.Platform.GetBuildTarget();
            var playbackEngineDirectory = BuildPipeline.GetPlaybackEngineDirectory(buildTarget, isDevelopment ? BuildOptions.Development : BuildOptions.None);

            var customizers = CreateCustomizers();
            foreach (var customizer in customizers)
                customizer.m_Info = new CustomizerInfoImpl(context);

            var workingDir = Path.GetFullPath(Path.Combine(Application.dataPath, $"../Library/BuildWorkingDir/{context.BuildConfigurationName}"));
            Directory.CreateDirectory(workingDir);
            context.SetValue(new ClassicSharedData()
            {
                BuildTarget = buildTarget,
                PlaybackEngineDirectory = playbackEngineDirectory,
                BuildToolsDirectory = Path.Combine(playbackEngineDirectory, "Tools"),
                DevelopmentPlayer = isDevelopment,
                Customizers = customizers,
                WorkingDirectory = workingDir.ToString()
            });

            var scenes = context.GetComponentOrDefault<SceneList>().GetScenePathsForBuild();
            foreach (var modifier in customizers)
                scenes = modifier.ModifyEmbeddedScenes(scenes);
            context.SetValue(new EmbeddedScenesValue() { Scenes = scenes });

            foreach (var customizer in customizers)
                customizer.OnBeforeBuild();
        }

        protected override ResultBase OnCanPrepare(PrepareContext context)
        {
            if (!context.Platform.IsModuleInstalled())
                return new ResultModuleNotInstalled(context.Platform);

            return Result.Success();
        }

        protected override ResultBase OnPrepare(PrepareContext context)
        {
            context.Platform.SetActiveBuildTargetAsync();
            return Result.Success();
        }

        protected override ResultBase OnCanBuild(BuildContext context)
        {
            if (!context.Platform.IsModuleInstalled())
                return new ResultModuleNotInstalled(context.Platform);

            if (!context.Platform.IsActiveBuildTarget())
                return new ResultBuildTargetNotActive(context.Platform);

            return Result.Success();
        }

        protected override ResultBase OnCanRun(RunContext context)
        {
            if (!context.Platform.IsModuleInstalled())
                return new ResultModuleNotInstalled(context.Platform);

            if (!context.Platform.IsActiveBuildTarget())
                return new ResultBuildTargetNotActive(context.Platform);

            return Result.Success();
        }

        protected override CleanResult OnClean(CleanContext context)
        {
            var buildDirectory = context.GetOutputBuildDirectory();
            if (Directory.Exists(buildDirectory))
                Directory.Delete(buildDirectory, true);
            return context.Success();
        }
    }
}
