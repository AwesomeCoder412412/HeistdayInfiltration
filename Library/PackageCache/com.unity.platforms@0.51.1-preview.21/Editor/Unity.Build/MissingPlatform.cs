using System;

namespace Unity.Build
{
    /// <summary>
    /// Describes a platform for which the type could not be resolved.
    /// </summary>
    sealed class MissingPlatform : Platform, ICloneable
    {
        public MissingPlatform(string name) : base(ComposeMissingPlatformInfo(name)) { }
        MissingPlatform(PlatformInfo info) : base(info) { }
        public object Clone() => new MissingPlatform(new PlatformInfo(Name, DisplayName, PackageId, IconPath));

        private static PlatformInfo ComposeMissingPlatformInfo(string name)
        {
            var info = KnownPlatforms.GetPlatformInfo(name);
            if (info != null)
            {
                // We reset the package info as we don't have a package
                info = new PlatformInfo(info.Name, info.DisplayName, null, info.IconPath);
            }
            else
                info = new PlatformInfo(name, name, null, null);
            return info;
        }
    }
}
