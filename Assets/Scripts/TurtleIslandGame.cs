
using System;
using System.Collections.Generic;

using UnityEngine;

using JuloUtil;
using JuloGame;

namespace TurtleIsland {
	
	public class TurtleIslandGame : Game<TurtleIslandObject, Options> {
		public Environment env;
		public Level level;
		public TeamManager teamManager;
		
		public Controller activeController = null;
		
		// State management
		private enum State { WAIT, APPLY_DAMAGE, DEAD, PREPARE, PLAY, OVER, CELEBRATE, REPLAY }
		private StateMachine<State> machine;
		private TTPlayStatus playStatus;
		
		public Character currentCharacter = null; // TODO make private
		private Team currentTeam = null;
		private Team leftTeam;
		private Team rightTeam;
		
		private List<Character> damagedCharacters;
		private List<Character> deadCharacters;
		
		private TurtleIslandStatus oldStatus;
		private TurtleIslandStatus gameStatus;
		
		public TurtleIslandGame(Environment env, Level level, int numCharacters) : base(env.options) {
			this.env = env;
			this.level = level;
			
			env.hk.replayManager.clear();
			
			this.machine = new StateMachine<State>(State.WAIT);
			damagedCharacters = new List<Character>();
			deadCharacters = new List<Character>();
			
			this.teamManager = new TeamManager(2, numCharacters); // TODO detect teams from characters
			leftTeam  = new Team(TurtleIsland.LeftTeamId,  TurtleIsland.LeftTeamName,  options.leftColor,  options.leftController);
			rightTeam = new Team(TurtleIsland.RightTeamId, TurtleIsland.RightTeamName, options.rightColor, options.rightController);
			
			this.teamManager.addTeam(leftTeam);
			this.teamManager.addTeam(rightTeam);
			
			leftTeam.setWeapon(0, options.initialWeaponValue);
			rightTeam.setWeapon(0, options.initialWeaponValue);
			
			updateWeaponInfo();
			
			this.teamManager.startsTeam(JuloMath.randomBool() ? TurtleIsland.LeftTeamId : TurtleIsland.RightTeamId);
			
			options.leftController.initialize(this, options.difficulty);
			options.rightController.initialize(this, options.difficulty);
			
			hideControls();
		}
		
		public void addCharacter(Character c) {
			c.activate();
			addObject(c);
			teamManager.addCharacter(c);
			c.init(this);
		}
		
		public Color getColorForTeam(int teamId) {
			if(teamId == TurtleIsland.LeftTeamId) {
				return options.leftColor;
			} else if(teamId == TurtleIsland.RightTeamId) {
				return options.rightColor;
			} else {
				throw new ApplicationException("Invalid team");
			}
		}
		
		public override void onInit() {
			//Debug.Log("Initting");
		}
		
