using System;

namespace Unity.Build
{
    /// <summary>
    /// Provides information about platform.
    /// </summary>
    sealed class PlatformInfo : IEquatable<PlatformInfo>
    {
        /// <summary>
        /// Platform short name. Used by serialization.
        /// <remarks>
        /// Warning: Changing it might break serialization.
        /// </remarks>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Platform display name. Used for displaying on user interface.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Platform package identifier.
        /// </summary>
        public string PackageId { get; }

        /// <summary>
        /// Platform icon file path.
        /// </summary>
        public string IconPath { get; }

        /// <summary>
        /// Construct a new <see cref="PlatformInfo"/> instance.
        /// </summary>
        /// <param name="name">The <see cref="Platform"/> short name, used by serialization.</param>
        /// <param name="displayName">The <see cref="Platform"/> display name, used by user interface.</param>
        /// <param name="packageId">The package identifier that contains the <see cref="Platform"/> type.</param>
        /// <param name="iconPath">The <see cref="Platform"/> icon name, used by user interface.</param>
        internal PlatformInfo(string name, string displayName, string packageId, string iconPath)
        {
            Name = name;
            DisplayName = displayName;
            PackageId = packageId;
            IconPath = iconPath;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PlatformInfo);
        }

        public bool Equals(PlatformInfo other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (ReferenceEquals(null, other))
                return false;

            return Name == other.Name;
        }

        public static bool operator ==(PlatformInfo lhs, PlatformInfo rhs)
        {
            if (ReferenceEquals(lhs, null))
                return ReferenceEquals(rhs, null);

            return lhs.Equals(rhs);
        }

        public static bool operator !=(PlatformInfo lhs, PlatformInfo rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }
}
