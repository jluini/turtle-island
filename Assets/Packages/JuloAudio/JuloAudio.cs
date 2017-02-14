
using UnityEngine;

namespace JuloAudio {
	
	public abstract class SoundSource : MonoBehaviour {
		public abstract void init();
		public abstract void play();
		public abstract void pause();
		public abstract void stop();
		public abstract void next();
		public abstract void setVolume(float volume);
		public abstract void playClip(AudioClip clip, float volume = 1f);
		
		public abstract bool isPlaying();
	}
	
}
