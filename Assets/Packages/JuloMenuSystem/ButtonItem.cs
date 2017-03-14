
using UnityEngine;
using UnityEngine.Events;

namespace JuloMenuSystem {
	public class ButtonItem : Item {
		public UnityEvent onClick;
		
		public override bool click(MenuSystem menuSystem) {
			onClick.Invoke();
			return true;
		}
	}
}