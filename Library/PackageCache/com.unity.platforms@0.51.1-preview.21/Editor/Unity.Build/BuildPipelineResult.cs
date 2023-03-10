using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Build
{
    /// <summary>
    /// Base class for results involving pipeline execution.
    /// </summary>
    public abstract class BuildPipelineResult : ResultBase
    {
        /// <summary>
        /// The build pipeline used in the operation.
        /// </summary>
        [CreateProperty] public BuildPipelineBase BuildPipeline { get; internal set; }

        /// <summary>
        /// The build configuration used in the operation.
        /// </summary>
        [CreateProperty] public BuildConfiguration BuildConfiguration { get; internal set; }

        /// <summary>
        /// The start time of the operation
        /// </summary>
        [CreateProperty] public DateTime StartTime { get; internal set; }

        /// <summary>
        /// The duration of the operation.
        /// </summary>
        [CreateProperty] public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// Log the result to the console window.
        /// </summary>
        public override void LogResult()
        {
            if (Succeeded)
            {
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, BuildConfiguration, "{0}", ToString());
            }
            else
            {
                if (Exception == null)
                    Debug.LogError(ToString(), BuildConfiguration);
                else
                    Debug.LogException(Exception, BuildConfiguration);
            }
        }

        public override string ToString()
        {
            var what = BuildConfiguration.ToHyperLink();
            var result = Succeeded ? "succeeded" : "failed";
            var message = Failed && !string.IsNullOrEmpty(Message) ? "\n" + Message : string.Empty;
            return $"{what} {result} after {Duration.ToShortString()}.{message}".Trim(' ');
        }
    }
}
