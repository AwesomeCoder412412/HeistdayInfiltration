using UnityEngine;

namespace Unity.Build.Editor
{
    static class Resources
    {
        // UI Templates
        public static UITemplate BuildConfiguration = new UITemplate("build-configuration");
        public static UITemplate BuildConfigurationDependency = new UITemplate("build-configuration-dependency");
        public static UITemplate BuildComponent = new UITemplate("build-component");
        public static UITemplate ClassicBuildProfile = new UITemplate("classic-build-profile");
        public static UITemplate TypeInspector = new UITemplate("type-inspector");

        // UI Icons
        public static Texture2D BuildComponentIcon = Package.LoadResource<Texture2D>("icons/component.png", true);
    }
}
