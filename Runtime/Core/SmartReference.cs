using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
//class SmartReferenceResetter
//{
//	//Is here so that enter play mode without domain reloading works
//	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
//	static void OnRuntimeMethodLoad()
//	{
//		string[] smartReferencesGuids = AssetDatabase.FindAssets("t:" + typeof(SmartReferenceBase).Name);
//		foreach (string guid in smartReferencesGuids)
//		{
//			AssetDatabase.LoadAssetAtPath<SmartReferenceBase>(AssetDatabase.GUIDToAssetPath(guid)).RemoveAllListeners();
//		}
//		string[] smartCollectionsGuids = AssetDatabase.FindAssets("t:" + typeof(SmartCollection).Name);
//		foreach (string guid in smartCollectionsGuids)
//		{
//			SmartCollection collection = AssetDatabase.LoadAssetAtPath<SmartCollection>(AssetDatabase.GUIDToAssetPath(guid));
//			foreach (SmartReferenceBase variable in collection.variables)
//			{
//				variable.RemoveAllListeners();
//			}
//		}
//	}
//}
#endif

namespace SmartVariables
{
    public abstract class SmartReferenceBase : ScriptableObject
    {
        [Tooltip("Set to true to ignore the global log level and set your own.")]
        public bool OverrideGlobalLogLevel = false;

        [Tooltip("Set log level for this variable")]
        public LogLevel LogLevel = LogLevel.Off;

        [FormerlySerializedAs("debugLog")]
        [Tooltip("Log all the changes, happening to this variable")]
        [Obsolete]
        public bool DebugLog = false;

        [FormerlySerializedAs("persistent")]
        [Tooltip("If true, the runtime value will persist between play sessions")]
        public bool Persistent = false;

        [FormerlySerializedAs("variableSaver")]
        public SmartVariableSaverBase VariableSaver;

        [FormerlySerializedAs("forceCallbacks")]
        [Tooltip("If true, callbacks will be triggered even if the variable is set to the same value as before")]
        public bool ForceCallbacks = false;

        private Logger _logger = null;
        protected Logger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new Logger() { Prefix = "[SmartReference]: " };

                if (OverrideGlobalLogLevel)
                    _logger.LogLevel = LogLevel;

