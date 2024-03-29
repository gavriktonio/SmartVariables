﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SmartVariables
{
    [System.Serializable]
    public enum VarType
    {
        Constant, //Acts like a normal variable
        Reference //Holds a SmartReference
    }

    [System.Serializable]
    public abstract class SmartVariableBase
    {
    }


    [System.Serializable]
    public class SmartVariable<T, VARIABLET> : SmartVariableBase where VARIABLET : SmartReference<T>
    {
        [FormerlySerializedAs("type")] public VarType Type;

        [SerializeField] private VARIABLET reference;

        public VARIABLET Reference
        {
            get { return reference; }
            set { reference = value; }
        }

        private Logger _logger = null;
        protected Logger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new Logger() { Prefix = "[SmartVariable]: " };

                if (Reference.OverrideGlobalLogLevel)
                    _logger.LogLevel = Reference.LogLevel;

                return _logger;
            }
        }

        //only valid when the Type is constant
        public T runtimeValue;

        public void AddListener(SmartReference<T>.VariableSetEvent listener)
        {
            switch (Type)
            {
                case VarType.Reference:
                    Reference.AddListener(listener);
                    break;
                default:
                    Logger.LogWarning("Trying to listen for unsupported Type variable changes");
                    break;
            }
        }

        public void RemoveListener(SmartReference<T>.VariableSetEvent listener)
        {
            switch (Type)
            {
                case VarType.Reference:
                    Reference.RemoveListener(listener);
                    break;
                default:
                    Logger.LogWarning("Trying to listen for unsupported Type variable changes");
                    break;
            }
        }

        public T Value
        {
            get
            {
                switch (Type)
                {
                    case VarType.Constant:
                        return runtimeValue;
                    case VarType.Reference:
                        if (Reference == null)
                        {
                            Logger.LogError("Variable Reference not set.");
                            return default(T);
                        }

                        return Reference.Value;
                    default:
                        Logger.LogError("INVALID TYPE VARIABLE! Type is: {0}", Type);
                        return default(T);
                }
            }
            set
            {
                switch (Type)
                {
                    case VarType.Constant:
                        runtimeValue = value;
                        break;
                    case VarType.Reference:
                        UnityEngine.Assertions.Assert.IsNotNull(Reference, "[SmartVariable]: No Reference assigned!");
                        Reference.Value = value;
                        break;
                    default:
                        Logger.LogError("INVALID TYPE VARIABLE!");
                        break;
                }
            }
        }

        public static implicit operator T(SmartVariable<T, VARIABLET> var) => var.Value;

        //public static T operator *(T var, SmartVariable<T, VARIABLET> smartVar)
        //{
        //	dynamic x = var, y = smartVar.Value;
        //	return x * y;
        //}
        //
        //public static T operator +(T var, SmartVariable<T, VARIABLET> smartVar)
        //{
        //	dynamic x = var, y = smartVar.Value;
        //	return x + y;
        //}
    }
}