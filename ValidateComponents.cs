#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTools.Editor
{
    public static class ValidateComponents
    {
        [MenuItem("Tools/UnityTools/Validate/Find Missing Scripts In Project", priority = 500)]
        private static void FindMissingScriptsInProject()
        {
            var paths = AssetDatabase.GetAllAssetPaths()
                .Where(x => x.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase));

            var isMissing = false;

            foreach (var path in paths)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null)
                    continue;

                foreach (var component in prefab.GetComponentsInChildren<Component>(includeInactive: true))
                {
                    if (component == null)
                    {
                        Debug.LogError($"Prefab found with missing script at '{path}'", prefab);
                        isMissing = true;
                        break;
                    }
                }
            }

            if (!isMissing)
                Debug.Log("Found zero missing scripts in the project.");
        }

        [MenuItem("Tools/UnityTools/Validate/Find Missing Scripts In Scene", priority = 500)]
        private static void FindMissingScriptsInScene()
        {
            var isMissing = false;

            foreach (var obj in GameObject.FindObjectsOfType<GameObject>(includeInactive: true))
            {
                foreach (var component in obj.GetComponentsInChildren<Component>(includeInactive: true))
                {
                    if (component == null)
                    {
                        Debug.LogError($"Prefab found with missing script at '{obj.name}'", obj);
                        isMissing = true;
                        break;
                    }
                }
            }

            if (!isMissing)
                Debug.Log("Found zero missing scripts in the scene.");
        }
    }
} 
#endif