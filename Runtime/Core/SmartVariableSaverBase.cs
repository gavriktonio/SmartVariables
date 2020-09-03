using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SmartVariables
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Variables/VariableSaver")]
    public abstract class SmartVariableSaverBase : ScriptableObject
    {
        protected HashSet<SmartReferenceBase> queuedVariablesToSave = new HashSet<SmartReferenceBase>();

        public abstract object GetSavedVariableValue(int variableId);

        public virtual void AddVariableToSaveQueue(SmartReferenceBase variable)
        {
            queuedVariablesToSave.Add(variable);
        }
    }
}