                return _logger;
            }
        }

        public abstract void PrepareEditorCallbacks();
        public abstract void InvokeEditorCallbacks();

        public abstract string ValueAsString();

        //Mainly to be used to reset persistent variables value to initial state
        public abstract void ResetRuntimeValue();
        public abstract object GetValueAsObject();
        public abstract void SetRuntimeValueFromObject(object obj);
        public abstract void RemoveAllListeners();
    }

    [Serializable]
    public class SmartReference<T> : SmartReferenceBase, ISerializationCallbackReceiver
    {
        [SerializeField] private T initialValue;

        //Needs to be serialized to be accessible from the inspector
        //May be possible to change to not-serialized in final build
        [SerializeField] private T runtimeValue;

        public delegate void VariableSetEvent(T oldValue, T newValue);

        private VariableSetEvent listeners;

        //If a value set is called when a value set is already taking place,
        //we need to finish calling all the listeners before setting the new value.
        //Therefore, we store the new set value in a queue until all the listeners are done.

        //The queue is also being used to store old values when a change is done from unity editor inspector
        private Queue<T> setQueue = null;
        private bool settingInProgress = false;
        private T lastQueued;
        public int ValueChangesInQueue => setQueue == null ? 0 : setQueue.Count;

        public void OnBeforeSerialize()
        {
        }

        void OnEnable()
        {
            Logger.LogDebug("Running OnEnable for '{0}'...", name);

#if !UNITY_EDITOR
            if (Persistent)
            {
                if (VariableSaver != null)
                {
                    object savedValue = VariableSaver.GetSavedVariableValue(GetInstanceID());
                    if (savedValue != null)
                    {
                        SetRuntimeValueFromObject(savedValue);

                        Logger.LogInfo("Value on '{0}' was set to '{1}' from persistent storage.", name, Value);
                        return;
                    }
                }
                //If can't find saved value, reset to initial
                ResetRuntimeValue();

                Logger.LogWarning("Persistent value could not be loaded for '{0}', reset to initial value: '{1}'", name, Value);
            }
#endif
        }

        //Gets called when a game starts, as well as when changing the value from the editor
        public void OnAfterDeserialize()
        {
            //It's not possible to use .name in OnAfterDeserialize
            if (LogLevel == LogLevel.Debug)
            {
                Debug.Log("Running after deserialize...");

                if (setQueue != null && setQueue.Count < 1)
                {
                    Debug.LogWarningFormat(
                        "_________ queue not clear after deserialize. This should happen only when the value gets changed from the editor. Queue count: {0}"
                        , setQueue.Count);
                }
            }

            //The only case where this should be false is when the value gets changed from the editor
            if (setQueue == null || setQueue.Count < 1)
            {
                if (!Persistent)
                {
                    runtimeValue = initialValue;
                }

                listeners = null;
                setQueue = null;
                settingInProgress = false;
            }
        }


        public T Value
        {
            get { return runtimeValue; }
            set
            {
                //Only change the value if
                if (
                    //It's not the same as the current one and no changes are queued
                    (settingInProgress == false || setQueue == null || setQueue.Count == 0) && !EqualityComparer<T>.Default.Equals(runtimeValue, value) ||
                    //More changes are queued up and it's not the same as the last queued
                    setQueue != null && setQueue.Count > 0 && !EqualityComparer<T>.Default.Equals(lastQueued, value) ||
                    //Force callbacks option is on
                    ForceCallbacks)
                {
                    //if a set is called from a callback of a set happening, save the set value in a queue and do it later
                    if (settingInProgress == true)
                    {
                        if (setQueue == null)
                        {
                            setQueue = new Queue<T>();
                        }

                        setQueue.Enqueue(value);
                        lastQueued = value;
                        Logger.LogDebug("{0} queued up a new value to set: '{1}'. Queue length: {2}", name, value.ToString(), setQueue.Count);

                        return;
                    }

                    Logger.Log("{0} is being set from '{1}' to '{2}'", name, runtimeValue, value.ToString());

                    settingInProgress = true;

                    T oldValue = runtimeValue;
                    runtimeValue = value;

                    if (VariableSaver != null)
                    {
                        Logger.LogDebug("{0} with value '{1}' is being queued to save with variable saver '{2}'.", name, value.ToString(), VariableSaver.name);

                        VariableSaver.AddVariableToSaveQueue(this);
                    }

                    if (listeners != null)
                    {
                        listeners.Invoke(oldValue, runtimeValue);
                    }

                    settingInProgress = false;

                    //After setting the variable, check if other values are queued up
                    //If so, call the function recursively to set the queued up value
                    if (setQueue != null && setQueue.Count > 0)
                    {
                        Logger.LogDebug("{0} dequeued a value: {1}", name, setQueue.Peek());

                        T newValue = setQueue.Dequeue();
                        this.Value = (newValue);
                    }
                }
                else
                {
                    Logger.LogWarning("{0} value set to {1} rejected. It's the same as the previous one. If callbacks should still work in this case, turn on the force callbacks option in the variable.",
                              name, value);
                }
            }
        }

        //Gets called from the editor before inspector modifies the values.
        //Because the old values are gone after the change,
        //we save them in the setQueue.
        public override void PrepareEditorCallbacks()
        {
            if (setQueue == null)
            {
                setQueue = new Queue<T>();
            }

            setQueue.Enqueue(initialValue);
            setQueue.Enqueue(runtimeValue);
        }

        //Gets called after the editor inspector modifies the values.
        //We take the values back from the queue, and make sure that
        //The listeners get invoked.
        public override void InvokeEditorCallbacks()
        {
            T newInitialValue = initialValue;
            T newRuntimeValue = runtimeValue;

            initialValue = setQueue.Dequeue();
            runtimeValue = setQueue.Dequeue();

            if (initialValue == null || !initialValue.Equals(newInitialValue))
            {
                initialValue = newInitialValue;
                Value = newInitialValue;
            }
            else
            {
                Value = newRuntimeValue;
            }
        }

        public override string ValueAsString()
        {
            return Value.ToString();
        }

        public override object GetValueAsObject()
        {
            return (object)Value;
        }

        public override void SetRuntimeValueFromObject(object obj)
        {
            T oldValue = runtimeValue;
            runtimeValue = (T)obj;
            if (listeners != null)
            {
                listeners.Invoke(oldValue, runtimeValue);
            }
        }

        public override void ResetRuntimeValue()
        {
            Logger.LogDebug("{0} ResetRuntimeValue", name);

            runtimeValue = initialValue;
        }

        public void AddListener(VariableSetEvent listener)
        {
            Logger.LogDebug("{0} AddListener", name);

            listeners += listener;
        }

        public void RemoveListener(VariableSetEvent listener)
        {
            Logger.LogDebug("{0} RemoveListener", name);

            listeners -= listener;
        }

        public override void RemoveAllListeners()
        {
            Logger.LogDebug("{0} RemoveAllListeners", name);

            if (listeners != null)
            {
                foreach (VariableSetEvent d in listeners.GetInvocationList())
                {
                    listeners -= d;
                }
            }
        }

        public static implicit operator T(SmartReference<T> smartVar)
        {
            if (smartVar == null)
            {
                SmartLogger.LogError("Trying to get a value from a null variable!");
                return default(T);
            }

            return smartVar.Value;
        }
    }
}
