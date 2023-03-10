using System;
using System.Diagnostics;
using Unity.Properties;
using Unity.Serialization;
using UnityEngine;

namespace Unity.Build.Common
{
    /// <summary>
    /// Represents the method that will handle player process output, line by line.
    /// </summary>
    public delegate void ProcessOutputHandler(object sender, string line);

    public sealed class RunSettings : IBuildComponent
    {
        /// <summary>
        /// Gets or sets whether the player will be configured
        /// to make process output (stdout, stderr) available to the API.
        /// </summary>
        [CreateProperty]
        public bool RedirectOutput { set; get; } = false;

        /// <summary>
        /// Gets or sets a user callback that can be used to process player stdout, line by line.
        /// </summary>
        [CreateProperty, HideInInspector, DontSerialize]
        public ProcessOutputHandler Stdout { set; get; } = null;

        /// <summary>
        /// Gets or sets a user callback that can be used to process player stderr, line by line.
        /// </summary>
        [CreateProperty, HideInInspector, DontSerialize]
        public ProcessOutputHandler Stderr { set; get; } = null;

        /// <summary>
        /// Gets or sets whether the player will be run in batch mode.
        /// </summary>
        [CreateProperty]
        public bool BatchMode { set; get; } = false;

        /// <summary>
        /// Gets or sets whether the player will be run with the "nographics" flag enabled.
        /// Requires <see cref="BatchMode"/> to be enabled.
        /// </summary>
        [CreateProperty]
        public bool NoGraphics { set; get; } = false;

        /// <summary>
        /// Gets or sets an array of extra arguments passed to the running process.
        /// </summary>
        [CreateProperty]
        public string[] ExtraArguments { set; get; } = Array.Empty<string>();
    }
}
