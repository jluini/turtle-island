
using UnityEngine;

namespace JuloAudio {
	
	public class SoundsPlayerBehaviour : SoundSource {
		private AudioSource _audioSource;
		private AudioSource audioSource {
			get {
				if(_audioSource == null)
					_audioSource = GetComponent<AudioSource>();
				return _audioSource;
			}
		}
		
		public override void playClip(AudioClip clip, float volume = 1f) {
			audioSource.PlayOneShot(clip, volume);
		}
		
		public override void setVolume(float volume) { audioSource.volume = volume; }
		
		public override void init() { }
		public override void play() { }
		public override void pause() { }
		public override void stop() { }
		public override void next() { }
		public override bool isPlaying() {
			return audioSource.isPlaying;
		}
	}
}
