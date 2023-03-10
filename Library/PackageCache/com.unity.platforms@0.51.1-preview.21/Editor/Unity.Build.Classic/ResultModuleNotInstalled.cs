namespace Unity.Build.Classic
{
    /// <summary>
    /// Container representing a failure caused by a missing platform module.
    /// </summary>
    public sealed class ResultModuleNotInstalled : ResultFailure
    {
        public ResultModuleNotInstalled(Platform platform) :
            base($"The selected build configuration platform requires {platform.DisplayName} platform module to be installed.")
        {
        }
    }
}
