using System;
using System.Collections;

using UnityEngine;

namespace JuloUtil {
	
	public class ReplayManager : MonoBehaviour {
		public bool recording = false;
		public float timestamp;
		
		public float interval = 0.3f;
		public float maximumLength = 5f;
		
		private CircularBuffer<Screenshot> buffer;
		
		void Start() {
			int maxSize = (int)(maximumLength / interval) + 1;
			buffer = new CircularBuffer<Screenshot>(maxSize);
		}
		
		public void record() {
			if(recording) {
				throw new ApplicationException("Already recording");
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
			Texture2D frame = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);  
			RenderTexture.active=null;  //`enter code here`
			frame.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);  
			frame.Apply();  
			//lastFrame = texture;
			
			buffer.save(new Screenshot(frame));
			
			//Debug.Log("Taking frame: " + buffer.getSize());
		}
		
		public void stop() {
			if(!recording) {
				throw new ApplicationException("Is not recording");
			}
			recording = false;
			
			//CancelInvoke("takeFrame");
			// ...
		}
		
		public void LateUpdate() {
			if(recording) {
				float now = JuloTime.gameTime();
				float ellapsed = now - timestamp;
				if(ellapsed >= interval) {
					timestamp = now;
					//Debug.Log("other call");
					takeFrame();
				}
			}
		}
		
		public Replay getReplay() {
			return new Replay(buffer.getSequence());
		}
	}
}