		public override void onStep() {
			if(machine.state == State.WAIT) {
				if(!allIsQuiet()) {
					machine.trigger(State.WAIT); // resets wait counter
				} else if(machine.isOver(options.waitTime)) {
					this.oldStatus = this.gameStatus;
					this.gameStatus = teamManager.getStatus();
					
					if(env.hk.replayManager.isRecording()) {
						env.hk.replayManager.stop();
					}
					
					if(env.hk.replayManager.hasRecord()) {
						if(isReplayable()) {
							triggerReplay();
						} else {
							triggerApply();
							env.hk.replayManager.clear();
						}
					} else {
						triggerApply();
					}
				}
			} else if(machine.state == State.APPLY_DAMAGE) {
				if(machine.isOver(options.applyDamageTime)) {
					List<Character> reallyDeadCharacters = new List<Character>();
					
					foreach(Character c in deadCharacters) {
						if(c.isActive() && !c.sinked) {
							c.dead();
							reallyDeadCharacters.Add(c);
						}
					}
					
					if(reallyDeadCharacters.Count > 0) {
						env.focusAny(reallyDeadCharacters);
						machine.trigger(State.DEAD);
					} else {
						endOfTurn();
					}
					
					deadCharacters.Clear();
				}
			} else if(machine.state == State.PREPARE) {
				if(machine.triggerIfEllapsed(State.PLAY, options.prepareTime)) {
					currentCharacter.play(activeController);
					
					playStatus = TTPlayStatus.PREPARE;
				}
			} else if(machine.state == State.DEAD) {
				if(machine.isOver(options.deathTime)) {
					endOfTurn();
				}
			} else if(machine.state == State.PLAY) {
				if(!currentCharacter.isAlive()) {
					endTurn();
				} else if(playStatus == TTPlayStatus.PREPARE) {
					float remaining = getRemainingTime();
					
					//if(machine.isOver(options.playTime) || currentCharacter.isLocked()) {
					if(remaining <= 0f || currentCharacter.isLocked()) {
						endTurn();
						showMainTime(0f);
					} else {
						showMainTime(remaining);
					}
				} else if(playStatus == TTPlayStatus.CHARGE) {
					if(machine.isOver(options.maximumChargeTime)) {
						activeController.dischargeForced();
						discharge();
					}
				} else if(playStatus == TTPlayStatus.DONE) {
					if(machine.isOver(options.postTime)) {
						endTurn();
					}
				} else {
					throw new ApplicationException("Unknown state");
				}
			} else if(machine.state == State.OVER) {
				if(machine.triggerIfEllapsed(State.CELEBRATE, options.overTime)) {
					Team winner = teamManager.getSomeReadyTeam();
					
					if(winner != null) {
						if(env.hk.soundsControl.isOn())
							env.hk.soundsControl.soundSource.playClip(options.winClip, 1f);
						
						foreach(Character c in winner) {
							if(c.isReady()) {
								c.celebrate();
							}
						}
						
						env.hk.teamDisplays[winner.id].trigger("Win");
					} else {
						if(env.hk.soundsControl.isOn())
							env.hk.soundsControl.soundSource.playClip(options.drawClip, 1f);
					}
				}
			} else if(machine.state == State.CELEBRATE) {
				if(machine.ellapsed() >= options.celebrateTime) {
					// TODO restart();
				}
			} else if(machine.state == State.REPLAY) {
				if(!env.hk.replayManager.isReplaying()) {
					triggerApply();
					env.hk.replayManager.clear();
				}
			} else {
				Debug.LogWarning("Unknown state: " + machine.state);
			}
		}
		public override void onDeactivate(TurtleIslandObject obj) {
			obj.kill();
		}
		public override bool isOver() {
			return machine.state == State.CELEBRATE;
		}
		public float getRemainingTime() {
			return options.playTime - machine.ellapsed();
		}
		public void walk(float axis) {
			if(machine.state != State.PLAY || playStatus == TTPlayStatus.CHARGE)
				throw new ApplicationException("Invalid call of walk()");
			
			if(currentCharacter.isGrounded()) {
				currentCharacter.walk(axis * options.walkSpeed);
			}
		}
		
		public void moveTarget(float axis) {
			if(machine.state != State.PLAY || (playStatus != TTPlayStatus.PREPARE && playStatus != TTPlayStatus.CHARGE))
				throw new ApplicationException("Invalid call of moveTarget()");
			currentCharacter.moveTarget(axis * options.targetSpeed);
		}
		
		public bool isUnderWater(TurtleIslandObject obj) {
			Vector2 pos = (Vector2) obj.transform.position;
			Vector2 waterLeft  = level.waterLeft.transform.position;
			Vector2 waterRight = level.waterRight.transform.position;
			
			return (pos.x <= waterLeft.x && pos.y <= waterLeft.y) || (pos.x >= waterRight.x && pos.y <= waterRight.y);
		}
		
		public void sink(TurtleIslandObject obj) {
			if(env.hk.soundsControl.isOn())
				env.hk.soundsControl.soundSource.playClip(options.sinkClip, 1f);
			
			Rigidbody2D b = obj.rb;
			b.drag = 10;
			b.angularDrag = 10;
		}
		
		public void charge() {
			if(machine.state != State.PLAY || playStatus != TTPlayStatus.PREPARE)
				throw new ApplicationException("Invalid call of charge()");
			machine.trigger(State.PLAY); // resets machine counter
			playStatus = TTPlayStatus.CHARGE;
			currentCharacter.charge();
		}
		public void discharge() {
			if(machine.state != State.PLAY || playStatus != TTPlayStatus.CHARGE)
				throw new ApplicationException("Invalid call of discharge()");
			
			float chargeTime = machine.ellapsed();
			
			AudioClip shotClip = chargeTime < options.shotClipThreshold ? options.shot1Clip : options.shot2Clip;
			
			if(env.hk.soundsControl.isOn())
				env.hk.soundsControl.soundSource.playClip(shotClip, 1f);
			
			machine.trigger(State.PLAY); // resets machine counter
			playStatus = TTPlayStatus.DONE;
			
			currentCharacter.discharge();
			
			Weapon w = level.addNewWeapon(
				this,
				currentTeam.weaponIndex,
				currentTeam.weaponValue,
				currentCharacter.shotPoint.position,
				JuloMath.unitVector(currentCharacter.getTargetWorldAngle()),
				chargeTime
			);
			
			env.focusObject(w);
			
			//hideTurnControls();
			
			if(options.showReplay) {
				env.hk.replayManager.record();
			}
		}
		
