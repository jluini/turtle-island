
using UnityEngine;
using UnityEngine.Events;

namespace JuloMenuSystem {
	public class OtherMenuItem : Item {
		public Menu otherMenu;
		
		public override bool click(MenuSystem menuSystem) {
			menuSystem.openMenu(otherMenu);
			return true;
		}
	}
}