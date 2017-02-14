
using UnityEngine;
using UnityEngine.UI;

namespace JuloUtil {
	public class OcultableDisplay : MonoBehaviour {
		
		public void show() {
			setVisibilityToAll(transform, true);
		}
		
		public void hide() {
			setVisibilityToAll(transform, false);
		}
		
		public void setVisibility(bool visibility) {
			setVisibilityToAll(transform, visibility);
		}
		
		private static void setVisibilityToAll(Transform context, bool visibility) {
			Text text = context.GetComponent<Text>();
			if(text) {
				text.enabled = visibility;
			}
			Image image = context.GetComponent<Image>();
			if(image) {
				image.enabled = visibility;
			}
			foreach(Transform child in context) {
				setVisibilityToAll(child, visibility);
			}
		}
	}
}