using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/VariableSavers/SystemIO")]
    public class SmartVariableSaverSystemIO : SmartVariableSaverBase
    {
        public string pathToSave;

        private Dictionary<int, object> cachedLoadedVariables = new Dictionary<int, object>();

        private bool loaded = false;

        public override object GetSavedVariableValue(int variableId)
        {
            if (!loaded)
            {
                LoadVariables();
            }

            if (cachedLoadedVariables.ContainsKey(variableId))
            {
                return cachedLoadedVariables[variableId];
            }

            return null;
        }

        public override void SaveQueuedVariables()
        {
            if (!loaded)
            {
                LoadVariables();
            }

            //saved queued variables into loaded cache
            foreach (SmartReferenceBase var in queuedVariablesToSave)
            {
                cachedLoadedVariables[var.GetInstanceID()] = var.GetValueAsObject();
            }

            queuedVariablesToSave.Clear();

            string pathAndName = Path.Combine(Application.persistentDataPath, pathToSave);
            string fullPath = Path.GetDirectoryName(pathAndName);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            FileStream file;
            if (File.Exists(pathAndName))
            {
                File.Delete(pathAndName);
            }

            file = File.Create(pathAndName);

            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(file, cachedLoadedVariables.Count);

            foreach (KeyValuePair<int, object> IdVarPair in cachedLoadedVariables)
            {
                bf.Serialize(file, IdVarPair.Key);
                bf.Serialize(file, IdVarPair.Value);
            }

            file.Close();
        }

        public override void LoadVariables()
        {
            string pathAndName = Path.Combine(Application.persistentDataPath, pathToSave);

            FileStream file;
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists(pathAndName))
            {
                file = File.OpenRead(pathAndName);
            }
            else
            {
                Debug.LogWarning("Saved variables file not found, creating new file: " + pathAndName);
                string fullPath = Path.GetDirectoryName(pathAndName);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
                file = File.Create(pathAndName);
                bf.Serialize(file, 0);
                file.Close();
                return;
            }


            int variablesCount = (int)bf.Deserialize(file);

            for (int i = 0; i < variablesCount; i++)
            {
                int deserializedId;
                object deserializedObject;
                try
                {
                    deserializedId = (int)bf.Deserialize(file);
                    deserializedObject = bf.Deserialize(file);
                }
                catch (System.SystemException e)
                {
                    Debug.LogWarning("Error reading from" + pathAndName + ". Message = " + e.Message);
                    break;
                }

                cachedLoadedVariables.Add(deserializedId, deserializedObject);
            }

            file.Close();
            loaded = true;
        }
    }
}