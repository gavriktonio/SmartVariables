using System;
using UnityEngine;

namespace SmartVariables
{
    public class SmartLoggerSettings : ScriptableObject
    {
        public LogLevel GlobalLogLevel = LogLevel.Warning;
        public bool IgnoreOverrides = false;


        private static SmartLoggerSettings _Config;
        public static SmartLoggerSettings Config
        {
            get
            {
                if (_Config == null)
                    _Config = GetConfig();
                return _Config;
            }
        }

        private static SmartLoggerSettings GetConfig()
        {
            string path = "SmartVariables/";
            var config = Resources.Load<SmartLoggerSettings>(path);

            if (config == null)
            {
                Debug.LogErrorFormat("Smart Variable Config not found in {0} Resources folder. Will use default.", path);

                config = (SmartLoggerSettings)CreateInstance("SmartLoggerSettings");

#if UNITY_EDITOR
                string pathToFolder = "Assets/SmartVariables/Resources/";
                string filename = "SmartLoggerSettings.asset";

                if (!System.IO.Directory.Exists(Application.dataPath + "/../" + pathToFolder))
                {
                    System.IO.Directory.CreateDirectory(pathToFolder);
                    UnityEditor.AssetDatabase.ImportAsset(pathToFolder);
                }

                if (!System.IO.File.Exists(Application.dataPath + "/../" + pathToFolder + "/" + filename))
                {
                    UnityEditor.AssetDatabase.CreateAsset(config, pathToFolder + "/" + filename);
                }
                UnityEditor.AssetDatabase.SaveAssets();
#endif
            }

            _Config = config;
            return config;
        }
    }
}
