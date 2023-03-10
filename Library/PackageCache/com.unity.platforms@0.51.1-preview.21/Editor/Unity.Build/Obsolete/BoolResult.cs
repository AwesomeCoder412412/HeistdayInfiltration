using System;

namespace Unity.Build
{
    [Obsolete("BoolResult will be removed and replaced with classes deriving from ResultBase. (RemovedAfter 2021-03-01)")]
    public struct BoolResult
    {
        public bool Result { get; private set; }

        public string Reason { get; private set; }

        [Obsolete("Replace with Result.Success(*). (RemovedAfter 2021-03-01)")]
        public static BoolResult True() => new BoolResult { Result = true, Reason = null };

        [Obsolete("Replace with Result.Failure(*). (RemovedAfter 2021-03-01)")]
        public static BoolResult False(string reason) => new BoolResult { Result = false, Reason = reason };

        public static implicit operator bool(BoolResult value) => value.Result;
    }
}
