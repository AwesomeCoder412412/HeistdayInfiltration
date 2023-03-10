#if ENABLE_PLAYMODE_EXTENSION
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Build.Classic;
using Unity.Build.Common;
using UnityEditor;
using UnityEditor.TestRunner.TestLaunchers;
using UnityEditor.TestTools;
using UnityEditor.TestTools.TestRunner;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditor.TestTools.TestRunner.TestRun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestRunner.Utils;
using UnityEngine.TestTools.TestRunner;
using UnityEngine.TestTools.TestRunner.Callbacks;


namespace Unity.Build.Playmode.TestRunner
{
    [Serializable]
    internal class BuildConfigurationPlayerLauncher
    {
        private readonly BuildTarget m_TargetPlatform;
        private readonly Platform m_BuildConfigurationPlatform;
        private readonly BuildPlayerOptions m_BuildOptions;

        public BuildConfigurationPlayerLauncher(ExecutionSettings executionSettings, BuildPlayerOptions buildOptions)
        {
            m_BuildOptions = buildOptions;
            m_TargetPlatform = executionSettings.targetPlatform ?? EditorUserBuildSettings.activeBuildTarget;
            m_BuildConfigurationPlatform = m_TargetPlatform.GetPlatform() ?? throw new Exception($"Cannot resolve platform for {m_TargetPlatform}");
        }

        private static SceneList.SceneInfo GetSceneInfo(string path)
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            // Note: Don't ever set AutoLoad to true, the tests are responsible for loading scenes
            return new SceneList.SceneInfo() { AutoLoad = false, Scene = GlobalObjectId.GetGlobalObjectIdSlow(sceneAsset) };
        }

        private string GetBuildConfiguratioName()
        {
            var name = m_BuildConfigurationPlatform.Name;
            return name;
        }

        private BuildConfiguration CreateBuildConfiguration(string name)
        {
            var config = BuildConfiguration.CreateInstance();

            config.name = name;


            config.SetComponent(new SceneList
            {
                SceneInfos = new List<SceneList.SceneInfo>(m_BuildOptions.scenes.Select(GetSceneInfo))
            });

            var profile = new ClassicBuildProfile()
            {
                Configuration = BuildType.Develop,
                Platform = m_BuildConfigurationPlatform
            };

            config.SetComponent(profile);
            config.SetComponent(new PlaymodeTestRunnerComponent());

            config.SetComponent<OutputBuildDirectory>(new OutputBuildDirectory()
            {
                OutputDirectory = m_BuildOptions.locationPathName
            });

            config.SetComponent(new GeneralSettings()
            {
                CompanyName = PlayerSettings.companyName,
                ProductName = PlayerSettings.productName
            });

            return config;
        }

        public virtual void Run()
        {
            var name = GetBuildConfiguratioName();
            var path = $"Assets/{m_BuildConfigurationPlatform.Name}.buildConfiguration";
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, $"Creating build configuration at path {path}");
            var config = CreateBuildConfiguration(name);

            // In basic scenarios you can build without saving build configuration to disk
            // But dots related systems, require build configuration to be present on disk
            config.SerializeToPath(path);
            AssetDatabase.Refresh();

            var buildResult = config.Build();
            AssetDatabase.DeleteAsset(path);
            buildResult.LogResult();

            if (buildResult.Failed)
            {
                Debug.LogError("Player build failed");
                throw new TestLaunchFailedException("Player build failed");
            }

            var runResult = config.Run();
            runResult.LogResult();
            if (runResult.Failed)
            {
                throw new TestLaunchFailedException("Player run failed");
            }
        }
    }
}
#endif
