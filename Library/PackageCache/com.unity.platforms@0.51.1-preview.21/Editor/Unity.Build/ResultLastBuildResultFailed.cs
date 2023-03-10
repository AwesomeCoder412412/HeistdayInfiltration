namespace Unity.Build
{
    /// <summary>
    /// Container representing a failure caused from the last build result that failed.
    /// </summary>
    public sealed class ResultLastBuildResultFailed : ResultFailure
    {
        public ResultLastBuildResultFailed(BuildResult result) : base($"Last build failed with error:\n{result.Message}") { }
    }
}
