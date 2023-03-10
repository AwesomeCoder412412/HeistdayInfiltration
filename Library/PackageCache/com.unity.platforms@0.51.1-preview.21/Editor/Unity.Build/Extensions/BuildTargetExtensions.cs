using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Unity.Build
{
    /// <summary>
    /// Extensions of <see cref="BuildTarget"/> enum type.
    /// </summary>
    public static class BuildTargetExtensions
    {
        /// <summary>
        /// Retrieve the corresponding <see cref="Platform"/> of this <see cref="BuildTarget"/>.
        /// </summary>
        /// <param name="buildTarget">The build target.</param>
        /// <returns>The corresponding <see cref="Platform"/> if found, <see langword="null"/> otherwise.</returns>
        public static Platform GetPlatform(this BuildTarget buildTarget)
        {
            return Platform.GetPlatformByName(buildTarget.GetPlatformName());
        }

        /// <summary>
        /// Retrieve the corresponding <see cref="BuildTarget"/> of this <see cref="Platform"/>.
        /// </summary>
        /// <param name="platform"></param>
        /// <returns>The corresponding <see cref="BuildTarget"/> if found, throws otherwise.</returns>
        public static BuildTarget GetBuildTarget(this Platform platform)
        {
            BuildTarget buildTarget;
            if (Enum.TryParse(platform.Name, out buildTarget))
                return buildTarget;

            throw new NotImplementedException($"Could not map platform {platform.Name} to a {typeof(BuildTarget).FullName} value.");
        }

        /// <summary>
        /// Retrieve the corresponding <see cref="BuildTargetGroup"/> of this <see cref="Platform"/>.
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static BuildTargetGroup GetBuildTargetGroup(this Platform platform)
        {
            return BuildPipeline.GetBuildTargetGroup(platform.GetBuildTarget());
        }

        internal static string GetPlatformName(this BuildTarget buildTarget)
        {
            return buildTarget.ToString();
        }
    }
}
