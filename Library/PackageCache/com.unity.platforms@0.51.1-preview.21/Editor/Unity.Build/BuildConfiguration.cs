using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;

namespace Unity.Build
{
    /// <summary>
    /// Can stores a set of hierarchical build components per type, which can be inherited or overridden using dependencies.
    /// </summary>
    public sealed class BuildConfiguration : HierarchicalComponentContainer<BuildConfiguration, IBuildComponent>
    {
        const string k_ActiveBuildConfigurationKey = "BuildConfiguration.Active";

        /// <summary>
        /// Event fired when active build configuration is changed.
        /// </summary>
        public static event Action<ActiveBuildConfigurationChangedEvent> ActiveChanged;

        /// <summary>
        /// Retrieve the current active build configuration.
        /// Value is stored per-project and per-user.
        /// </summary>
        /// <returns>Current active build configuration, <see langword="null"/> otherwise.</returns>
        public static BuildConfiguration GetActive()
        {
            var value = EditorUserSettings.GetConfigValue(k_ActiveBuildConfigurationKey);
            if (string.IsNullOrEmpty(value))
                value = new GlobalObjectId().ToString();

            var config = default(BuildConfiguration);
            if (GlobalObjectId.TryParse(value, out var id))
                config = (BuildConfiguration)GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);

            return config;
        }

        /// <summary>
        /// Set current active build configuration.
        /// The build configuration must have a build pipeline set, and be buildable.
        /// </summary>
        /// <remarks>Note: Some build pipeline might trigger a domain reload upon setting the active build configuration.</remarks>
        /// <param name="config">The build configuration.</param>
        /// <returns>A result indicating if the active build configuration was set or not.</returns>
        public static ResultBase SetActive(BuildConfiguration config)
        {
            var oldConfig = GetActive();
            if (config == null || !config)
            {
                EditorUserSettings.SetConfigValue(k_ActiveBuildConfigurationKey, null);
                ActiveChanged?.Invoke(new ActiveBuildConfigurationChangedEvent(null, oldConfig));
                return Result.Success();
            }

            var pipeline = config.GetBuildPipeline();
            if (pipeline == null)
                return new ResultNoPipelineSpecified();

            var canPrepare = pipeline.CanPrepare(config);
            if (!canPrepare)
                return canPrepare;

            var id = GlobalObjectId.GetGlobalObjectIdSlow(config);
            EditorUserSettings.SetConfigValue(k_ActiveBuildConfigurationKey, id.ToString());
            ActiveChanged?.Invoke(new ActiveBuildConfigurationChangedEvent(config, oldConfig));

            // Prepare environment for pipeline
            // NOTE: This can result in a domain reload!
            return pipeline.Prepare(config);
        }

        /// <summary>
        /// File extension for build configuration assets.
        /// </summary>
        public const string AssetExtension = ".buildconfiguration";

        /// <summary>
        /// Determine if this build configuration should be shown or hidden.
        /// </summary>
        [CreateProperty] public bool Show { get; set; } = true;

        /// <summary>
        /// Retrieve the build pipeline of this build configuration.
        /// </summary>
        /// <returns>The build pipeline if found, <see langword="null"/> otherwise.</returns>
        public BuildPipelineBase GetBuildPipeline() => TryGetComponent<IBuildPipelineComponent>(out var component) ? component.Pipeline : null;

        /// <summary>
        /// Retrieve the platform of this build configuration.
        /// </summary>
        /// <returns>The platform if found, <see langword="null"/> otherwise.</returns>
        public Platform GetPlatform() => TryGetComponent<IBuildPipelineComponent>(out var component) ? component.Platform : null;

        /// <summary>
        /// Determine if component is used by the build pipeline of this build configuration.
        /// Returns <see langword="false"/> if this build configuration does not have a build pipeline.
        /// </summary>
        /// <param name="type">The component type.</param>
        /// <returns><see langword="true"/> if the component is used by the build pipeline, <see langword="false"/> otherwise.</returns>
        public bool IsComponentUsed(Type type) => GetBuildPipeline()?.IsComponentUsed(type) ?? false;

        /// <summary>
        /// Determine if component is used by the build pipeline of this build configuration.
        /// Returns <see langword="false"/> if this build configuration does not have a build pipeline.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns><see langword="true"/> if the component is used by the build pipeline, <see langword="false"/> otherwise.</returns>
        public bool IsComponentUsed<T>() where T : IBuildComponent => IsComponentUsed(typeof(T));

        /// <summary>
        /// Determine if the environment can be prepared before the build pipeline of this build configuration can be used.
        /// </summary>
        /// <returns>A result indicating if the environment can be prepared.</returns>
        public ResultBase CanPrepare()
        {
            var pipeline = GetBuildPipeline();
            var canUse = CanUsePipeline(pipeline);
            return canUse ? pipeline.CanPrepare(this) : canUse;
        }

