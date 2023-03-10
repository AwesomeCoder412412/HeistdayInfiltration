using System;
using Unity.Properties;
using UnityEngine;

namespace Unity.Build
{
    /// <summary>
    /// Base class for results.
    /// </summary>
    public abstract class ResultBase
    {
        /// <summary>
        /// Determine whether or not the operation succeeded.
        /// </summary>
        [CreateProperty] public bool Succeeded { get; internal set; }

        /// <summary>
        /// Determine whether or not the operation failed.
        /// </summary>
        public bool Failed => !Succeeded;

        /// <summary>
        /// Message attached to this result.
        /// </summary>
        [CreateProperty] public string Message { get; internal set; }

        /// <summary>
        /// Exception attached to this result.
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// Log the result to the console window.
        /// </summary>
        public virtual void LogResult()
        {
            if (Succeeded)
            {
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "{0}", ToString());
            }
            else
            {
                if (Exception == null)
                    Debug.LogError(ToString(), null);
                else
                    Debug.LogException(Exception, null);
            }
        }

        /// <summary>
        /// Implicit conversion to <see cref="bool"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        public static implicit operator bool(ResultBase result)
        {
            return result.Succeeded;
        }

        public override string ToString()
        {
            var result = Succeeded ? "Succeeded" : "Failed";
            var message = Failed && !string.IsNullOrEmpty(Message) ? ": " + Message : string.Empty;
            return (result + message).Trim(' ');
        }

        [Obsolete("Result has been replaced by Succeeded. (RemovedAfter 2021-03-01)")]
        public bool Result
        {
            get => Succeeded;
            private set => Succeeded = value;
        }

        [Obsolete("Reason has been replaced by Message. (RemovedAfter 2021-03-01)")]
        public string Reason
        {
            get => Message;
            private set => Message = value;
        }
    }

    public static class Result
    {
        public static ResultBase Success() => new ResultSuccess();
        public static ResultBase Failure(string message) => new ResultFailure(message);
    }
}
