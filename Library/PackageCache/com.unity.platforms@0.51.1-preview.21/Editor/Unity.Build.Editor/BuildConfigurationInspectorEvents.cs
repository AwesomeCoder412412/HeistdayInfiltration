using System;

namespace Unity.Build.Editor
{
    /// <summary>
    /// Utility class to listen on build configuration inspector events.
    /// </summary>
    public static class BuildConfigurationInspectorEvents
    {
        /// <summary>
        /// Event fired after user click build action button.
        /// Return <see langword="true"/> if the event is handled, and stop propagation.
        /// </summary>
        public static event Func<BuildConfiguration, ResultBase, bool> AfterBuildAction;

        internal static bool OnAfterBuildAction(BuildConfiguration config, ResultBase result) => AfterBuildAction?.Invoke(config, result) ?? false;
    }
}
