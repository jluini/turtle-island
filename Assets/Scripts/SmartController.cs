
using System;
using System.Collections.Generic;

using UnityEngine;

using JuloUtil;

namespace TurtleIsland {

public class SmartController : Controller {
	public bool debugEnabled = false;
	public float speedDeviation = 0f;
	public float angleDeviation = 0f;
	
	public float thinkingTimeMean = 1.5f;
	public float thinkingTimeDeviation = 1f;
	
	public float walkingTime = 1.8f;
	public float walkingTimeDeviation = 1f;
	
	public float targetSpeedMinimum = 0.1f;
	public float targetSpeedFactor = 0.02f;
	public float targetAngleThreshold = 0.1f;
	
	private TurtleIslandGame game;
	private Character character;
	
	private enum RobotState { THINKING, WALKING, THINKING2, AIMING, SHOTING, DONE }
	private StateMachine<RobotState> machine;
	
	private float currentTime;
	
	private float targetAngle;
	private int grenadeTime;
	
	public override void initialize(TurtleIslandGame game, int difficulty) {
		this.game = game;
		machine = new StateMachine<RobotState>(RobotState.THINKING);
		
		switch(difficulty) {
		case TurtleIsland.Easy:
			speedDeviation = 1.2f;
			angleDeviation = 6f;
			break;
		case TurtleIsland.Medium:
			speedDeviation = 0.6f;
			angleDeviation = 3f;
			break;
		case TurtleIsland.Hard:
			speedDeviation = 0.2f;
			angleDeviation = 1f;
			break;
		case TurtleIsland.Maximum:
			speedDeviation = 0f;
			angleDeviation = 0f;
			break;
		default:
			Debug.LogWarning("WARNING: difficulty greater than maximum");
			speedDeviation = 0f;
			angleDeviation = 0f;
			break;
		}
	}
	
	public override void play(Character character) {
		this.character = character;
		//status = TTPlayStatus.PREPARE;
		think(1f);
	}
	
	public override void dischargeForced() {
		//status = TTPlayStatus.DONE;
	}
	
	public override void step() {
		if(machine.state == RobotState.THINKING) {
			if(machine.isOver(currentTime)) {
				tryToShot();
			}
		} else if(machine.state == RobotState.WALKING) {
			if(machine.isOver(currentTime)) {
				think(0.5f);
				//tryToShot();
			} else {
				//game.walk(1f * (character.side ? 1f : -1f));
				game.walk(1f * (character.orientation == Character.EAST ? 1f : -1f));
			}
		} else if(machine.state == RobotState.AIMING) {
			// TODO check this
			
			float current = character.getTargetWorldAngle();
			
			if(Mathf.Abs(current - targetAngle) >= targetAngleThreshold) {
				float targetSpeed = (targetAngle - current) * targetSpeedFactor;
				
				if(Mathf.Abs(targetSpeed) < targetSpeedMinimum)
					targetSpeed = targetSpeedMinimum * Mathf.Sign(targetSpeed);
				
				if(character.orientation == Character.WEST) {
					targetSpeed *= -1f;
				}
				
				game.moveTarget(targetSpeed);
			} else {
				machine.trigger(RobotState.SHOTING);
				game.setWeaponValue(grenadeTime);
				game.charge();
			}
		} else if(machine.state == RobotState.SHOTING) {
			if(machine.triggerIfEllapsed(RobotState.DONE, currentTime)) {
				game.discharge();
			}
		}
	}
	
	public override void setValue(int number) {
		// nothing
	}
	public override void nextWeapon() {
		// nothing
	}
	
	void think(float factor) {
		float thinkTime;
		if(factor >= 1f) {
			thinkTime = JuloMath.randomFloat(thinkingTimeMean, thinkingTimeDeviation);
		} else {
			thinkTime = thinkingTimeMean * factor;
		}
		machine.trigger(RobotState.THINKING);
		currentTime = thinkTime;
	}
	
