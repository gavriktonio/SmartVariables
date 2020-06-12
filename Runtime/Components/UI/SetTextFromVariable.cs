using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SmartVariables
{
	public class SetTextFromVariable : MonoBehaviour
	{
		private Text text;
		private TMP_Text textTMPro;
		public SmartReferenceBase variable;

		private void OnEnable()
		{
			text = GetComponent<Text>();
			textTMPro = GetComponent<TMP_Text>();

			CheckReferences();
		}

		void Update()
		{
			if (text != null)
			{
				text.text = variable.ValueAsString();
			}
			else if (textTMPro != null)
			{
				textTMPro.text = variable.ValueAsString();
			}
		}

		void CheckReferences()
		{
			if (text == null && textTMPro == null)
			{
				Debug.LogError("A text component or TMProText component needs to be attached to this gameObject!");
			}

			if (variable == null)
			{
				Debug.LogError("A variable is not assigned to set the text from!");
			}
		}
	}
}