# Performance Testing Extension for Unity Test Framework

The Unity Performance Testing package extends Unity Test Framework with performance testing capabilities. It provides an API and test case decorators for taking measurements/samples of Unity profiler markers, and other custom metrics, in the Unity Editor and built players. It also collects configuration metadata that is useful for comparing data across different hardware and configurations, such as build and player settings.

The Performance Testing Extension is intended for use with the Unity Test Framework. You should be familiar with how to create and run tests as described in the [Unity Test Framework documentation](https://docs.unity3d.com/Packages/com.unity.test-framework@latest/manual/index.html).


> **Note:** When tests are run with Unity Test Framework, a development player is always built to support communication between the editor and player, effectively overriding the development build setting from the build settings UI or scripting API.

## Installing the Performance Testing Extension

To install the Performance Testing Extension package
1. Open the `manifest.json` file for your Unity project (located in the YourProject/Packages directory) in a text editor.
2. Add `"com.unity.test-framework.performance": "2.5.0",` to the dependencies.
3. Save the manifest.json file.
4. Verify the Performance Testing Extension is now installed by opening the Unity Package Manager window.

To access performance testing apis, add a reference to `Unity.PerformanceTesting` in your assembly definition.

## Unity alpha version compatibility

Unity alpha releases can often include changes that break compatibility with the Performance Testing Extension, so we cannot currently guarantee compatability with every Unity alpha version. The table below shows which version of the package is compatible with which alpha releases.

| Unity version             | Package version |
| ------------------------- | --------------- |
| 2019.2.0a10 - latest      | 1.2.3-preview+  |
| 2019.2.0a1 - 2019.2.0a10  | 1.0.9-preview   |
| 2019.1.0a10 - 2019.2.0a1  | 0.1.50-preview  |
| 2019.1.0a01 - 2019.1.0a10 | 0.1.42-preview  |
| Older versions            | 0.1.50-preview  |

## Tips

### Project settings

- Remove all but one of the [Quality level settings](https://docs.unity3d.com/Manual/class-QualitySettings.html) in **Project Settings > Quality**. Otherwise, you may have different configurations when running on different platforms. If you require different settings per platform then make sure they are being set as expected.
- Disable VSync under **Project Settings > Quality**. Some platforms like Android have a forced VSync and this will not be possible.
- Disable HW Reporting **PlayerSettings -> Other -> Disable HW Reporting**.
- Remove camera and run in batchmode if you are not measuring rendering.

### Generating assets

Use [IPrebuildSetup](https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/api/UnityEngine.TestTools.IPrebuildSetup.html) attribute when you need to generate assets.
Place assets in Resources or [StreamingAssets](https://docs.unity3d.com/Manual/SpecialFolders.html) folders, scenes can be placed anywhere in the project, but should be added to build settings.

#### Example 1: IPrebuildSetup implementation

``` csharp
public class TestsWithPrebuildStep : IPrebuildSetup
{
    public void Setup()
    {
        // this code is executed before entering playmode or the player is executed
    }
}

public class MyAmazingPerformanceTest
{
    [Test, Performance]
    [PrebuildSetup(typeof(TestsWithPrebuildStep))]
    public void Test()
    {
        ...
    }
}
```

When loading scenes in IPrebuildSetup you have to use `LoadSceneMode.Additive`.

#### Example 2: Using EditorSceneManager to create new scenes additively, save and add them to build settings.

``` csharp
private static string m_ArtifactsPath = "Assets/Artifacts/";

public static Scene NewScene(NewSceneSetup setup)
{
    Scene scene = EditorSceneManager.NewScene(setup, NewSceneMode.Additive);
    EditorSceneManager.SetActiveScene(scene);
    return scene;
}

public static void SaveScene(Scene scene, string name, bool closeScene = true)
{
    EditorSceneManager.SaveScene(scene, GetScenePath(name));

    if (closeScene)
    {
        foreach (var sceneSetting in EditorBuildSettings.scenes)
            if (sceneSetting.path == GetScenePath((name)))
                return;

        EditorSceneManager.CloseScene(scene, true);
        EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneAt(0));

        var newListOfScenes = new List<EditorBuildSettingsScene>();
        newListOfScenes.Add(new EditorBuildSettingsScene(GetScenePath(name), true));
        newListOfScenes.AddRange(EditorBuildSettings.scenes);
        EditorBuildSettings.scenes = newListOfScenes.ToArray();
    }
}

public static string GetScenePath(string name)
{
    return m_ArtifactsPath + name + ".unity";
}
```
