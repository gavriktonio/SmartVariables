using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class SmartLocalCollection : MonoBehaviour
{
    private int copyInstanceID;

    public List<SmartReferenceBase> variables = new List<SmartReferenceBase>();

    public void AddToCollection(SmartReferenceBase variable)
    {
        string oldPath = AssetDatabase.GetAssetPath(variable);
        variables.Add(variable);
        AssetDatabase.RemoveObjectFromAsset(variable);
        if (AssetDatabase.LoadAllAssetsAtPath(oldPath).Length == 0)
        {
            AssetDatabase.DeleteAsset(oldPath);
        }
        AssetDatabase.SaveAssets();
    }

    private void Awake()
    {
        if (copyInstanceID != GetInstanceID())
        {
            DeepCopy();
        }
    }

    private void OnValidate()
    {
        if (copyInstanceID != GetInstanceID())
        {
            DeepCopy();
        }
    }

    private void DeepCopy()
    {
        List<Component> components = new List<Component>();

        components.AddRange(GetComponentsInChildren<Component>());
        components.RemoveAll(x =>
        x.GetType() != null &&
        (x.GetType() == typeof(Transform) ||
         x.GetType() == typeof(DecoupleReferences))
        );

        for (int i = 0; i < variables.Count; i++)
        {
            variables[i] = DeepCopy(variables[i], components);
        }
        copyInstanceID = GetInstanceID();
    }

    private SmartReferenceBase DeepCopy(SmartReferenceBase variable, List<Component> components)
    {
        SmartReferenceBase copy = Instantiate(variable);
        copy.name = variable.name;
        copy.SetRuntimeValueFromObject(variable.GetValueAsObject());

        foreach (Component component in components)
        {
            System.Type type = component.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                //In case of a smart reference - replace the reference inside
                if (field.FieldType.IsSubclassOf(typeof(SmartVariableBase)))
                {
                    object smartVariable = field.GetValue(component);
                    System.Type typeTest = smartVariable.GetType();
                    PropertyInfo referenceFieldfInSmartVariable = typeTest.GetProperty("Reference");
#pragma warning disable CS0252 // Possible unintended reference comparison; It is completely intended
                    if (referenceFieldfInSmartVariable.GetValue(smartVariable, null) == variable)
                    {
                        referenceFieldfInSmartVariable.SetValue(smartVariable, copy, null);
                    }
                }
                else if (field.GetValue(component) == variable)
#pragma warning restore CS0252
                {
                    field.SetValue(component, copy);
                }

            }
        }
        return copy;
    }
}
