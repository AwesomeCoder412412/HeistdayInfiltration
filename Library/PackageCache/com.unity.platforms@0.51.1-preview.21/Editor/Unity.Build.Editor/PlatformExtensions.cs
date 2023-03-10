using System.Collections.Generic;
using UnityEngine;

namespace Unity.Build.Editor
{
    public static class PlatformExtensions
    {
        static Dictionary<Platform, Texture2D> s_PlatformIcons;

        public static Texture2D GetIcon(this Platform platform)
        {
            if (platform == null)
                return null;

            if (s_PlatformIcons == null)
                s_PlatformIcons = new Dictionary<Platform, Texture2D>();

            if (s_PlatformIcons.TryGetValue(platform, out var icon))
                return icon;

            icon = Package.LoadResource<Texture2D>(platform.IconPath, true);
            s_PlatformIcons[platform] = icon;
            return icon;
        }
    }
}
