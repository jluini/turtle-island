
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
		
		public Sprite onIcon;
		public Sprite offIcon;
		
		Image iconImage;
		
		public override void onStart() {
			iconImage = JuloFind.byName<Image>("Icon", this);
			setValue(value);
		}
		
		public override bool click(MenuSystem menuSystem) {
			setValue(!value);
			return true;
		}
		public void setValue(bool newValue) {
			value = newValue;
			onChange.Invoke(value);
			iconImage.sprite = value ? onIcon : offIcon;
		}
	}
}