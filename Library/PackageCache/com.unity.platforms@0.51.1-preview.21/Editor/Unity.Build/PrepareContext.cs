using System;

namespace Unity.Build
{
    public sealed class PrepareContext
    {
        public BuildConfiguration BuildConfiguration { get; }
        public Platform Platform { get; }

        public PrepareContext(BuildConfiguration config)
        {
            BuildConfiguration = config ?? throw new ArgumentNullException(nameof(config));
            Platform = config?.GetPlatform();
        }
    }
}
