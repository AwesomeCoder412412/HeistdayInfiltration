#if !UNITY_2021_2_OR_NEWER
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Unity.Build.Bridge
{
    internal static class EditorGUIBridge
    {
        [InitializeOnLoadMethod]
        static void Register()
        {
            EditorGUI.hyperLinkClicked += (sender, args) =>
            {
                var hyperLinkArgs = new Dictionary<string, string>();
                if (args is EditorGUILayout.HyperLinkClickedEventArgs editorArgs)
                {
                    foreach (var pair in editorArgs.hyperlinkInfos)
                    {
                        hyperLinkArgs.Add(pair.Key, pair.Value);
                    }
                }
                HyperLinkClicked?.Invoke(hyperLinkArgs);
            };
        }

        public static event Action<IReadOnlyDictionary<string, string>> HyperLinkClicked;
    }
}
#endif
