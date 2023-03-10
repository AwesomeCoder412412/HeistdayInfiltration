namespace Unity.Build
{
    /// <summary>
    /// Container representing a failure caused by a missing platform package.
    /// </summary>
    public sealed class ResultPackageNotInstalled : ResultFailure
    {
        public ResultPackageNotInstalled(Platform platform) :
            base($"The selected build configuration platform requires {platform.DisplayName} package [{platform.PackageId}] to be installed.")
        {
        }
    }
}
