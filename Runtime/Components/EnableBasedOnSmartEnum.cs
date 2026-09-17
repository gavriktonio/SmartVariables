using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace SmartVariables
{
    [AddComponentMenu("Smart Variables/Enable Based on Smart Enum")]
    public sealed class EnableBasedOnSmartEnum : MonoBehaviour
    {
        [Serializable]
        public sealed class EnumOption
        {
            // Store the underlying value rather than its ordinal or display name.
            public string enumType;
            public string enumValue;
            public GameObject[] gameObjects = Array.Empty<GameObject>();
        }

        public SmartReferenceBase enumReference;
        [HideInInspector] public List<EnumOption> options = new List<EnumOption>();

        private SmartReferenceBase subscribedReference;
        private string subscribedEnumType;
        private IDisposable subscription;

        // Constructed once when binding. All change and unsubscribe calls are typed.
        [Preserve]
        private sealed class EnumSubscription<T> : IDisposable where T : struct, Enum
        {
            private readonly SmartReference<T> reference;
            private readonly Action onChanged;
            private readonly SmartReference<T>.VariableSetEvent listener;

            [Preserve]
            public EnumSubscription(SmartReferenceBase reference, Action onChanged)
            {
                this.reference = (SmartReference<T>)reference;
                this.onChanged = onChanged;
                listener = OnValueChanged;
                this.reference.AddListener(listener);
            }

            private void OnValueChanged(T oldValue, T newValue) => onChanged();

            public void Dispose()
            {
                if (reference != null)
                    reference.RemoveListener(listener);
            }
        }

        /// <summary>Find the enum argument of an existing SmartReference&lt;T&gt; subclass.</summary>
        public static Type GetEnumType(SmartReferenceBase reference)
        {
            if (reference == null)
                return null;

            for (Type type = reference.GetType(); type != null; type = type.BaseType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(SmartReference<>))
                {
                    Type valueType = type.GetGenericArguments()[0];
                    return valueType.IsEnum ? valueType : null;
                }
            }
            return null;
        }
        private readonly HashSet<GameObject> managedObjects = new HashSet<GameObject>();
        private readonly HashSet<GameObject> enabledObjects = new HashSet<GameObject>();
        private bool applying;
        private bool applyPending;

        private void OnEnable()
        {
            Rebind();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        /// <summary>Rebind after changing enumReference at runtime, then apply its current value.</summary>
        public void Rebind()
        {
            Unsubscribe();
            if (!isActiveAndEnabled || enumReference == null)
                return;

            Type enumType = GetEnumType(enumReference);
            if (enumType == null)
            {
                Debug.LogError("Enable Based on Smart Enum requires an enum Smart Reference.", this);
                return;
            }

            Type subscriptionType = typeof(EnumSubscription<>).MakeGenericType(enumType);
            subscription = (IDisposable)Activator.CreateInstance(subscriptionType, enumReference, (Action)ApplyCurrentValue);
            subscribedReference = enumReference;
            subscribedEnumType = enumType.FullName;
            ApplyCurrentValue();
        }

        private void Unsubscribe()
        {
            subscription?.Dispose();
            subscription = null;
            subscribedReference = null;
            subscribedEnumType = null;
        }

        /// <summary>Apply the current value after changing GameObject assignments at runtime.</summary>
        public void ApplyCurrentValue()
        {
            if (!isActiveAndEnabled || subscribedReference == null)
                return;

            // A target's activation callback can change a variable and request another apply.
            if (applying)
            {
                applyPending = true;
                return;
            }

            applying = true;
            try
            {
                do
                {
                    applyPending = false;
                    ApplyValue();
                }
                while (applyPending && isActiveAndEnabled && subscribedReference != null);
            }
            finally
            {
                applying = false;
            }
        }

        private void ApplyValue()
        {
            string enumType = subscribedEnumType;
            string enumValue = ((Enum)subscribedReference.GetValueAsObject()).ToString("D");
            managedObjects.Clear();
            enabledObjects.Clear();

            if (options != null)
            {
                foreach (EnumOption option in options)
                {
                    if (option == null || option.enumType != enumType || option.gameObjects == null)
                        continue;

                    foreach (GameObject target in option.gameObjects)
                    {
                        if (target == null)
                            continue;
                        managedObjects.Add(target);
                        if (option.enumValue == enumValue)
                            enabledObjects.Add(target);
                    }
                }
            }

            // Evaluate membership across all lists before applying, so list order cannot
            // disable an object shared with the current option.
            foreach (GameObject target in managedObjects)
            {
                if (target != null)
                {
                    bool active = enabledObjects.Contains(target);
                    if (target.activeSelf != active)
                        target.SetActive(active);
                }
            }
        }
    }
}
