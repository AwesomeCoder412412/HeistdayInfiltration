#if ENABLE_PLAYMODE_EXTENSION
using System;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor;
using UnityEditor.TestTools.TestRunner;
using UnityEngine;

namespace Unity.Build.Playmode.TestRunner
{
    internal class BuildConfigurationPlayerLauncherContextSettings : PlayerLauncherContextSettings
    {
        public BuildConfigurationPlayerLauncherContextSettings(ITestRunSettings overloadSettings)
            :base (overloadSettings)
        {
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
#endif
