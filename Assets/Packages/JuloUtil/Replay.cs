using System;

using UnityEngine;

namespace JuloUtil {
	public class Replay {
		public Screenshot[] sequence;
		public Replay(Screenshot[] sequence) {
			this.sequence = sequence;
			/*
			Debug.Log("Creating replay");
			for(int i = 0; i < sequence.Length; i++) {
				Texture2D frame = sequence[i];
				Debug.Log(frame);
			}
			*/
		}
	}
	public class Screenshot {
		public float timestamp;
		public Texture2D texture;
		
		public Screenshot(Texture2D texture) {
			this.timestamp = JuloTime.gameTime();
			this.texture = texture;
		}
	}
}