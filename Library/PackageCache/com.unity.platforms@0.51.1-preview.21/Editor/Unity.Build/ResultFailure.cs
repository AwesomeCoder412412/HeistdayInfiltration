namespace Unity.Build
{
    /// <summary>
    /// Container representing a failed result.
    /// </summary>
    public class ResultFailure : ResultBase
    {
        public ResultFailure(string message)
        {
            Succeeded = false;
            Message = message;
        }
    }
}
