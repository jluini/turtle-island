
using System;

using UnityEngine;

namespace JuloUtil {
	
	public class Utils {
		public static bool pointIsWithinRect(Vector2 point, RectTransform rect) {
			float left = rect.position.x + rect.rect.xMin;
			float rite = rect.position.x + rect.rect.xMax;
			float bott = rect.position.y + rect.rect.yMin;
			float topp = rect.position.y + rect.rect.yMax;
			
			bool xIn = point.x >= left && point.x <= rite;
			bool yIn = point.y >= bott && point.y <= topp;
			
			bool pointIn = xIn&&yIn;
			
			return pointIn;
		}
	}
	
	public class JuloBehaviour : MonoBehaviour {
		public bool isActive  () {
			try {
				if(!gameObject)
					Debug.Log("JuloBehaviour strange error 1");
				return gameObject && gameObject.activeSelf;
			} catch(Exception) {
				Debug.Log("JuloBehaviour strange error 2 (" + this + ")");
				return false;
			}
		}
		public void activate  () { gameObject.SetActive(true);   }
		public void deactivate() { gameObject.SetActive(false);  }
	}
	
	
	public interface Behav {
		void start(InputManager inputManager);
		void update();
	}
	public interface Animatable {
		void trigger(string newState);
	}
}
