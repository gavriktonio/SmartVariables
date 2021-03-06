﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
		[FormerlySerializedAs("debugLog")]
		[Tooltip("Log all the changes, happening to this variable")]
		public bool DebugLog = false;

		[FormerlySerializedAs("persistent")]
		[Tooltip("If true, the runtime value will persist between play sessions")]
		public bool Persistent = false;

		[FormerlySerializedAs("variableSaver")]
		public SmartVariableSaverBase VariableSaver;

		[FormerlySerializedAs("forceCallbacks")]
		[Tooltip("If true, callbacks will be triggered even if the variable is set to the same value as before")]
		public bool ForceCallbacks = false;

		public abstract void PrepareEditorCallbacks();
		public abstract void InvokeEditorCallbacks();

		public abstract string ValueAsString();

		//Mainly to be used to reset persistent variables value to initial state
		public abstract void ResetRuntimeValue();
		public abstract object GetValueAsObject();
		public abstract void SetRuntimeValueFromObject(object obj);
		public abstract void RemoveAllListeners();
	}

	[System.Serializable]
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

		public void OnBeforeSerialize() { }

		void OnEnable()
		{
			if (DebugLog)
			{
				Debug.Log("SmartReference: " + name + " OnEnable: ");
			}
#if !UNITY_EDITOR
        if (Persistent)
        {
            if (VariableSaver != null)
            {
                object savedValue = VariableSaver.GetSavedVariableValue(GetInstanceID());
                if (savedValue != null)
                {
                    SetRuntimeValueFromObject(savedValue);
                    if (DebugLog)
                    {
                        Debug.Log("SmartReference: " + name + " persistent value was loaded OnEnable: " + Value);
                    }
                    return;
                }
            }
            //If can't find saved value, reset to initial 
            ResetRuntimeValue();
            if (DebugLog)
            {
                Debug.Log("SmartReference: " + name + " persistent value could not be loaded, reset to initial: " + Value);
            }
        }
#endif
		}

		//Gets called when a game starts, as well as when changing the value from the editor
		public void OnAfterDeserialize()
		{
			if (DebugLog)
			{
				//It's not possible to use .name in OnAfterDeserialize
				Debug.Log("SmartReference: _________ after deserialize");
				if (setQueue != null && setQueue.Count < 1)
				{
					Debug.Log(
						"SmartReference: _________ queue not clear after deserialize. This should happen only when the value gets changed from the editor. Queue count: " +
						setQueue.Count);
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
					((settingInProgress == false || (setQueue == null || setQueue.Count == 0)) &&
					 !value.Equals(runtimeValue)) ||
					//More changes are queued up and it's not the same as the last queued
					(setQueue != null && setQueue.Count > 0 && !value.Equals(lastQueued)) ||
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
						if (DebugLog)
						{
							Debug.Log("SmartReference: " + name + " queued up a new value to set: " + value.ToString() +
							          ". queue length: " + setQueue.Count);
						}

						return;
					}

					if (DebugLog)
					{
						Debug.Log("SmartReference: " + name + " is being set from " + runtimeValue + " to " +
						          value.ToString());
					}

					settingInProgress = true;

					T oldValue = runtimeValue;
					runtimeValue = value;

					if (VariableSaver != null)
					{
						if (DebugLog)
						{
							Debug.Log("SmartReference: " + name + " with value " + value.ToString() +
							          " is being queued to save with variable saver " + VariableSaver.name);
						}

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
						if (DebugLog)
						{
							Debug.Log("SmartReference: " + name + " dequeued a value: " + setQueue.Peek());
						}

						T newValue = setQueue.Dequeue();
						this.Value = (newValue);
					}
				}
				else
				{
					if (DebugLog)
					{
						Debug.Log("SmartReference: " + name +
						          " value set to " + value +
						          " regected. It's the same as the previous one. If callbacks should still work in this case, turn on the force callbacks option in the variable ");
					}
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
			return (object) Value;
		}

		public override void SetRuntimeValueFromObject(object obj)
		{
			T oldValue = runtimeValue;
			runtimeValue = (T) obj;
			if (listeners != null)
			{
				listeners.Invoke(oldValue, runtimeValue);
			}
		}

		public override void ResetRuntimeValue()
		{
			if (DebugLog)
			{
				Debug.Log("SmartReference: " + name + " ResetRuntimeValue");
			}

			runtimeValue = initialValue;
		}

		public void AddListener(VariableSetEvent listener)
		{
			if (DebugLog)
			{
				Debug.Log("SmartReference: " + name + " AddListener");
			}

			listeners += listener;
		}

		public void RemoveListener(VariableSetEvent listener)
		{
			if (DebugLog)
			{
				Debug.Log("SmartReference: " + name + " RemoveListener");
			}

			listeners -= listener;
		}

		public override void RemoveAllListeners()
		{
			if (DebugLog)
			{
				Debug.Log("SmartReference: " + name + " RemoveAllListeners");
			}

			if (listeners != null)
			{
				foreach (VariableSetEvent d in listeners.GetInvocationList())
				{
					listeners -= d;
				}
			}
		}

		public static implicit operator T(SmartReference<T> smartVar) => smartVar.Value;

		//public static T operator *(T var, SmartReference<T> smartVar)
		//{
		//	dynamic x = var, y = smartVar.Value;
		//	return x * y;
		//}
//
		//public static T operator +(T var, SmartReference<T> smartVar)
		//{
		//	dynamic x = var, y = smartVar.Value;
		//	return x + y;
		//}
	}
}

