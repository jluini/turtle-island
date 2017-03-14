
using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using JuloUtil;

namespace JuloMenuSystem {
	public class SwitchItem : Item {
		
		[Serializable]
		public class SwitchEvent : UnityEvent<bool> { }
		
		public SwitchEvent onChange;
		public bool value = true;
		
		Image iconImage;
		
		public override void onStart() {
			iconImage = JuloFind.byName<Image>("Icon", this);
		}
		
		public override bool click(MenuSystem menuSystem) {
			value = !value;
			iconImage.sprite = menuSystem.getSprite(value);
			onChange.Invoke(value);
			
			return true;
		}
	}
}