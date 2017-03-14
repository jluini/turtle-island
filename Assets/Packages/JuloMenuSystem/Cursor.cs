
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using JuloUtil;

namespace JuloMenuSystem {
	public class Cursor : MonoBehaviour {
		public float getPosition() {
			return transform.position.y;
		}
		
		public void setPosition(float newPos) {
			Vector3 pos = transform.position;
			transform.position = new Vector3(pos.x, newPos, pos.z);
		}
		
		public void hide() {
			setPosition(-1000f);
		}
		
	}
}
