using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace SmartVariables
{
//For all the references, that are in the array, creates a copy, and substitutes all
//same references in components of this gameObject and its children to the copy
//
//Needs to run before other components, that use these references. Make sure to set
//it to go early in the script execution order
	[System.Obsolete("DecoupleReferences is obsolete. Use local collection instead")]
	public class DecoupleReferences : MonoBehaviour
	{
		public SmartReferenceBase[] toDecouple;

		private void Awake()
		{
			List<Component> components = new List<Component>();

			components.AddRange(GetComponentsInChildren<Component>());
			components.RemoveAll(x =>
				x.GetType() == typeof(Transform) ||
				x.GetType() == typeof(DecoupleReferences)
			);

			foreach (SmartReferenceBase referenceToDecouple in toDecouple)
			{
				Decouple(referenceToDecouple, components);
			}
		}

		private void Decouple(SmartReferenceBase toDecouple, List<Component> components)
		{
			SmartReferenceBase copied = Instantiate(toDecouple);

			foreach (Component component in components)
			{
				System.Type type = component.GetType();
				FieldInfo[] fields =
					type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

				foreach (FieldInfo field in fields)
				{
					//In case of a smart reference - replace the reference inside
					if (field.FieldType.IsSubclassOf(typeof(SmartVariableBase)))
					{
						object smartVariable = field.GetValue(component);
						System.Type typeTest = smartVariable.GetType();
						PropertyInfo referenceFieldfInSmartVariable = typeTest.GetProperty("Reference");
#pragma warning disable CS0252 // Possible unintended reference comparison; It is completely intended
						if (referenceFieldfInSmartVariable.GetValue(smartVariable, null) == toDecouple)
						{
							referenceFieldfInSmartVariable.SetValue(smartVariable, copied, null);
						}
					}
					else if (field.GetValue(component) == toDecouple)
#pragma warning restore CS0252
					{
						field.SetValue(component, copied);
					}

				}
			}
		}
	}
}