        /// <summary>
        /// Prepare the environment to use the build pipeline of this build configuration.
        /// </summary>
        /// <returns>A result indicating if the operation is successful.</returns>
        public ResultBase Prepare()
        {
            var pipeline = GetBuildPipeline();
            var canUse = CanUsePipeline(pipeline);
            return canUse ? pipeline.Prepare(this) : canUse;
        }

        /// <summary>
        /// Determine if the build pipeline of this build configuration can build.
        /// </summary>
        /// <returns>A result describing if the pipeline can build or not.</returns>
        public ResultBase CanBuild()
        {
            var pipeline = GetBuildPipeline();
            var canUse = CanUsePipeline(pipeline);
            return canUse ? pipeline.CanBuild(this) : canUse;
        }

        /// <summary>
        /// Run the build pipeline of this build configuration to build the target.
        /// </summary>
        /// <returns>The result of the build pipeline build.</returns>
        public BuildResult Build()
        {
            var pipeline = GetBuildPipeline();
            var canUse = CanUsePipeline(pipeline);
            if (!canUse)
                return new BuildResult(pipeline, this, canUse);

            var what = !string.IsNullOrEmpty(name) ? $" {name}" : string.Empty;
            using (var progress = new BuildProgress($"Building{what}", "Please wait..."))
            {
                return pipeline.Build(this, progress);
            }
        }

        /// <summary>
        /// Determine if the build pipeline of this build configuration can run.
        /// </summary>
        /// <param name="runTargets">List of run targets to deploy and run on.</param>
        /// <returns>A result describing if the pipeline can run or not.</returns>
        public ResultBase CanRun(params RunTargetBase[] runTargets)
        {
            var pipeline = GetBuildPipeline();
            var canUse = CanUsePipeline(pipeline);
            return canUse ? pipeline.CanRun(this, runTargets) : canUse;
        }

        /// <summary>
        /// Run the resulting target from building the build pipeline of this build configuration.
        /// </summary>
        /// <param name="runTargets">List of run targets to deploy and run on.</param>
        /// <returns></returns>
        public RunResult Run(params RunTargetBase[] runTargets)
        {
            var pipeline = GetBuildPipeline();
            var canUse = CanUsePipeline(pipeline);
            return canUse ? pipeline.Run(this, runTargets) : new RunResult(pipeline, this, canUse);
        }

        /// <summary>
        /// Clean the build result from building the build pipeline of this build configuration.
        /// </summary>
        public CleanResult Clean()
        {
            var pipeline = GetBuildPipeline();
            var canUse = CanUsePipeline(pipeline);
            return canUse ? pipeline.Clean(this) : new CleanResult(pipeline, this, canUse);
        }

        /// <summary>
        /// Determine if a build artifact that is assignable to the specified type is present.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        /// <param name="buildArtifactType">The build artifact type.</param>
        /// <returns><see langword="true"/> if a matching build artifact is found, <see langword="false"/> otherwise.</returns>
        public bool HasBuildArtifact(BuildPipelineBase pipeline, Type buildArtifactType) => BuildArtifacts.HasBuildArtifact(pipeline, this, buildArtifactType);

        /// <summary>
        /// Determine if a build artifact that is assignable to the specified type is present.
        /// </summary>
        /// <typeparam name="T">The build artifact type.</typeparam>
        /// <param name="pipeline">The build pipeline.</param>
        /// <returns></returns>
        public bool HasBuildArtifact<T>(BuildPipelineBase pipeline) where T : class, IBuildArtifact, new() => BuildArtifacts.HasBuildArtifact<T>(pipeline, this);

        /// <summary>
        /// Determine if a build artifact that is assignable to the specified type is present.
        /// </summary>
        /// <param name="buildArtifactType">The build artifact type.</param>
        /// <returns><see langword="true"/> if a matching build artifact is found, <see langword="false"/> otherwise.</returns>
        public bool HasBuildArtifact(Type buildArtifactType) => HasBuildArtifact(GetBuildPipeline(), buildArtifactType);

        /// <summary>
        /// Determine if a build artifact that is assignable to the specified type is present.
        /// </summary>
        /// <typeparam name="T">The build artifact type.</typeparam>
        /// <returns><see langword="true"/> if a matching build artifact is found, <see langword="false"/> otherwise.</returns>
        public bool HasBuildArtifact<T>() where T : class, IBuildArtifact, new() => HasBuildArtifact<T>(GetBuildPipeline());

        /// <summary>
        /// Get the first build artifact value that is assignable to specified type.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        /// <param name="buildArtifactType">The build artifact type.</param>
        /// <returns>A build artifact value if found, <see langword="null"/> otherwise.</returns>
        public IBuildArtifact GetBuildArtifact(BuildPipelineBase pipeline, Type buildArtifactType) => BuildArtifacts.GetBuildArtifact(pipeline, this, buildArtifactType);

        /// <summary>
        /// Get the first build artifact value that is assignable to specified type.
        /// </summary>
        /// <typeparam name="T">The build artifact type.</typeparam>
        /// <param name="pipeline">The build pipeline.</param>
        /// <returns>A build artifact value if found, <see langword="null"/> otherwise.</returns>
        public T GetBuildArtifact<T>(BuildPipelineBase pipeline) where T : class, IBuildArtifact, new() => BuildArtifacts.GetBuildArtifact<T>(pipeline, this);

