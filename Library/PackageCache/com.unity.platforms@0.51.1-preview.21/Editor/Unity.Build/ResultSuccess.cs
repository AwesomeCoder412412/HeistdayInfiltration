namespace Unity.Build
{
    /// <summary>
    /// Container representing a successful result.
    /// </summary>
    public sealed class ResultSuccess : ResultBase
    {
        public ResultSuccess()
        {
            Succeeded = true;
        }
    }
}
