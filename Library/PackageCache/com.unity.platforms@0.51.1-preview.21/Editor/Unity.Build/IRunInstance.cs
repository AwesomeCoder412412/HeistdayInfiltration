using System;

namespace Unity.Build
{
    /// <summary>
    /// Base interface for all run instance classes.
    /// </summary>
    public interface IRunInstance : IDisposable
    {
        /// <summary>
        /// Determine if the instance is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Kill the instance.
        /// </summary>
        void Kill();
    }

    /// <summary>
    /// Provides a default run instance implementation.
    /// </summary>
    public class DefaultRunInstance : IRunInstance
    {
        /// <summary>
        /// Determine if the instance is running.
        /// </summary>
        public virtual bool IsRunning => throw new NotImplementedException();

        /// <summary>
        /// Kill the instance.
        /// </summary>
        public virtual void Kill() => throw new NotImplementedException();

        public void Dispose() { }
    }
}
