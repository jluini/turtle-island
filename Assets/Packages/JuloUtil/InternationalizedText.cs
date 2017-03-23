
using System;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
			string t = getText(language);
			t = t.Replace("\\n", "\n");
			textComponent.text = t;
			#if UNITY_EDITOR
			EditorUtility.SetDirty(textComponent);
			#endif
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