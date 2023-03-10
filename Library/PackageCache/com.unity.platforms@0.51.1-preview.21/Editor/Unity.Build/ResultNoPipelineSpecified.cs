namespace Unity.Build
{
    /// <summary>
    /// Container representing a failure caused by no pipeline specified.
    /// </summary>
    public sealed class ResultNoPipelineSpecified : ResultFailure
    {
        public ResultNoPipelineSpecified() : base("The selected build configuration has no pipeline specified.") { }
    }
}
