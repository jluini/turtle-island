
using System;

using UnityEngine;

using JuloUtil;

namespace JuloAudio {
	
	public class MusicPlayerBehaviour : SoundSource {
		public bool shuffle;
		public int initialIndex = 0;
		// Time to wait between songs.
		public float waitTime = 2.5f;
		
		public AudioClip[] clips;
		private bool[] clipFlags;
		private int numSongs;
		
		private enum State { STOPPED, PLAYING, OVER, PAUSED };
		private State state = State.STOPPED;
		
		private float songOverTimestamp;
		public int lastIndex;
		private int songsPlayed;
		
		public override void onInit() {
			numSongs = clips.Length;
			if(numSongs < 1)
				return;
			//	throw new ApplicationException("There are no clips");
			
			clipFlags = new bool[numSongs];
			
			for(int i = 0; i < numSongs; i++) {
				clipFlags[i] = false;
			}
			
			lastIndex = (initialIndex % numSongs) - 1;
			songsPlayed = 0;
		}
		
		void Update() {
			if(state == State.PLAYING) {
				if(!audioSource.isPlaying) {
					// clip is over
					//Debug.Log("Clip " + lastIndex + " is over.");
					state = State.OVER;
					songOverTimestamp = JuloTime.applicationTime();
				}
			} else if(state == State.OVER) {
				float ellapsed = JuloTime.applicationTimeSince(songOverTimestamp);
				
				if(ellapsed > waitTime) {
					state = State.PLAYING;
					next();
				}
			}
			/*
			if(state == State.PLAYING) {
				if(audioSource.time == 0) {
					float now = JuloTime.now();
					float ellapsed = now - playTimestamp;
					if(ellapsed > waitTime) {
						playTimestamp = JuloTime.now();
						next();
					}
				} else {
					playTimestamp = JuloTime.now();
				}
			}*/
		}
		
		public override bool isPlaying() {
			return state == State.PLAYING || state == State.OVER;
		}
		/*
		public bool isPaused() {
			return state == State.PAUSED;
		}
		*/
		public override void next() {
			if(songsPlayed == numSongs) {
				for(int i = 0; i < numSongs; i++) {
					clipFlags[i] = false;
				}
				if(shuffle && numSongs > 1) {
					songsPlayed = 1;
					clipFlags[lastIndex] = true;
				} else {
					songsPlayed = 0;
					lastIndex = (initialIndex % numSongs) - 1;
				}
			} else if(songsPlayed > numSongs) {
				throw new ApplicationException("Calculo inesperado en la music1");
			}
			
			int index = -1;
			
			if(shuffle) {
				int j = JuloMath.randomInt(0, numSongs - songsPlayed - 1);
				for(int i = 0; i < numSongs; i++) {
					if(clipFlags[i]) {
						j++;
					} else if(i == j) {
						index = i;
						break;
					}
				}
				//if(index == -1)
				//	throw new ApplicationException("Calculo inesperado en la music2");
			} else {
				index = (lastIndex + 1) % numSongs;
			}
			
			if(index < 0 || index >= numSongs || clipFlags[index]) {
				throw new ApplicationException("Calculo inesperado en la music2");
			}
			
			lastIndex = index;
			clipFlags[index] = true;
			songsPlayed++;
			
			audioSource.clip = clips[index];
			//Debug.Log("Playing song " + index);
			audioSource.Play();
		}
		
		public override void play() {
			if(state == State.STOPPED) {
				if(numSongs > 0) {
					state = State.PLAYING;
					next();
				}
			} else if(state == State.PAUSED) {
				audioSource.Play();
				state = State.PLAYING;
			}
		}
		
		public override void pause() {
			if(state == State.PLAYING && audioSource.isPlaying) {
				// pausing while PLAYING
				//Debug.Log("Pausing while PLAY " + lastIndex);
				audioSource.Pause();
				state = State.PAUSED;
			} else if(state == State.PLAYING || state == State.OVER) {
				// pausing while OVER
				//Debug.Log("Pausing while OVER " + lastIndex + (state == State.PLAYING ? " (caso loco)" : ""));
				next();
				audioSource.Pause();
				state = State.PAUSED;
			}
		}
		
		public override void stop() {
			if(state != State.STOPPED) {
				audioSource.Stop();
				init();
			}
		}
		
		public override void onSwitch() {
			if(isOn)
				play();
			else
				pause();
		}
		
		public override void playClip(AudioClip clip, float volume = 1f) { }
		
	}
}