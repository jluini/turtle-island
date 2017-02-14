
using System;
using UnityEngine;

namespace JuloUtil {
	
	public class JuloTime {
		public static float gameTime() {
			return Time.time;
		}
		
		public static float gameTimeSince(float stamp) {
			return gameTime() - stamp;
		}
		public static float applicationTime() {
			return Time.unscaledTime;
		}
		
		public static float applicationTimeSince(float stamp) {
			return applicationTime() - stamp;
		}
	}
	
}