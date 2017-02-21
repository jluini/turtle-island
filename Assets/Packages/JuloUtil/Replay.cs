using System;

using UnityEngine;

namespace JuloUtil {
	/*
	public class Replay {
		public Screenshot[] sequence;
		public Replay(Screenshot[] sequence) {
			this.sequence = sequence;
		}
	}
	*/
	public class Screenshot {
		public float timestamp;
		public Texture2D texture;
		
		public Screenshot(Texture2D texture) {
			this.timestamp = JuloTime.gameTime();
			this.texture = texture;
		}
	}
}