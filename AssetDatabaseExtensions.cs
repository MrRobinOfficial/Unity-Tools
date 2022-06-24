#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Editor
{
    public static class AssetDatabaseExtensions
    {
        public static string GetAssetPath(string filter)
        {
            var guids = AssetDatabase.FindAssets(filter);

            if (guids == null || guids.Length == 0)
                return string.Empty;

            return AssetDatabase.GUIDToAssetPath(guids[0]);
        }

        public static string GetAssetPath(this Object behaviour) =>
            GetAssetPath(filter: $"t:Script {behaviour.GetType().Name}");

        public static string GetDirectoryPath(this Object behaviour) => string.IsNullOrEmpty(behaviour.GetAssetPath()) ? string.Empty : Path.GetDirectoryName(behaviour.GetAssetPath());

        public static bool TryGetAssetPath(this Object behaviour, out string path)
        {
            path = GetAssetPath(filter: $"t:Script {behaviour.GetType().Name}");
            return !string.IsNullOrEmpty(path);
        }

        public static bool TryGetDirectoryPath(this Object behaviour, out string path)
        {
            path = string.IsNullOrEmpty(behaviour.GetAssetPath()) ? string.Empty : Path.GetDirectoryName(behaviour.GetAssetPath());
            return !string.IsNullOrEmpty(path);
        }
    }
} 
#endif