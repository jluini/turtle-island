using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace JuloUtil {
	
	public class ReplayManager : MonoBehaviour {
		public bool recording = false;
		public float timestamp;
		
		public float interval = 0.3f;
		public float maximumLength = 5f;
		public Image display = null;
		
		private CircularBuffer<Screenshot> buffer = null;
		
		public void record() {
			if(recording) {
				throw new ApplicationException("Already recording");
			} else if(buffer == null) {
				int maxSize = (int)(maximumLength / interval) + 1;
				buffer = new CircularBuffer<Screenshot>(maxSize);
			}
			recording = true;
			timestamp = JuloTime.gameTime();
			buffer.clear();
			takeFrame();
		}
		
		public bool isRecording() {
			return recording;
		}
		
		private void takeFrame() {
			StartCoroutine(__takeFrame());
		}
		private IEnumerator __takeFrame() {
			yield return new WaitForEndOfFrame();
			Screenshot frame = buffer.elemToOverride();
			int width  = Screen.width;
			int height = Screen.height;
			
			if(frame == null || frame.texture.width != width || frame.texture.height != height) {
				Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
				frame = new Screenshot(newTexture);
			} else {
				frame.updateTimestamp();
			}
			//RenderTexture.active = null; // TODO ??
			frame.texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);  
			frame.texture.Apply();  
			
			buffer.save(frame);
		}
		
		public void showFrame(int frameNumber) {
			Texture2D texture = get(frameNumber).texture;
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 100);
			
			display.gameObject.SetActive(true);
			display.sprite = sprite;
		}
		
		public void stop() {
			if(!recording) {
				throw new ApplicationException("Is not recording");
			}
			recording = false;
		}
		
		public void LateUpdate() {
			if(recording) {
				float now = JuloTime.gameTime();
				float ellapsed = now - timestamp;
				if(ellapsed >= interval) {
					timestamp = now;
					takeFrame();
				}
			}
		}
		
		public int getReplayLength() {
			return buffer.length;
		}
		
		public Screenshot get(int index) {
			Screenshot ret = buffer.get(index);
			if(ret == null)
				throw new ApplicationException("No element");
			return ret;
		}
	}
}