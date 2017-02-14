
using System;

using UnityEngine;
using UnityEngine.UI;

using JuloUtil;

namespace JuloAudio{
	
	public class VolumeControl : MonoBehaviour {
		public SoundSource soundSource;
		
		private bool on;
		private float volume;
		
		private Toggle _toggle;
		private Toggle toggle {
			get {
				if(_toggle == null)
					_toggle = JuloFind.byName<Toggle>("Toggle", this);
				return _toggle;
			}
		}
		private Slider _slider;
		private Slider slider {
			get {
				if(_slider == null)
					_slider = JuloFind.byName<Slider>("Slider", this);
				return _slider;
			}
		}
		
		void Awake() {
			toggle.onValueChanged.AddListener(this.onToggle);
			slider.onValueChanged.AddListener(this.onSlide);
		}
		void Start() {
			on = toggle.isOn;
			volume = slider.value;
		}
		
		public void onToggle(bool newValue) {
			on = newValue;
			
			if(soundSource != null) {
				if(on) {
					soundSource.play();
				} else {
					soundSource.pause();
				}
			}
		}
		
		public void onSlide(float newValue) {
			this.volume = newValue;
			
			if(soundSource != null)
				soundSource.setVolume(this.volume);
		}
		
		public bool isOn() {
			return on;
		}
		
		public void setOn(bool on) {
			toggle.isOn = on;
		}
	}
}
