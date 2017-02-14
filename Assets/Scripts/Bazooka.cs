
using System;

using UnityEngine;

using JuloUtil;

namespace TurtleIsland {
	
	public class Bazooka : Weapon {
		public float windFactor = 0.1f;
		
		private float explosionTime = 1f;
		private float wind = 0f;
		
		private enum State { NEW, CHARGED, EXPLODING, OFF }
		private State state;
		
		private float timestamp;
		
		private ParticleSystem explosion;
		private FrictionJoint2D friction;
		
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
			
			explosion = JuloFind.byName<ParticleSystem>("Explosion", this);
			friction = GetComponent<FrictionJoint2D>();
			friction.enabled = false;
		}
		public override void onStep() {
			Vector2 vel = rb.velocity;
			float newAngle = JuloMath.degrees(vel);
			transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
			
			if(state == State.EXPLODING) {
				float ellapsed = JuloTime.gameTimeSince(timestamp);
				if(ellapsed >= explosionTime) {
					state = State.OFF;
					deactivate();
				}
			} else if(state == State.CHARGED) {
				vel.x += wind;
				rb.velocity = vel;
			}
		}
		
		public void OnCollisionEnter2D() {
			if(state == State.CHARGED) {
				explode();
			}
		}
		
		private void explode() {
			state = State.EXPLODING;
			renderer.enabled = false;
			
			Vector2 position2D = (Vector2)transform.position;
			
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
			
			wind = windFactor * (value - 3);
			
			state = State.CHARGED;
			
			transform.position = position;
			rb.velocity = direction * shotTime * game.options.weaponSpeedFactor;
			rb.angularVelocity = 0f;
			
			timestamp = JuloTime.gameTime();
		}
		public override bool isIdle() {
			return state == State.NEW || state == State.OFF;
		}
		
		public override void kill() {
			state = State.OFF;
		}
		
		protected override bool isSinkable() {
			return state == State.NEW || state == State.CHARGED;
		}
		
		public override void onDestroy() {
			state = State.OFF;
		}
		public override string getInfo(int value) {
			switch(value) {
			case 1: return "<----";
			case 2: return " <-- ";
			case 3: return "  -  ";
			case 4: return " --> ";
			case 5: return "---->";
			default: return "";
			}
		}
	}
	
}
