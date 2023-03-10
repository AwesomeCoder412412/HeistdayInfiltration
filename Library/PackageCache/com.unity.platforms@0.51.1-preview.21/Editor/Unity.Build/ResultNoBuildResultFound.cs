namespace Unity.Build
{
    /// <summary>
    /// Container representing a failure caused because of missing build result.
    /// </summary>
    public sealed class ResultNoBuildResultFound : ResultFailure
    {
        public ResultNoBuildResultFound(BuildConfiguration config) : base($"No build result found for {config.ToHyperLink()}.") { }
    }
}