	void tryToShot() {
		List<Character> rivals = game.getRivals();
		int numRivals = rivals.Count;
		
		if(numRivals == 0) {
			think(1f);
			return;
		}
		
		TTShot shot = null;
		
		//Vector2 pa = character.shotPoint.position;
		Vector2 pa = character.transform.position;
		
		int rivalIndex = JuloMath.randomInt(0, numRivals - 1);
		
		for(int ft = 1; ft <= 3 && shot == null; ft++) {
			for(int i = 0; i < numRivals && shot == null; i++) {
				Character rival = rivals[(rivalIndex + i) % numRivals];
				Vector2 pb = (Vector2) rival.transform.position;
				
				if(Math.Abs(pa.x) > 2.2f && Math.Abs(pb.x) > 2.2f) {
					pa.x += .15f * Mathf.Sign(pb.x - pa.x);
					pb.x -= .15f * Mathf.Sign(pb.x - pa.x);
					pb.y -= 0.15f;
				}
				
				shot = TTShot.getPerfectShotForTime(pa, pb, ft);
			
				if(hitsSomeBarrier(shot)) {
					shot = null;
				}
			}
		}
		
		if(shot == null) {
			float remaining = game.getRemainingTime();
			
			bool leftSide = Mathf.Sign(pa.x) < 0f;
			
			if(remaining < walkingTime * 2) {
				if(debugEnabled) Debug.Log("Tiro apurado");
				shot = getTetaInterna(!leftSide, 3f, 5f);
			} else if(Mathf.Abs(pa.x) < 1.8f) {
				walk(leftSide);  // caminar hacia afuera
			} else if(Math.Abs(pa.x) > 6.5f) {
				walk(!leftSide); // caminar hacia adentro
			} else {
				//Character rival = rivals[0];
				
				int first = JuloMath.randomBool(0.5f) ? 3 : 4;
				int second = (first - 3) * -1 + 1 + 3;
				
				shot = getTetaInterna(!leftSide, first - 1, first);
				if(hitsSomeBarrier(shot)) {
					shot = getTetaInterna(!leftSide, second - 1, second);
					if(debugEnabled)
						Debug.Log(hitsSomeBarrier(shot) ? "Yet hitting barrier" : "Tiro de descarte 2");
				} else {
					if(debugEnabled) Debug.Log("Tiro de descarte 1");
				}
			}
		}
		
		if(shot != null)
			doShot(shot);
	}
	
	void walk(bool toLeft) {
		game.walk(0.1f * (toLeft ? -1f : +1f));
		currentTime = JuloMath.randomFloat(/*curve, */walkingTime, walkingTimeDeviation);
		machine.trigger(RobotState.WALKING);
	}
	
	void doShot(TTShot shot) {
		hitsSomeBarrier(shot/*, true*/);
		
		bool charSide = (character.orientation == Character.EAST);
		bool shotSide = shot.angle < 90f;
		
		if(charSide != shotSide) {
			game.walk(0.01f * (shotSide ? 1f : -1f));
		}
		
		shot.randomizeAngle(angleDeviation);
		shot.randomizeSpeed(speedDeviation);
		
		targetAngle = shot.angle;
		currentTime = shot.speed / game.options.weaponSpeedFactor;
		grenadeTime = (int)shot.fuse;
		machine.trigger(RobotState.AIMING);
	}
		
	private TTShot getTetaInterna(bool left, float tetaTime, float explosionTime) {
		GameObject[] caras = GameObject.FindGameObjectsWithTag("CaraInterna");
		
		bool isFirst = caras[0].transform.position.x < 0 == left;
		GameObject cara = isFirst ? caras[0] : caras[1];
		
		Vector2 pa = character.shotPoint.position;
		Vector2 pb = cara.transform.position;
		
		TTShot ret = TTShot.getPerfectShotForTime(pa, pb, tetaTime);
		ret.fuse = explosionTime;
		
		return ret;
	}
	
	private static bool hitsSomeBarrier(TTShot shot/*, bool debugInfo = false*/) {
		GameObject[] barrierObjs = GameObject.FindGameObjectsWithTag("Barrier");
		foreach(GameObject barrier in barrierObjs) {
			if(shot.under(barrier.transform.position/*, debugInfo*/)) {
				return true;
			}
		}
		return false;
	}
	
}

}
