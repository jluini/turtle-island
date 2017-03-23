
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace JuloUtil {
	public enum Language { English, Spanish }
	
	public class Internationalization {
		
		public static Language currentLanguage = Language.English;
		
		public static void setLanguage(Language language) {
			// Debug.Log("Setting language to " + language);
			
			currentLanguage = language;
			updateTexts();
		}
		static void updateTexts() {
			List<InternationalizedText> texts = JuloFind.allWithComponent<InternationalizedText>();
			foreach(InternationalizedText text in texts) {
				text.setLanguage(currentLanguage);
				//EditorUtility.SetDirty(text);
			}
		}
	}
}