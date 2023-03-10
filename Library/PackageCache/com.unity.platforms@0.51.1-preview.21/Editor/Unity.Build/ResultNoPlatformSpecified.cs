namespace Unity.Build
{
    /// <summary>
    /// Container representing a failure caused by no platform specified.
    /// </summary>
    public sealed class ResultNoPlatformSpecified : ResultFailure
    {
        public ResultNoPlatformSpecified() : base("The selected build configuration has no platform specified.") { }
    }
}
