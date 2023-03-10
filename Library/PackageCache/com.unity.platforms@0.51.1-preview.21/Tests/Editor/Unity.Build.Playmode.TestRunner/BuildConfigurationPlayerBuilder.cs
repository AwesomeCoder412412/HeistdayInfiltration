#if ENABLE_PLAYMODE_EXTENSION
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.TestRunner.TestLaunchers;
using UnityEditor.TestTools.TestRunner;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor.TestTools.TestRunner.TestRun;
using UnityEditor.TestTools.TestRunner.TestRun.Tasks;
using UnityEngine;
using UnityEngine.TestTools.TestRunner;


namespace Unity.Build.Playmode.TestRunner
{
    class BuildConfigurationPlayerBuilder : IPlayerBuilder
    {
        public string Name
        {
            get => "BuildConfiguration";
        }

        public bool AlwaysUseDirectoryForLocationPath { get => true; }

        public IEnumerator BuildAndRun(ExecutionSettings settings, BuildPlayerOptions buildOptions)
        {
            var launcher = new BuildConfigurationPlayerLauncher(settings, buildOptions);
            launcher.Run();
            yield return null;
        }
    }
}
#endif
