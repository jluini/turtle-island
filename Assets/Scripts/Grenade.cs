
using System;

using UnityEngine;

using JuloUtil;

namespace TurtleIsland {
	
	public class Grenade : Weapon {
		private float explosionTime = 1f;
		
		private enum State { NEW, CHARGED, EXPLODING, OFF }
		private State state;
		
		private int fuseTime;
		private float timestamp;
		
		private ParticleSystem explosion;
		private FrictionJoint2D friction;
		
		private TextDisplay _display;
		private TextDisplay display {
			get {
				if(_display == null) {
					_display = JuloFind.byName<TextDisplay>("Display", this);
				}
				return _display;
			}
		}
		
		private SpriteRenderer _renderer;
		private new SpriteRenderer renderer {
			get {
				if(_renderer == null)
					_renderer = JuloFind.byName<SpriteRenderer>("Renderer", this);
				return _renderer;
			}
		}
		
		public override void onInit() {
			state = State.NEW;
			display.transform.SetParent(game.env.hk.displayContainer.transform, false);
			
			explosion = JuloFind.byName<ParticleSystem>("Explosion", this);
			friction = GetComponent<FrictionJoint2D>();
			friction.enabled = false;
		}
		public override void onStep() {
			float ellapsed = JuloTime.gameTimeSince(timestamp);
			float remaining = fuseTime - ellapsed;
			
			if(state == State.CHARGED) {
				if(remaining > 0f) {
					showRemainingTime(remaining);
				} else {
					explode();
				}
			} else if(state == State.EXPLODING) {
				if(ellapsed >= explosionTime) {
					state = State.OFF;
					deactivate();
				}
			}
		}
		
		private void explode() {
			state = State.EXPLODING;
			renderer.enabled = false;
			
			display.deactivate();
			
			Vector2 position2D = (Vector2)transform.position;
			
			//showPoint(position2D.x, position2D.y, Color.blue);
			
			game.addForce(
				new DistanceWeighting(position2D, minimumDistance, maximumDistance, distancePower),
				maximumForce,
				maximumDamage
			);
			explosion.Play();
			
			renderer.enabled = false;
			friction.enabled = true;
			
			timestamp = JuloTime.gameTime();
		}
		
		public override void go(int value, Vector3 position, Vector2 direction, float shotTime) {
			if(state != State.NEW)
				throw new ApplicationException("Invalid call of go");
			
			state = State.CHARGED;
			fuseTime = value;
			
			transform.position = position;
			rb.velocity = direction * shotTime * game.options.weaponSpeedFactor;
			rb.angularVelocity = 0f;
			rb.AddTorque(game.options.grenadeTorque * Mathf.Sign(direction.x) * -1f);
			
			timestamp = JuloTime.gameTime();
			showRemainingTime(fuseTime);
			display.activate();
		}
		
		private void showRemainingTime(float remainingTime) {
			//display.showText(JuloMath.floatToString(value, JuloMath.TruncateMethod.CEIL, 1));
			display.showFloat(remainingTime, JuloMath.TruncateMethod.CEIL, 1);
		}
		
		public override bool isIdle() {
			return state == State.NEW || state == State.OFF;
		}
		
		public override void kill() {
			state = State.OFF;
			display.deactivate();
		}
		
		protected override bool isSinkable() {
			return state == State.NEW || state == State.CHARGED;
		}
		
		public override void onDestroy() {
			state = State.OFF;
			GameObject.Destroy(display.gameObject);
		}
		public override string getInfo(int value) {
			return value + " seg.";
		}
		/*
		private static SpriteRenderer _point = null;
		private static SpriteRenderer point {
			get {
				if(_point == null)
					_point = JuloFind.byName<SpriteRenderer>("PruPoint");
				return _point;
			}
		}
		private static SpriteRenderer showPoint(float x, float y, Color c) {
			SpriteRenderer p = UnityEngine.Object.Instantiate(point);
			p.transform.position = new Vector3(x, y, 0f);
			p.gameObject.SetActive(true);
			p.color = c;
			return p;
		}
		*/
	}
}
