
using System;

using UnityEngine;

using JuloUtil;
using JuloGame;

namespace TurtleIsland {
	
	public enum TTPlayStatus { PREPARE, CHARGE, DONE, OVER }
	
	public enum CharacterStatus { READY, INACTIVE, DEAD }
	
	public class TurtleIslandStatus {
		public int readyTeams;
		public int readyChars;
		
		// int numTeams;
		// int numCharacters;
		
		CharacterStatus[][] characterStatus;
		
		public TurtleIslandStatus(int numTeams, int numCharacters) {
			// this.numTeams = numTeams;
			// this.numCharacters = numCharacters;
			
			characterStatus = new CharacterStatus[numTeams][];
			for(int t = 0; t < numTeams; t++) {
				characterStatus[t] = new CharacterStatus[numCharacters];
			}
		}
		
		public bool isOver() {
			return readyTeams < 2;
			/*
			int numberOfReadyTeams = 0;
			for(int t = 0; t < numTeams; t++) {
				bool isReady = false;
				for(int c = 0; c < numCharacters; c++) {
					if(characterStatus[t][c] == CharacterStatus.READY) {
						isReady = true;
						break;
					}
				}
				if(isReady) {
					numberOfReadyTeams++;
				}
			}
			
			return numberOfReadyTeams < 2;
			*/
		}
	}
	
	public class TurtleIsland {
		public const int NumTeams = 2;
		public const int LeftTeamId = 0;
		public const int RightTeamId = 1;
		public const string LeftTeamName = "Left";
		public const string RightTeamName = "Right";
		
		public const int Easy = 0;
		public const int Medium = 1;
		public const int Hard = 2;
		public const int Maximum = 3;
	}
	
	public abstract class TurtleIslandObject : GameObj {
		public bool sinked = false;
		
		protected TurtleIslandGame game = null;
		
		private Rigidbody2D _rb;
		public Rigidbody2D rb { // TODO should be protected ?
			get {
				if(_rb == null)
					_rb = GetComponent<Rigidbody2D>();
				return _rb;
			}
		}
		
		public void init(TurtleIslandGame game) {
			this.game = game;
			this.onInit();
		}
		public override void step() {
			if(isSinkable() && !sinked && game.isUnderWater(this)) {
				game.sink(this);
				sinked = true;
				kill();
			}
			onStep();
		}
		
		public abstract void onInit();
		public abstract void onStep();
		public abstract bool isIdle();
		
		public abstract void kill();
		protected abstract bool isSinkable();
		
		public bool isQuiet() {
			Vector2 vel = rb.velocity;
			float angularVel = rb.angularVelocity;
			
			return vel.magnitude < 0.001f && JuloMath.abs(angularVel) < 0.001f;
		}
	}
	
	public abstract class Weapon : TurtleIslandObject {
		public float maximumForce;
		public float maximumDamage;
		public float minimumDistance;
		public float maximumDistance;
		public float distancePower;
		
		public abstract void go(int value, Vector3 position, Vector2 direction, float shotTime);
		public abstract string getInfo(int value);
		public Sprite getImage() {
			SpriteRenderer rend = JuloFind.descendant<SpriteRenderer>(this);
			return rend.sprite;
		}
	}
	
	public abstract class Controller : MonoBehaviour, Steppable {
		public abstract void initialize(TurtleIslandGame game, int difficulty);
		
		public abstract void play(Character character);
		public abstract void step();
		
		public abstract void dischargeForced();
		
		public abstract void setValue(int number);
		public abstract void incrementValue();
		public abstract void decrementValue();
		public abstract void nextWeapon();
	}
	
	public interface ForceWeighting {
		Vector2 weight(TurtleIslandObject obj);
	}
	
	public class DistanceWeighting : ForceWeighting {
		private Vector2 referencePosition;
		private float minimumDistance;
		private float maximumDistance;
		
		private float minPow;
		private float power;
		
		public DistanceWeighting(Vector2 referencePosition, float minimumDistance, float maximumDistance, float power = 1f) {
			this.referencePosition = referencePosition;
			this.minimumDistance = minimumDistance;
			this.maximumDistance = maximumDistance;
			
			this.power = power;
			this.minPow = Mathf.Pow(minimumDistance, power);
			
			//float th = minimumDistance * Mathf.Pow(2 * 100f, 1 / power);
			//Debug.Log("Theoric maximum distance is (100f): " + th);
		}
		
