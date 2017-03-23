
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using JuloUtil;

public class EditorMagics {
	[MenuItem("Magics/Language/English")]
	private static void setLanguageToEnglish() {
		setLang(Language.English);
	}
	
	[MenuItem("Magics/Language/Spanish")]
	private static void setLanguageToSpanish() {
		setLang(Language.Spanish);
	}
	
	static void setLang(Language lang) {
		Internationalization.setLanguage(lang);
	}
}

#endif
