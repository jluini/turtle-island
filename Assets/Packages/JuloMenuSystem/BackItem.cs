
using UnityEngine;
using UnityEngine.Events;

namespace JuloMenuSystem {
	public class BackItem : Item {
		public UnityEvent onClick;
		
		public override bool click(MenuSystem menuSystem) {
			menuSystem.goBack();
			return false;
		}
	}
}