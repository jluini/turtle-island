using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace JuloUtil {
	
	class ReplayFrame {
		public float timestamp;
		public Texture2D texture;
		
		public ReplayFrame(Texture2D texture, float timestamp) {
			this.texture = texture;
			this.timestamp = timestamp;
		}
	}
	public class ReplayManager : MonoBehaviour {
		public enum ReplayManagerState { NONE, RECORDING, REPLAYING }
		public ReplayManagerState state = ReplayManagerState.NONE;
		
		public float minimumInterval = 0f;
		public float maximumLength = 5f;
		public float lastFrameDelay = 2f;
		public int maxSize = -1;
		
		private CircularBuffer<ReplayFrame> buffer = null;
		private float timestamp;
		public Image display = null;
		
		int currentIndex = -1;
		ReplayFrame currentFrame = null;
		private float relativePhase;
		
		public void record() {
			if(isRecording()) {
				throw new ApplicationException("Already recording");
			} else if(buffer == null) {
				if(maxSize < 2) {
					throw new ApplicationException("maxSize not set");
				}
				
				buffer = new CircularBuffer<ReplayFrame>(maxSize);
			}
			state = ReplayManagerState.RECORDING;
			timestamp = JuloTime.gameTime();
			
			if(hasRecord())
				clear();
			
			takeFrame();
		}
		
		public void showReplay() {
			if(state != ReplayManagerState.NONE) {
				throw new ApplicationException("Invalid state");
			}
			state = ReplayManagerState.REPLAYING;
			timestamp = JuloTime.gameTime(); 
			currentFrame = null;
			currentIndex = -1;
		}
		
		public void stop() {
			if(!isRecording()) {
				throw new ApplicationException("Is not recording");
			}
			state = ReplayManagerState.NONE;
		}
		
		public bool isReplaying() {
			return state == ReplayManagerState.REPLAYING;
		}
		
		public bool isRecording() {
			return state == ReplayManagerState.RECORDING;
		}
		
		private void takeFrame() {
			StartCoroutine(__takeFrame());
		}
		private IEnumerator __takeFrame() {
			yield return new WaitForEndOfFrame();
			ReplayFrame frame = buffer.elemToOverride();
			int width  = Screen.width;
			int height = Screen.height;
			
			float now = JuloTime.gameTime();
			
			if(frame == null || frame.texture.width != width || frame.texture.height != height) {
				Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
				frame = new ReplayFrame(newTexture, now);
			} else {
				frame.timestamp = now;
			}
			
			frame.texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);  
			frame.texture.Apply();  
			
			buffer.save(frame);
		}
		
		public void showFrame(int frameIndex) {
			ReplayFrame frame = get(frameIndex);
			this.currentIndex = frameIndex;
			this.currentFrame = frame;
			
			Texture2D texture = frame.texture;
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100);
			
			display.gameObject.SetActive(true);
			display.sprite = sprite;
		}
		
		public void clear() {
			if(isRecording())
				stop();
			
			if(buffer != null)
				buffer.clear();
		}
		
		public bool hasRecord() {
			return buffer != null && buffer.length > 0;
		}
		
		public void LateUpdate() {
			if(Time.timeScale == 0f)
				return;
			
			if(state == ReplayManagerState.RECORDING) {
				float now = JuloTime.gameTime();
				if(buffer.length == 0 || now - buffer.get(buffer.length - 1).timestamp >= minimumInterval) {
					takeFrame();
				}
			} else if(state == ReplayManagerState.REPLAYING) {
				float ellapsed = JuloTime.gameTimeSince(timestamp);
				
				if(currentIndex == -1) {
					showFrame(0);
					relativePhase = currentFrame.timestamp;
				} else {
					bool isLast = (currentIndex == buffer.length - 1);
					
					if(isLast) {
						if(ellapsed > currentFrame.timestamp - relativePhase + lastFrameDelay) {
							state = ReplayManagerState.NONE;
							currentIndex = -1;
							currentFrame = null;
							display.gameObject.SetActive(false);
						}
					} else {
						int index = currentIndex;
						
						bool done = false;
						
						do {
							int nextIndex = index + 1;
							
							ReplayFrame nextFrame = get(nextIndex);
							if(ellapsed >= nextFrame.timestamp - relativePhase) {
								index++;
							} else {
								done = true;
							}
						} while(!done && index < buffer.length - 1);
						
						if(index != currentIndex) {
							showFrame(index);
						}
					}
				}
			}
		}
		
		public Texture2D getTexture(int index) {
			return get(index).texture;
		}
		
		private ReplayFrame get(int index) {
			ReplayFrame ret = buffer.get(index);
			if(ret == null)
				throw new ApplicationException("No element");
			return ret;
		}
	}
}