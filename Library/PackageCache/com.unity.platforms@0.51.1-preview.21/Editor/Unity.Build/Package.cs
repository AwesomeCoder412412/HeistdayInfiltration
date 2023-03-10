using System.IO;
using UnityEditor;

namespace Unity.Build
{
    internal class Package
    {
        public const string PackageName = "com.unity.platforms";
        public const string PackagePath = "Packages/" + PackageName;
        public const string EditorDefaultResourcesPath = PackagePath + "/Editor Default Resources";

        public static T LoadResource<T>(string path, bool logError) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
                return null;

            return typeof(UnityEngine.Texture).IsAssignableFrom(typeof(T)) ?
                LoadTexture<T>(path, logError) :
                Load<T>(path, logError);
        }

        static T LoadTexture<T>(string path, bool logError) where T : UnityEngine.Object
        {
            path = path.ToForwardSlash();
            var directory = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var fileExtension = Path.GetExtension(path);
            var prefix = EditorGUIUtility.isProSkin ? "d_" : string.Empty;
            var suffix = EditorGUIUtility.pixelsPerPoint > 1.0 ? "@2x" : string.Empty;
            return Load<T>(Path.Combine(directory, prefix + fileName + suffix + fileExtension).ToForwardSlash(), logError);
        }

        static T Load<T>(string path, bool logError) where T : UnityEngine.Object
        {
            var pathInPackage = Path.Combine(EditorDefaultResourcesPath, path).ToForwardSlash();
            var resource = EditorGUIUtility.Load(pathInPackage);
            if (resource == null || !resource)
                resource = EditorGUIUtility.Load(path);

            if ((resource == null || !resource) && logError)
                UnityEngine.Debug.LogError($"Missing resource at {path.ToHyperLink()}.");

            return resource as T;
        }
    }
}
