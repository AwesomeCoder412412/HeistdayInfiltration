using System;

namespace Bee.Core
{
    [Obsolete("Please switch to Unity.Build namespace (This class will be removed after 2021-03-01)", true)]
    public abstract class Platform
    {
        public string Name => throw new NotImplementedException();
        public string DisplayName => throw new NotImplementedException();
    }

    [Obsolete("Replace with Platform.Windows from com.unity.build.windows package. (RemovedAfter 2021-03-01)", true)]
    public class WindowsPlatform : Platform { }

    [Obsolete("Replace with Platform.macOS from com.unity.build.macos package. (RemovedAfter 2021-03-01)", true)]
    public class MacOSXPlatform : Platform { }

    [Obsolete("Replace with Platform.iOS from com.unity.build.ios package. (RemovedAfter 2021-03-01)", true)]
    public class IosPlatform : Platform { }

    [Obsolete("Replace with Platform.Android from com.unity.build.android package. (RemovedAfter 2021-03-01)", true)]
    public class AndroidPlatform : Platform { }

    [Obsolete("Replace with Platform.Linux from com.unity.build.linux package. (RemovedAfter 2021-03-01)", true)]
    public class LinuxPlatform : Platform { }

    [Obsolete("Replace with Platform.Web from com.unity.build.web package. (RemovedAfter 2021-03-01)", true)]
    public class WebGLPlatform : Platform { }

    [Obsolete("Replace with KnownPlatforms.tvOS.GetPlatform(). (RemovedAfter 2021-03-01)", true)]
    public class TvosPlatform : Platform { }

    [Obsolete("Replace with KnownPlatforms.UniversalWindowsPlatform.GetPlatform(). (RemovedAfter 2021-03-01)", true)]
    public class UniversalWindowsPlatform : Platform { }

    [Obsolete("Replace with KnownPlatforms.XboxOne.GetPlatform(). (RemovedAfter 2021-03-01)", true)]
    public class XboxOnePlatform : Platform { }

    [Obsolete("Replace with KnownPlatforms.Switch.GetPlatform(). (RemovedAfter 2021-03-01)", true)]
    public class SwitchPlatform : Platform { }

    [Obsolete("Replace with KnownPlatforms.PlayStation4.GetPlatform(). (RemovedAfter 2021-03-01)", true)]
    public class PS4Platform : Platform { }

    [Obsolete("Replace with KnownPlatforms.Stadia.GetPlatform(). (RemovedAfter 2021-03-01)", true)]
    public class StadiaPlatform : Platform { }

    [Obsolete("Replace with KnownPlatforms.Lumin.GetPlatform(). (RemovedAfter 2021-03-01)", true)]
    public class LuminPlatform : Platform { }
}