        /// <summary>
        /// Get the first build artifact value that is assignable to specified type.
        /// </summary>
        /// <param name="buildArtifactType">The build artifact type.</param>
        /// <returns>A build artifact value if found, <see langword="null"/> otherwise.</returns>
        public IBuildArtifact GetBuildArtifact(Type buildArtifactType) => GetBuildArtifact(GetBuildPipeline(), buildArtifactType);

        /// <summary>
        /// Get the first build artifact value that is assignable to specified type.
        /// </summary>
        /// <typeparam name="T">The build artifact type.</typeparam>
        /// <returns>A build artifact value if found, <see langword="null"/> otherwise.</returns>
        public T GetBuildArtifact<T>() where T : class, IBuildArtifact, new() => GetBuildArtifact<T>(GetBuildPipeline());

        /// <summary>
        /// Get all build artifact values.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        /// <returns>Enumeration of all build artifact values.</returns>
        public IEnumerable<IBuildArtifact> GetAllBuildArtifacts(BuildPipelineBase pipeline) => BuildArtifacts.GetAllBuildArtifacts(pipeline, this);

        /// <summary>
        /// Get all build artifact values.
        /// </summary>
        /// <returns>Enumeration of all build artifact values.</returns>
        public IEnumerable<IBuildArtifact> GetAllBuildArtifacts() => GetAllBuildArtifacts(GetBuildPipeline());

        /// <summary>
        /// Get the build result of the last <see cref="Build"/> performed.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        /// <returns>The build result if found, <see langword="null"/> otherwise.</returns>
        public BuildResult GetBuildResult(BuildPipelineBase pipeline) => BuildArtifacts.GetBuildResult(pipeline, this);

        /// <summary>
        /// Get the build result of the last <see cref="Build"/> performed.
        /// </summary>
        /// <returns>The build result if found, <see langword="null"/> otherwise.</returns>
        public BuildResult GetBuildResult() => GetBuildResult(GetBuildPipeline());

        /// <summary>
        /// Clean the build artifact of the last <see cref="Build"/> performed.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        public void CleanBuildArtifact(BuildPipelineBase pipeline) => BuildArtifacts.CleanBuildArtifact(pipeline, this);

        /// <summary>
        /// Clean the build artifact of the last <see cref="Build"/> performed.
        /// </summary>
        public void CleanBuildArtifact() => CleanBuildArtifact(GetBuildPipeline());

        /// <summary>
        /// Clean all build artifact of every build configuration.
        /// </summary>
        public static void CleanAllBuildArtifacts() => BuildArtifacts.CleanAllBuildArtifacts();

        /// <summary>
        /// Get the output build directory override for this build configuration.
        /// The output build directory can be overridden using a <see cref="OutputBuildDirectory"/> component.
        /// </summary>
        /// <returns>The output build directory.</returns>
        public string GetOutputBuildDirectory()
        {
            var pipeline = GetBuildPipeline();
            if (pipeline == null)
                throw new NullReferenceException("The BuildConfiguration must have a BuildPipline in order to retrieve the OutputBuildDirectory");

            return pipeline.GetOutputBuildDirectory(this).ToString();
        }

        ResultBase CanUsePipeline(BuildPipelineBase pipeline)
        {
            return pipeline == null ? Result.Failure($"No valid build pipeline found for {this.ToHyperLink()}. At least one component that derives from {nameof(IBuildPipelineComponent)} must be present.") : Result.Success();
        }

        [Obsolete("GetLastBuildArtifact has been renamed to GetBuildArtifact. (RemovedAfter 2021-02-01)")]
        public IBuildArtifact GetLastBuildArtifact(Type type) => GetBuildArtifact(type);

        [Obsolete("GetLastBuildArtifact has been renamed to GetBuildArtifact. (RemovedAfter 2021-02-01)")]
        public T GetLastBuildArtifact<T>() where T : class, IBuildArtifact => (T)GetBuildArtifact(typeof(T));

        [Obsolete("GetLastBuildResult has been renamed to GetBuildResult. (RemovedAfter 2021-02-01)")]
        public BuildResult GetLastBuildResult() => GetBuildResult();
    }

    public static class BuildConfigurationReadOnlyExtensions
    {
        /// <summary>
        /// Retrieve the build pipeline of this build configuration.
        /// </summary>
        /// <returns>The build pipeline if found, <see langword="null"/> otherwise.</returns>
        public static BuildPipelineBase GetBuildPipeline(this BuildConfiguration.ReadOnly config) => config.Container.GetBuildPipeline();

        /// <summary>
        /// Retrieve the platform of this build configuration.
        /// </summary>
        /// <returns>The platform if found, <see langword="null"/> otherwise.</returns>
        public static Platform GetPlatform(this BuildConfiguration.ReadOnly config) => config.Container.GetPlatform();
    }
}
