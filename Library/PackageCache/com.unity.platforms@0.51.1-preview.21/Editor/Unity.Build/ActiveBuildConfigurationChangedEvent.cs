namespace Unity.Build
{
    /// <summary>
    /// Active build configuration changed event container.
    /// </summary>
    public struct ActiveBuildConfigurationChangedEvent
    {
        /// <summary>
        /// The build configuration that is now set active.
        /// </summary>
        public readonly BuildConfiguration NewValue;

        /// <summary>
        /// The build configuration that was previously set active.
        /// </summary>
        public readonly BuildConfiguration OldValue;

        /// <summary>
        /// Construct a new ActiveBuildConfigurationChangedEvent.
        /// </summary>
        /// <param name="newValue">The build configuration that is now set active.</param>
        /// <param name="oldValue">The build configuration that was previously set active.</param>
        public ActiveBuildConfigurationChangedEvent(BuildConfiguration newValue, BuildConfiguration oldValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}
