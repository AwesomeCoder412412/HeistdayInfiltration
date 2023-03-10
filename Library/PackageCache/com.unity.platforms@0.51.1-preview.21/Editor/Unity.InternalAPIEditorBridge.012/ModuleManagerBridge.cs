using UnityEditor;
using UnityEditor.Modules;

namespace Unity.Build.Bridge
{
    static class ModuleManagerBridge
    {
        public static string GetTargetStringFrom(BuildTargetGroup group, BuildTarget target)
        {
            return ModuleManager.GetTargetStringFrom(group, target);
        }
    }
}