		public void setWeaponValue(int value) {
			if(activeController != null) {
				currentTeam.weaponValue = value;
				updateWeaponInfo();
			} else {
				Debug.Log("Invalid call of setWeaponValue");
			}
		}
		public void nextWeapon() {
			if(activeController != null) {
				currentTeam.setWeapon(
					(currentTeam.weaponIndex + 1) % env.currentLevel.weapons.Length,
					options.initialWeaponValue
				);
				updateWeaponInfo();
			} else {
				Debug.Log("Invalid call of nextWeapon");
			}
		}
		
		public List<Character> getRivals() {
			return teamManager.getReadyRivals(currentCharacter);
		}
		
		public void addForce(ForceWeighting weighting, float forceScale, float damageScale) {
			if(env.hk.soundsControl.isOn())
				env.hk.soundsControl.soundSource.playClip(options.explosionClip, 1f);
			
			float maxMagnitude = 0;
			Character maxMagnitudeChar = null;
			
			foreach(Character c in teamManager.allActiveCharacters()) {
				Vector2 f = weighting.weight(c);
				float magnitude = f.magnitude;
				
				if(magnitude > 0f) {
					c.rb.AddForce(f * forceScale);
				}
				
				if(c.isAlive()) {
					float fDamage = magnitude * damageScale;
					int damage = (int)(Mathf.Round(fDamage));
					
					if(damage > 0) {
						damagedCharacters.Add(c);
						if(c.hit(damage)) {
							deadCharacters.Add(c);
						}
					}
					
					if(magnitude > maxMagnitude) {
						maxMagnitude = magnitude;
						maxMagnitudeChar = c;
					}
				}
			}
			
			if(maxMagnitudeChar != null) {
				env.focusObject(maxMagnitudeChar);
			}
		}
		
		private bool allIsQuiet() {
			foreach(TurtleIslandObject obj in objects) {
				if(!obj.isIdle()) {
					return false;
				}
			}
			return true;
		}
		private bool isReplayable() {
			return gameStatus.readyChars != oldStatus.readyChars;
			//return gameStatus.isOver();
		}
		
		private void endOfTurn() {
			if(gameStatus.isOver()) {
				triggerGameOver();
			} else {
				triggerNextTurn();
			}
		}
		
		private void triggerReplay() {
			machine.trigger(State.REPLAY);
			env.hk.replayManager.showReplay();
		}
		
		private void triggerApply() {
			List<Character> reallyDamagedCharacters = new List<Character>();
			
			foreach(Character c in damagedCharacters) {
				if(c.isActive() && !c.sinked) {
					c.applyDamage();
					reallyDamagedCharacters.Add(c);
				}
			}
			
			if(reallyDamagedCharacters.Count > 0) {
				//env.focusAny(reallyDamagedCharacters);
				machine.trigger(State.APPLY_DAMAGE);
			} else {
				endOfTurn();
			}
			
			damagedCharacters.Clear();
		}
		
		private void triggerGameOver() {
			machine.trigger(State.OVER);
			Team winner = teamManager.getSomeReadyTeam();
			
			if(winner != null) {
				env.focusObject(teamManager.getLastCharacterOf(winner));
			}
		}
		
		private void triggerNextTurn() {
			machine.trigger(State.PREPARE);
			
			currentCharacter = teamManager.nextCharacter();
			currentTeam = teamManager.currentTeam();
			activeController = currentTeam.controller;
			
			env.focusObject(currentCharacter);
			showTurnControls();
		}
		private void endTurn() {
			playStatus = TTPlayStatus.OVER;
			machine.trigger(State.WAIT);
			currentCharacter.stop();
			
			hideTurnControls();
			
			activeController = null;
		}
		private void hideControls() {
			env.hk.mainTimeDisplay.trigger("Off");
			env.hk.teamDisplays[TurtleIsland.LeftTeamId].trigger("Off");
			env.hk.teamDisplays[TurtleIsland.RightTeamId].trigger("Off");
		}
		private void showTurnControls() {
			showMainTime(options.playTime);
			env.hk.mainTimeDisplay.trigger("Show");
			env.hk.teamDisplays[currentTeam.id].trigger("Play");
		}
		private void showMainTime(float remaining) {
			env.hk.mainTimeDisplay.showFloat(remaining, JuloMath.TruncateMethod.CEIL, 0);
		}
		private void hideTurnControls() {
			env.hk.mainTimeDisplay.trigger("Hide");
			env.hk.teamDisplays[currentTeam.id].trigger("Stop");
		}
		private void updateWeaponInfo() {
			env.hk.teamDisplays[TurtleIsland.LeftTeamId].setWeapon(level.weapons[leftTeam.weaponIndex], leftTeam.weaponValue);
			env.hk.teamDisplays[TurtleIsland.RightTeamId].setWeapon(level.weapons[rightTeam.weaponIndex], rightTeam.weaponValue);
		}
	}
}