		public Vector2 weight(TurtleIslandObject obj) {
			Vector2 objPosition = (Vector2) obj.transform.position;
			Vector2 difference = objPosition - referencePosition;
			
			float dist = difference.magnitude;
			
			float factor;
			if(dist <= minimumDistance) {
				factor = 1f;
			} else if(dist >= maximumDistance) {
				factor = 0f;
			} else {
				factor = minPow / Mathf.Pow(dist, power);
			}
			
			Vector2 direction = difference.normalized;
			
			return direction * factor;
		}
	}
	
	public class CurveDistanceWeighting : ForceWeighting {
		private Vector2 referencePosition;
		private AnimationCurve curve;
		
		public CurveDistanceWeighting(Vector2 referencePosition, AnimationCurve curve) {
			this.referencePosition = referencePosition;
			this.curve = curve;
		}
		
		public Vector2 weight(TurtleIslandObject obj) {
			Vector2 objPosition = (Vector2) obj.transform.position;
			Vector2 difference = objPosition - referencePosition;
			Vector2 direction = difference.normalized;
			float dist = difference.magnitude;
			
			float factor = curve.Evaluate(dist);
					
			return direction * factor;
		}
	}
	
	public class TTShot {
		public Vector2 origin;
		
		public float angle;
		public float speed;
		public float fuse;
		
		public TTShot(Vector2 origin, float angle, float speed, float fuse) {
			this.origin = origin;
			this.angle = angle;
			this.speed = speed;
			this.fuse= fuse;
		}
		
		public void randomizeAngle(/*AnimationCurve curve, */float deviation) {
			angle = JuloMath.randomFloat(/*curve, */angle, deviation);
		}
		public void randomizeSpeed(/*AnimationCurve curve, */float deviation) {
			speed = JuloMath.randomFloat(/*curve, */speed, deviation);
		}
		
		public bool under(Vector2 target/*, bool debugInfo = false*/) {
			float t = (target.x - origin.x) / speedX();
			
			
			if(t < 0f) {
				//Debug.Log(String.Format("Barrier {0} at {1}: {2}", target, t, "antes"));
				return false;
			} else if(t > fuse) {
				//Debug.Log(String.Format("Barrier {0} at {1}: {2}", target, t, "despues"));
				return false;
			}
			
			float y = origin.y + speedY() * t + gravity() * t * t / 2;
			bool ret = y <= target.y;
			/*
			if(debugInfo) {
				Debug.Log(String.Format("Barrier {0} at {1}: {2} - {3} {4} {5}",
					target,
					t,
					ret ? "CHOCA!" : "arriba",
					y,
					ret ? "<=" : ">",
					target.y
				));
			}
			*/
			return ret;
		}
		public static float gravity() {
			return Physics2D.gravity.y;
		}
		public float speedX() {
			return speed * Mathf.Cos(angle * JuloMath.PI_180);
		}
		public float speedY() {
			return speed * Mathf.Sin(angle * JuloMath.PI_180);
		}
		
		public static TTShot getSpeculativeShotForTime(Vector2 pa, Vector2 pb, float time) {
			pb.x = pa.x + 2 * (pb.x - pa.x) / 3;
			TTShot ret = getPerfectShotForTime(pa, pb, 2f);
			ret.fuse = time;
			return ret;
		}
		
		public static TTShot getPerfectShotForTime(Vector2 pa, Vector2 pb, float time) {
			bool flip = pb.x < pa.x;
			
			Vector2 dd = pb - pa;
			if(flip)
				dd.x = -dd.x;
			
			Vector2 vel = new Vector2(dd.x / time, dd.y / time - gravity() * time / 2f);
			
			float angle = JuloMath.degrees(vel);
			
			if(angle <= -90f || angle >= +90f) {
				Debug.Log("Angulo raro");
			}
			
			if(flip)
				angle = 180f - angle;
			
			float speed = Mathf.Sqrt(vel.x * vel.x + vel.y * vel.y);
			
			TTShot ret = new TTShot(pa, angle, speed, time);
			// ret.show();
			return ret;
		}
		/*
		private void show() {
			showShotPoint(0.7f, Color.green);
			showShotPoint(0.8f, Color.green);
			showShotPoint(0.9f, Color.green);
			showShotPoint(  1f, Color.red);
		}
		private SpriteRenderer showShotPoint(float nt, Color c) {
			float t = nt * fuse;
			float radians = angle * JuloMath.PI_180;
			Vector2 vel = new Vector2(speed * Mathf.Cos(radians), speed * Mathf.Sin(radians));
			float x = origin.x + t * vel.x;
			float y = origin.y + t * vel.y + (t * t * gravity()) / 2f;
			
			SpriteRenderer p = showPoint(x, y, c);
			//p.gameObject.name = "PruPoint" + nt;
			return p;
		}
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
		}*/
	}
}