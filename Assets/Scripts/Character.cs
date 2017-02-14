
using System;

using UnityEngine;

using JuloUtil;

namespace TurtleIsland {
	
	public class Character : TurtleIslandObject {
		public static int EAST = +1;
		public static int WEST = -1;
		
		[HideInInspector]
		public int teamId;
		[HideInInspector]
		public int life;
		
		[HideInInspector]
		public int orientation = EAST;
		
		[HideInInspector]
		public bool isSkeleton = false;
		
		private Controller controller = null;
		
		private float walkForce = 0f;
		
		private Transform _shotPoint;
		public Transform shotPoint {
			get {
				if(_shotPoint == null)
					_shotPoint = JuloFind.byName<Transform>("ShotPoint", this);
				return _shotPoint;
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
		
		private SpriteRenderer _minimapRenderer;
		public SpriteRenderer minimapRenderer {
			get {
				if(_minimapRenderer == null) {
					_minimapRenderer = JuloFind.byName<SpriteRenderer>("MinimapRenderer", this);
				}
				return _minimapRenderer;
			}
		}
		
		private Target _target;
		public Target target {
			get {
				if(_target == null)
					_target = JuloFind.byName<Target>("Target", this);
				return _target;
			}
		}
		
		private Animator _anim;
		private Animator anim {
			get {
				if(_anim == null)
					_anim = GetComponent<Animator>();
				return _anim;
			}
		}
		
		private LifeDisplay _display;
		public LifeDisplay display {
			get {
				if(_display == null)
					_display = JuloFind.byName<LifeDisplay>("Display", this);
				return _display;
			}
		}
		
		public override void onInit() {
			display.transform.SetParent(game.env.hk.displayContainer.transform, false);
			display.init(life, game.getColorForTeam(teamId));
			
			target.hide();
		}
		
		bool physicallyUpdated = false;
		void FixedUpdate() {
			physicallyUpdated = true;
		}
		public override void onStep() {
			if(physicallyUpdated) {
				Vector2 v = rb.velocity;
				
				float hAxis = Math.Abs(walkForce);
				float hSpeed = Math.Abs(v.x);
				float speed = v.magnitude;
				
				anim.SetBool("grounded", isGrounded());
				anim.SetBool("alive", isAlive());
				anim.SetFloat("hAxis", hAxis);
				anim.SetFloat("hSpeed", hSpeed);
				anim.SetFloat("speed", speed);
				
				walkForce = 0f;
				/*
				if(this == game.currentCharacter) {
					Debug.Log(String.Format(
						"{0}, {1}  ({2})",
						JuloMath.floatToString(hSpeed, JuloMath.TruncateMethod.ROUND, 4),
						JuloMath.floatToString(speed,  JuloMath.TruncateMethod.ROUND, 4),
						physicallyUpdated
					));
				}
				*/
			}
			
			if(controller != null) {
				physicallyUpdated = false;
				controller.step();
			}
		}
		
		public override bool isIdle() {
			return sinked || isSkeleton || isQuiet();
		}
		
		public bool isAlive() {
			return life > 0;
		}
		
		public bool isGrounded() {
			// TODO check if should be localRotation
			float rot = transform.rotation.eulerAngles.z;
			bool ret = rot < 90f || rot > 270f;
			return ret;
		}
		
		public bool isReady() {
			return isActive() && isAlive() && isGrounded();
		}
		
		public bool isLocked() {
			return isQuiet() && !isGrounded();
		}
		
		public bool isLeft() {
			return teamId == TurtleIsland.LeftTeamId;
		}
		
		public static Quaternion rotationQuat = Quaternion.Euler(0f, 180f, 0f);
		
		public void flip() {
			orientation *= -1;
			target.orientation = orientation;
			transform.rotation *= rotationQuat;
		}
		
		public bool hit(int damage) {
			if(!isAlive() || damage <= 0) {
				return false;
			} else {
				life -= damage;
				display.showDamage(damage);
				
				if(life <= 0) {
					life = 0;
					return true;
				} else {
					return false;
				}
			}
		}
		
		public void applyDamage() {
			if(display.isActive())
				display.applyDamage();
		}
		
		public void play(Controller controller) {
			this.controller = controller;
			controller.play(this);
			target.reset();
			target.show();
		}
		public void stop() {
			controller = null;
			target.hide();
		}
		
		public void dead() {
			isSkeleton = true;
			anim.SetTrigger("dead");
		}
		public void celebrate() {
			anim.SetTrigger("celebrate");
		}
		public void charge() {
			anim.SetTrigger("charge");
		}
		public void discharge() {
			anim.SetTrigger("discharge");
			target.hide();
		}
		
		public void walk(float walkForce) {
			if(orientation != Mathf.Sign(walkForce)) {
				flip();
			} else {
				rb.velocity = new Vector2(walkForce, rb.velocity.y);
			}
			this.walkForce = walkForce;
		}
		
		public override void kill() {
			display.deactivate();
			target.hide();
			life = 0;
		}
		
		public override void onDestroy() {
			GameObject.Destroy(display.gameObject);
		}
		
		protected override bool isSinkable() {
			return true;
		}
		public float localZ() {
			return transform.rotation.eulerAngles.z;
		}
		public float getTargetWorldAngle() {
			float ret = target.transform.rotation.eulerAngles.z;
			return orientation == EAST ? ret : 180f - ret;
		}
		
		public void moveTarget(float angularVelocity) {
			float angleDelta = angularVelocity * Time.deltaTime;
			target.rotate(angleDelta);
		}
		
		public int getTeamId() {
			return teamId;
		}
	}
	
}
