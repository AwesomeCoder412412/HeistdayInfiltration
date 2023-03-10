using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Properties.Editor;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Unity.Build
{
    /// <summary>
    /// Platform base class.
    /// </summary>
    public abstract partial class Platform : IEquatable<Platform>
    {
        static readonly Platform[] s_AvailablePlatforms;
        internal readonly PlatformInfo m_PlatformInfo;

        /// <summary>
        /// All available platforms.
        /// </summary>
        public static IEnumerable<Platform> AvailablePlatforms => s_AvailablePlatforms;

        /// <summary>
        /// Platform short name. Used by serialization.
        /// </summary>
        public string Name => m_PlatformInfo.Name;

        /// <summary>
        /// Platform display name. Used for displaying on user interface.
        /// </summary>
        public string DisplayName => m_PlatformInfo.DisplayName;

        /// <summary>
        /// Platform icon file path.
        /// </summary>
        public string IconPath => m_PlatformInfo.IconPath;

        /// <summary>
        /// Platform package identifier.
        /// </summary>
        public string PackageId => m_PlatformInfo.PackageId;

        /// <summary>
        /// Determine if the platform has a known package.
        /// </summary>
        public bool HasPackage => !string.IsNullOrEmpty(PackageId);

        /// <summary>
        /// Determine if the platform package is installed.
        /// </summary>
        public bool IsPackageInstalled => HasPackage ? PackageInfo.FindForAssetPath($"Packages/{PackageId}/package.json") != null : false;

        /// <summary>
        /// Determine if the platform is public, or closed (require license to use).
        /// </summary>
        public bool IsPublic => true;

        /// <summary>
        /// Start installation of platform package.
        /// </summary>
        public void InstallPackage()
        {
            if (!HasPackage || IsPackageInstalled)
                return;

            var request = UnityEditor.PackageManager.Client.Add(PackageId);
            if (request.Status == UnityEditor.PackageManager.StatusCode.Failure)
                Debug.LogError(request.Error.message);
            else
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Started installation of {0} platform package [{1}].", DisplayName, PackageId);
        }

        /// <summary>
        /// Get current platform name.
        /// </summary>
        /// <param name="legacyName"></param>
        /// <returns>A the current platform name if the legacy name is found. Returns the input parameter otherwise.</returns>
        public static string GetNameFromLegacyName(string legacyName)
        {
            string name = legacyName.ToLowerInvariant();
            if (name == "windows" || name == "win")
                name = "StandaloneWindows64";
            else if (name == "osx" || name == "macos")
                name = "StandaloneOSX";
            else if (name == "linux")
                name = "StandaloneLinux64";
            else if (name == "ios")
                name = "iOS";
            else if (name == "android")
                name = "Android";
            else if (name == "webgl" || name == "web")
                name = "WebGL";
            else if (name == "wsa" || name == "uwp")    //@todo: Is uwp good in here?
                name = "WSAPlayer";
            else if (name == "ps4")
                name = "PS4";
            else if (name == "xboxone" || name == "xb1")
                name = "XboxOne";
            else if (name == "tvos")
                name = "tvOS";
            else if (name == "switch")
                name = "Switch";
            else if (name == "stadia")
                name = "Stadia";
            else if (name == "lumin")
                name = "Lumin";
            return name;
        }

        /// <summary>
        /// Get platform by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A <see cref="Platform"/> instance if found, <see langword="null"/> otherwise.</returns>
        public static Platform GetPlatformByName(string name)
        {
            var platform = s_AvailablePlatforms.FirstOrDefault(p => p.Name == name);

            // Check for platform former name.
            // Dot not change these values, they are only used to deserialize old build assets.
            // This list does not need to be updated when adding new platforms.
            if (platform == null)
            {
                name = GetNameFromLegacyName(name);
                platform = s_AvailablePlatforms.FirstOrDefault(p => p.Name == name);
            }
            return platform;
        }

        static Platform()
        {
            var platforms = TypeCache.GetTypesDerivedFrom<Platform>()
                .Where(type => type != typeof(MissingPlatform))
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .Where(type => !type.HasAttribute<ObsoleteAttribute>())
                .Where(TypeConstruction.CanBeConstructed)
                .Select(TypeConstruction.Construct<Platform>);

            var platformsByName = new Dictionary<string, Platform>();
            foreach (var platform in platforms)
            {
                if (platformsByName.TryGetValue(platform.Name, out var registeredPlatform))
                    throw new InvalidOperationException($"Duplicate platform name found. Platform named '{platform.Name}' is already registered by class '{registeredPlatform.GetType().FullName}'.");

                platformsByName.Add(platform.Name, platform);
            }

            // Fill up missing platforms
            var enumFields = typeof(BuildTarget).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var field in enumFields)
            {
                var buildTargetName = field.Name;
                var buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), buildTargetName);
                if (buildTarget == BuildTarget.NoTarget ||
                    buildTarget == BuildTarget.StandaloneWindows)
                    continue;

                bool isObsolete = false;
                foreach (var attribute in field.CustomAttributes)
                {
                    if (attribute.AttributeType == typeof(ObsoleteAttribute))
                    {
                        isObsolete = true;
                        break;
                    }
                }
                if (isObsolete)
                    continue;

                if (platformsByName.ContainsKey(buildTargetName))
                    continue;

                platformsByName.Add(buildTargetName, new MissingPlatform(buildTargetName));
            }

            s_AvailablePlatforms = platformsByName.Values.ToArray();
        }

        internal Platform(PlatformInfo info)
        {
            m_PlatformInfo = info;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Platform);
        }

        public bool Equals(Platform other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (ReferenceEquals(null, other))
                return false;

            return m_PlatformInfo.Equals(other.m_PlatformInfo);
        }

        public static bool operator ==(Platform lhs, Platform rhs)
        {
            if (ReferenceEquals(lhs, null))
                return ReferenceEquals(rhs, null);

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Platform lhs, Platform rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return m_PlatformInfo.GetHashCode();
        }
    }
}
