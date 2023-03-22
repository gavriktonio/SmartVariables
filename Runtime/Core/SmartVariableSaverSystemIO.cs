using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Serialization;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/VariableSavers/SystemIO")]
    public class SmartVariableSaverSystemIO : SmartVariableSaverBase
    {
        public SmartString[] PathToSave;

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

            string pathToSave = "";
            foreach (SmartString pathPart in PathToSave)
            {
                pathToSave = Path.Combine(pathToSave, pathPart.Value);
            }
            
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
            string pathToSave = "";
            foreach (SmartString pathPart in PathToSave)
            {
                pathToSave = Path.Combine(pathToSave, pathPart.Value);
            }
            
            string pathAndName = Path.Combine(Application.persistentDataPath, pathToSave);

            FileStream file;
            BinaryFormatter bf = new BinaryFormatter();

            if (File.Exists(pathAndName))
            {
                file = File.OpenRead(pathAndName);
            }
            else
            {
                SmartLogger.LogWarning("Saved variables file not found, creating new file: {0}", pathAndName);
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
                    SmartLogger.LogWarning("Error reading from '{0}'. Message = {1}", pathAndName, e.Message);
                    break;
                }

                cachedLoadedVariables.Add(deserializedId, deserializedObject);
            }

            file.Close();
            loaded = true;
        }
    }
}