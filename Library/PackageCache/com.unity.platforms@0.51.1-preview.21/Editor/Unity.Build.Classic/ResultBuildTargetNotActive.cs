namespace Unity.Build.Classic
{
    /// <summary>
    /// Container representing a failure caused by the selected build target not being active.
    /// </summary>
    public sealed class ResultBuildTargetNotActive : ResultFailure
    {
        public ResultBuildTargetNotActive(Platform platform) :
            base($"The selected build configuration platform requires active build target to be {platform.DisplayName}.")
        {
        }
    }
}
