using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum VarType
{
	Constant, //Acts like a normal variable
	Reference //Holds a SmartReference
}

[System.Serializable]
public abstract class SmartVariableBase { }

[System.Serializable]
public class SmartVariable<T, VARIABLET> : SmartVariableBase where VARIABLET : SmartReference<T>
{
	public VarType type;

	[SerializeField]
	private VARIABLET reference;
	public VARIABLET Reference
	{
		get
		{
			return reference;
		}
		set
		{
			reference = value;
		}
	}
	//only valid when the type is constant
	public T runtimeValue;

	public void AddListener(SmartReference<T>.VariableSetEvent listener)
	{
		switch (type)
		{
			case VarType.Reference:
				Reference.AddListener(listener);
				break;
			default:
				Debug.LogWarning("Trying to listen for unsupported type variable changes");
				break;
		}
	}

	public void RemoveListener(SmartReference<T>.VariableSetEvent listener)
	{
		switch (type)
		{
			case VarType.Reference:
				Reference.RemoveListener(listener);
				break;
			default:
				Debug.LogWarning("SmartVariable: Trying to listen for unsupported type variable changes");
				break;
		}
	}

	public T Value
	{
		get
		{
			switch (type)
			{
				case VarType.Constant:
					return runtimeValue;
				case VarType.Reference:
					if (Reference == null)
					{
						Debug.LogError("SmartVariable: Variable Reference not set");
						return default(T);
					}
					return Reference.Value;
				default:
					Debug.LogError("SmartVariable: INVALID TYPE VARIABLE");
					Debug.Log(type);
					return default(T);
			}
		}
		set
		{
			switch (type)
			{
				case VarType.Constant:
					runtimeValue = value;
					break;
				case VarType.Reference:
					UnityEngine.Assertions.Assert.IsNotNull(Reference, "SmartVariable: No Reference assigned!");
					Reference.Value = value;
					break;
				default:
					Debug.LogError("SmartVariable: INVALID TYPE VARIABLE");
					break;
			}
		}
	}
}