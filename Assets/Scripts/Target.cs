
using UnityEngine;

using JuloUtil;

namespace TurtleIsland {
	
	public class Target : JuloBehaviour {
		private float _relativeAngle = 0f;
		public float relativeAngle {
			get {
				return _relativeAngle;
			}
			set {
				_relativeAngle = value;
				updateAngle();
			}
		}
		public float effectiveAngle {
			get {
				if(orientation == Character.EAST) {
					return relativeAngle;
				} else {
					return 180f - relativeAngle;
				}
			}
		}
		private int _orientation = Character.EAST;
		public int orientation {
			get {
				return _orientation;
			}
			set {
				_orientation = value;
				updateAngle();
			}
		}
		
		private SpriteRenderer _display;
		private SpriteRenderer display {
			get {
				if(_display == null)
					_display = GetComponent<SpriteRenderer>();
				return _display;
			}
		}
		
		static float MIN_ANGLE =  -30f;
		static float MAX_ANGLE = +120f;
		
		public void reset() {
			relativeAngle = JuloMath.randomFloat(45f, 15f);
		}
		
		public void show() {
			display.enabled = true;
		}
		
		public void hide() {
			display.enabled = false;
		}
		
		public void rotate(float angleDelta) {
			float newAngle = relativeAngle + angleDelta;
			newAngle = JuloMath.minimax(newAngle, MIN_ANGLE, MAX_ANGLE);
			
			relativeAngle = newAngle;
		}
		
		private void updateAngle() {
			transform.localRotation = Quaternion.Euler(0, 0, relativeAngle);
		}
	}
}