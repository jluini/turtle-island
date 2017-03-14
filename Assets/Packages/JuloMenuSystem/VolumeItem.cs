
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using JuloUtil;
using JuloAudio;

namespace JuloMenuSystem {
	public class VolumeItem : Item {
		public SoundSource soundSource;
		
		Image iconImage;
		
		public override void onStart() {
			iconImage = JuloFind.byName<Image>("Icon", this);
		}
		
		public override bool click(MenuSystem menuSystem) {
			bool value = !soundSource.isOn;
			soundSource.isOn = value;
			iconImage.sprite = menuSystem.getSprite(value);
			
			return true;
		}
	}
}