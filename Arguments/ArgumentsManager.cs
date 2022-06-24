using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
#endif

namespace Arguments
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class ArgumentsManager
    {
        public const string PREFIX = "--";
        private const string FILE_PATH = "_Debug/Arguments.txt";

        public static IReadOnlyDictionary<string, string> Arguments => _arguments;
        private static Dictionary<string, string> _arguments = new();

#if UNITY_EDITOR
        [MenuItem("Tools/Arguments/Open File"), 
            Shortcut("arguments-open-file", KeyCode.Home, ShortcutModifiers.Alt)]
        private static void OpenFile() => System.Diagnostics.Process.Start(GetFullPath());

        private static string GetFullPath() => Path.Combine(Directory.GetCurrentDirectory(), FILE_PATH);

        static ArgumentsManager() => Init();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            var directoryPath = Path.GetDirectoryName(FILE_PATH);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using var stream = new FileStream(FILE_PATH, FileMode.OpenOrCreate);
            using var reader = new StreamReader(stream);

            var contents = reader.ReadToEnd();
            var args = contents.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            LoadArgs(args);
        }
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => LoadArgs(Environment.GetCommandLineArgs());
#endif

        private static void LoadArgs(params string[] args)
        {
            _arguments.Clear();

            for (int i = 0; i < args.Length; i++)
            {
                if (string.IsNullOrEmpty(args[i]))
                    continue;

                var key = args[i].Trim().ToLower();

                if (!key.StartsWith(PREFIX))
                    continue;

                key = key.Remove(startIndex: 0, PREFIX.Length);

                var builder = new StringBuilder();

                for (int j = i + 1; j < args.Length; j++)
                {
                    if (string.IsNullOrEmpty(args[i]))
                        continue;

                    var value = args[j].Trim();

                    if (value.StartsWith(PREFIX))
                        break;

                    builder.Append(value);
                    builder.Append(' ');
                }

                _arguments[key] = builder.Length == 0 ? string.Empty : builder.ToString();
            }
        }
    } 
}