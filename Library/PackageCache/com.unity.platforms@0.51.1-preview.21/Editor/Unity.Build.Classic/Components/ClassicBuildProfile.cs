using System;
using Unity.Properties;
using Unity.Serialization;
using Unity.Serialization.Json;
using Unity.Serialization.Json.Adapters;
using UnityEditor;

namespace Unity.Build.Classic
{
    [FormerName("Unity.Build.Common.ClassicBuildProfile, Unity.Build.Common")]
    public sealed class ClassicBuildProfile : IBuildPipelineComponent
    {
        Platform m_Platform;
        BuildPipelineBase m_Pipeline;

        /// <summary>
        /// Gets or sets which <see cref="Build.Platform"/> this profile is going to use for the build.
        /// Used for building classic players.
        /// </summary>
        [CreateProperty]
        public Platform Platform
        {
            get => m_Platform;
            set
            {
                if (value == null)
                {
                    m_Platform = null;
                    m_Pipeline = null;
                }
                else if (!value.Equals(m_Platform))
                {
                    m_Platform = value;
                    m_Pipeline = ConstructPipeline(m_Platform);
                }
            }
        }

        /// <summary>
        /// Gets or sets which <see cref="BuildType"/> this profile is going to use for the build.
        /// </summary>
        [CreateProperty]
        public BuildType Configuration { get; set; } = BuildType.Develop;

        public BuildPipelineBase Pipeline
        {
            get => m_Pipeline;
            set => throw new InvalidOperationException($"Cannot explicitly set {nameof(Pipeline)}, set {nameof(Platform)} property instead.");
        }

        public int SortingIndex => throw new NotImplementedException();

        public bool SetupEnvironment() => throw new NotImplementedException();

        public ClassicBuildProfile()
        {
#if UNITY_EDITOR_WIN
            Platform = KnownPlatforms.Windows.GetPlatform();
#elif UNITY_EDITOR_OSX
            Platform = KnownPlatforms.macOS.GetPlatform();
#elif UNITY_EDITOR_LINUX
            Platform = KnownPlatforms.Linux.GetPlatform();
#endif
        }

        internal static string GetExecutableExtension(BuildTarget target)
        {
#pragma warning disable 618
            switch (target)
            {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                    return ".app";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";
                case BuildTarget.NoTarget:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.Stadia:
                    return string.Empty;
                case BuildTarget.Android:
                    if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
                        return "";
                    else if (EditorUserBuildSettings.buildAppBundle)
                        return ".aab";
                    else
                        return ".apk";
                case BuildTarget.Lumin:
                    return ".mpk";
                case BuildTarget.iOS:
                case BuildTarget.tvOS:
                default:
                    return "";
            }
#pragma warning restore 618
        }

        /// <summary>
        /// Attempt to map a given <see cref="Platforms.BuildTarget"/>
        /// to its corresponding <see cref="Build.Platform"/>.
        /// </summary>
        public static Platform ConstructPlatform(BuildTarget target)
        {
            var platform = target.GetPlatform();
            if (platform == null)
                throw new NotImplementedException($"Could not map {nameof(BuildTarget)} '{target}' to a known {nameof(Platform)}. (are you missing an assembly or package reference?)");

            return platform;
        }

        static BuildPipelineBase ConstructPipeline(Platform platform)
        {
            return platform != null && TypeConstructionUtility.TryConstructTypeDerivedFrom<BuildPipelineSelectorBase>(out var selector) ? selector.SelectFor(platform) : null;
        }

        class ClassicBuildProfileMigration : IJsonMigration<ClassicBuildProfile>
        {
            [InitializeOnLoadMethod]
            static void Register() => JsonSerialization.AddGlobalMigration(new ClassicBuildProfileMigration());

            public int Version => 1;

            public ClassicBuildProfile Migrate(JsonMigrationContext context)
            {
                context.TryRead<ClassicBuildProfile>(out var profile);
                if (context.SerializedVersion == 0)
                {
                    if (context.TryRead<BuildTarget>("Target", out var target))
                    {
                        if (target != BuildTarget.NoTarget)
                        {
                            profile.Platform = ConstructPlatform(target);
                        }
                    }
                }
                return profile;
            }
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    internal abstract class BuildPipelineSelectorBase
    {
        public abstract BuildPipelineBase SelectFor(Platform platform);
    }
}
