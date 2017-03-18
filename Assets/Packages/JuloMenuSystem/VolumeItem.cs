
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using JuloUtil;
using JuloAudio;

namespace JuloMenuSystem {
	public class VolumeItem : Item {
		public bool value = true;
		public float volume = 1f;
		
		public SoundSource soundSource;
		
		public Sprite onIcon;
		public Sprite offIcon;
		
		Image iconImage;
		Slider slider;
		
		public override void onStart() {
			iconImage = JuloFind.byName<Image>("Icon", this);
			slider = JuloFind.byName<Slider>("Slider", this);
			_setValue(value);
			_setVolume(volume);
		}
		
		public override bool click(MenuSystem menuSystem) {
			if(soundSource.isOn != value) {
				Debug.LogWarning("Is not equal");
			}
			
			setValue(!soundSource.isOn);
			
			if(value && volume <= 0f) {
				_setVolume(0.7f);
				slider.value = volume;
			}
			
			return true;
		}
		
		public override bool move(MenuSystem menuSystem, bool direction) {
			if(direction && volume < 1f) {
				slider.value = Mathf.Min(1f, slider.value + 0.2f);
				return true;
			} else if(!direction && volume > 0f) {
				slider.value = Mathf.Max(0, slider.value - 0.2f);
				return true;
			} else {
				return false;
			}
			
			
		}
		
		public void onChangeVolume(float newVolume) {
			setVolume(newVolume);
		}
		
		public void setValue(bool newValue) {
			if(newValue != value) {
				_setValue(newValue);
			}
		}
		
		public void setVolume(float newVolume) {
			_setVolume(newVolume);
			if(volume <= 0.001f && value) {
				volume = 0f;
				setValue(false);
			} else if(volume > 0.001f && !value) {
				setValue(true);
			}
		}
		void _setValue(bool newValue) {
			value = newValue;
			soundSource.isOn = value;
			iconImage.sprite = value ? onIcon : offIcon;
		}
		void _setVolume(float newVolume) {
			volume = newVolume;
			soundSource.setVolume(volume);
		}
	}
}