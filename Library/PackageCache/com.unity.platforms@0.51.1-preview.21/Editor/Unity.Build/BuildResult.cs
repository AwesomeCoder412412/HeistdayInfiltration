using System;

namespace Unity.Build
{
    /// <summary>
    /// Container for results happening when building a build pipeline.
    /// </summary>
    public sealed class BuildResult : BuildPipelineResult
    {
        /// <summary>
        /// Construct a <see cref="BuildResult"/> from a <see cref="ResultBase"/>.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        /// <param name="config">The build configuration.</param>
        /// <param name="result">The original result.</param>
        public BuildResult(BuildPipelineBase pipeline, BuildConfiguration config, ResultBase result)
        {
            Succeeded = result.Succeeded;
            BuildPipeline = pipeline;
            BuildConfiguration = config;
            Message = result.Message;
            Exception = result.Exception;
        }

        /// <summary>
        /// Get a build result representing a success.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        /// <param name="config">The build configuration.</param>
        /// <returns>A new build result instance.</returns>
        public static BuildResult Success(BuildPipelineBase pipeline, BuildConfiguration config) => new BuildResult
        {
            Succeeded = true,
            BuildPipeline = pipeline,
            BuildConfiguration = config
        };

        /// <summary>
        /// Get a build result representing a failure.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        /// <param name="config">The build configuration.</param>
        /// <param name="reason">The reason of the failure.</param>
        /// <returns>A new build result instance.</returns>
        public static BuildResult Failure(BuildPipelineBase pipeline, BuildConfiguration config, string reason) => new BuildResult
        {
            Succeeded = false,
            BuildPipeline = pipeline,
            BuildConfiguration = config,
            Message = reason
        };

        /// <summary>
        /// Get a build result representing a failure.
        /// </summary>
        /// <param name="pipeline">The build pipeline.</param>
        /// <param name="config">The build configuration.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <returns>A new build result instance.</returns>
        public static BuildResult Failure(BuildPipelineBase pipeline, BuildConfiguration config, Exception exception) => new BuildResult
        {
            Succeeded = false,
            BuildPipeline = pipeline,
            BuildConfiguration = config,
            Exception = exception
        };

        string GetOpenBuildDirectory()
        {
            if (Failed)
                return string.Empty;

            var directory = BuildPipeline.GetOutputBuildDirectory(BuildConfiguration);
            return directory != null ? $"\nResult can be found in {directory.ToHyperLink()}." : string.Empty;
        }

        public override string ToString() => $"Build {base.ToString()}{GetOpenBuildDirectory()}";

        public BuildResult() { }
    }
}
