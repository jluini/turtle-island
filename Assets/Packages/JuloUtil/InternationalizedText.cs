
using System;

using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

namespace JuloUtil {
	[ExecuteInEditMode]
	public class InternationalizedText : MonoBehaviour {
		public string englishText;
		public string spanishText;
		
		public Text textComponent;
		
		void OnEnable() {
			textComponent = GetComponent<Text>();
		}
		
		public void setLanguage(Language language) {
			if(textComponent == null) {
				Debug.Log("Fail in " + transform.parent.name);
				return;
			}
			textComponent.text = getText(language);
			EditorUtility.SetDirty(textComponent);
		}
		
		string getText(Language language) {
			if(language == Language.English) {
				return englishText;
			} else if(language == Language.Spanish) {
				return spanishText;
			} else {
				throw new ApplicationException("Unknown language");
			}
		}
	}
	
}