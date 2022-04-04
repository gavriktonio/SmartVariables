using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SmartVariables
{
    public class SmartLoggerSettings : ScriptableObject
    {
        public const string assetPath = "Assets/UserSettings/SmartVariablesSettings.asset";

        public LogLevel GlobalLogLevel = LogLevel.Warning;
        public bool IgnoreOverrides = false;


        private static SmartLoggerSettings _Config;
        public static SmartLoggerSettings Config
        {
            get
            {
                if (_Config == null)
                    _Config = GetOrCreateSettings();
                return _Config;
            }
        }

#if UNITY_EDITOR
        private static SmartLoggerSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<SmartLoggerSettings>(assetPath);
            if (settings == null)
            {
                settings = CreateInstance<SmartLoggerSettings>();

                Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

#else
        private static SmartLoggerSettings GetOrCreateSettings()
        {
            return CreateInstance<SmartLoggerSettings>();
        }
#endif
    }

#if UNITY_EDITOR
    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class SmartLoggerSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateSmartLoggerSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the User Settings window.
            var provider = new SettingsProvider("Project/SmartVariables", SettingsScope.User)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Smart Variables",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = new SerializedObject(SmartLoggerSettings.Config);
                    EditorGUILayout.PropertyField(settings.FindProperty("GlobalLogLevel"), new GUIContent("Global Log Level"));
                    EditorGUILayout.PropertyField(settings.FindProperty("IgnoreOverrides"), new GUIContent("Ignore Overrides"));
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "SmartVariables", "Log" })
            };

            return provider;
        }
    }
#endif
}