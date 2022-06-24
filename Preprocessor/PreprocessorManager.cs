#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Preprocessor
{
    [InitializeOnLoad]
    public static class PreprocessorManager
    {
        private const string FILE_PATH = "_Debug/Preprocessor.txt";

        static PreprocessorManager() => Init();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            var directoryPath = Path.GetDirectoryName(FILE_PATH);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using var stream = new FileStream(FILE_PATH, FileMode.OpenOrCreate);
            using var reader = new StreamReader(stream);

            var contents = reader.ReadToEnd();
            var processors = contents.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (processors == null || processors.Length == 0)
                return;

            var target = EditorUserBuildSettings.activeBuildTarget;
            var group = BuildPipeline.GetBuildTargetGroup(target);

            AddDefines(group, processors);
        }

        [MenuItem("Tools/Preprocessor/Open File"),
            Shortcut("preprocessor-open-file", KeyCode.End, ShortcutModifiers.Alt)]
        private static void OpenFile() => System.Diagnostics.Process.Start(GetFullPath());

        private static string GetFullPath() => Path.Combine(Directory.GetCurrentDirectory(), FILE_PATH);

        private static void AddDefine(string defineSymbol, BuildTargetGroup group = BuildTargetGroup.Standalone)
        {
            var groupDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var defines = groupDefine.Split(';');

            for (int i = 0; i < defines.Length; i++)
            {
                if (string.Compare(defines[i], defineSymbol) == 0)
                    return;
            }

            groupDefine += string.Format(",{0}", defineSymbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, groupDefine);
        }

        private static void AddDefines(BuildTargetGroup group = BuildTargetGroup.Standalone, 
            params string[] defineSymbols)
        {
            var groupDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            var defines = groupDefine.Split(';');

            for (int i = 0; i < defineSymbols.Length; i++)
            {
                for (int j = 0; j < defines.Length; j++)
                {
                    if (string.Compare(defines[j], defineSymbols[i]) == 0)
                        return;
                }

                groupDefine += string.Format(",{0}", defineSymbols[i]);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, groupDefine);
        }

        private static string[] GetDefines(BuildTargetGroup group = BuildTargetGroup.Standalone) => PlayerSettings
                .GetScriptingDefineSymbolsForGroup(group)
                .Split(';');
    }
} 
#endif