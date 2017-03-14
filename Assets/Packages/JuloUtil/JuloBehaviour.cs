
using System;

using UnityEngine;

namespace JuloUtil {
	
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
