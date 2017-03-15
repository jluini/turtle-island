
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using JuloUtil;

namespace TurtleIsland {
	public class Environment : MonoBehaviour, Behav, IFocusTarget {
		public enum Mode { NONE, TWO_PLAYERS, ONE_PLAYER, ZERO_PLAYERS }
		
		[HideInInspector]
		public InputManager inputManager;
		
		[HideInInspector]
		public Hooks hk;

		[HideInInspector]
		public Options options;
		
		[HideInInspector]
		public Level currentLevel;
		
		TurtleIslandGame currentGame;
		FocusTarget gameTarget;
		
		public void start(InputManager inputManager) {
			this.inputManager = inputManager;
			
			gameTarget = new FocusTarget();
			
			options = JuloFind.byName<Options>("Options", this);
			hk = new Hooks(this);
			currentLevel = JuloFind.byName<Level>("Level");
			currentLevel.load(this);
			
			hk.replayManager.display         = hk.replayDisplay;
			hk.replayManager.maxSize         = options.replayMaxSize;
			hk.replayManager.minimumInterval = options.replayMinimumInterval;
			hk.replayManager.lastFrameDelay  = options.replayDelay;
			
			hk.musicPlayer.init();
			
			if(options.musicInitiallyOn) {
				hk.musicPlayer.play();
			}
			
			//hk.soundsPlayer.setOn(options.soundsInitiallyOn);
			
			hk.cam.target = this;
			
		}
		
		public void update() {
			if(isPlaying()) {
				currentGame.step();
				hk.cam.updateCamera();
				
				// TODO
				if(currentGame.activeController && Input.GetButtonDown("WeaponValue")) {
					float rawValue = Input.GetAxisRaw("WeaponValue");
					if(rawValue > 0) {
						currentGame.activeController.incrementValue();
					} else if(rawValue < 0) {
						currentGame.activeController.decrementValue();
					} else {
						Debug.LogWarning("No se leyó ningún valor");
					}
				}
			}
		}
		
		public void play() {
			if(isPlaying()) {
				clearGame();
			}
			playGame(Mode.TWO_PLAYERS, 2, 3);
		}
		
		public void playCpu() {
			if(isPlaying()) {
				clearGame();
			}
			playGame(Mode.ONE_PLAYER, TurtleIsland.Hard, 3);
		}
		
		void playGame(Mode mode, int difficulty, int numCharacters) {
			setControl(mode);
			options.difficulty = difficulty;
			
			options.numberOfTurtles = numCharacters;
			
			currentGame = currentLevel.newGame(options);
			currentGame.init();
		}
		
		public bool isPlaying() {
			return currentGame != null;
		}
		
		void setControl(Mode mode) {
			switch(mode) {
			case Mode.TWO_PLAYERS:
				options.leftController = hk.userController;
				options.rightController = hk.userController;
				hk.teamDisplays[TurtleIsland.LeftTeamId].setTeamName("1P");
				hk.teamDisplays[TurtleIsland.RightTeamId].setTeamName("2P");
				break;
			case Mode.ONE_PLAYER:
				bool cpuIsLeft = JuloMath.randomBool();
				
				options.leftController = cpuIsLeft ? hk.leftRobotController : hk.userController;
				options.rightController = cpuIsLeft ? hk.userController      : hk.rightRobotController;
				hk.teamDisplays[TurtleIsland.LeftTeamId].setTeamName(cpuIsLeft ? "CPU" : "1P");
				hk.teamDisplays[TurtleIsland.RightTeamId].setTeamName(cpuIsLeft ? "1P" : "CPU");
				break;
			case Mode.ZERO_PLAYERS:
				options.leftController = hk.leftRobotController;
				options.rightController = hk.rightRobotController;
				hk.teamDisplays[TurtleIsland.LeftTeamId].setTeamName("CPU");
				hk.teamDisplays[TurtleIsland.RightTeamId].setTeamName("CPU");
				break;
			default:
				throw new ApplicationException("Invalid game mode");
			}
		}
		
		void clearGame() {
			foreach(TurtleIslandObject obj in currentGame.objects) {
				obj.onDestroy();
				GameObject.Destroy(obj.gameObject);
			}
			
			gameTarget.blur();
			
			currentGame = null;
		}
		
		/**************************/
		
		// TODO 
		void OnGUI() {
			Event e = Event.current;
			KeyCode cod = e.keyCode;
			
			if(e.type == EventType.KeyDown) {
				if(cod >= KeyCode.Alpha1 && cod <= KeyCode.Alpha5) {
					int number = (int)(cod - KeyCode.Alpha1) + 1;
					if(currentGame != null && currentGame.activeController != null)
						currentGame.activeController.setValue(number);
				} else if(cod >= KeyCode.Keypad1 && cod <= KeyCode.Keypad5) {
					int number = (int)(cod - KeyCode.Keypad1) + 1;
					if(currentGame != null && currentGame.activeController != null)
						currentGame.activeController.setValue(number);
				}
			}
		}
		
		/**************************/
		
		public void focusObject(Component obj) {
			gameTarget.focusObject(obj);
		}
		public void focusAny<T>(IList<T> objs) where T : Component {
			gameTarget.focusAny<T>(objs);
		}
		public void focusPosition(Vector3 position) {
			gameTarget.focusPosition(position);
		}
		public void blur() {
			gameTarget.blur();
		}
		public bool isFocused() {
			return gameTarget.isFocused();
			//return selectorState == SelectorState.ON || gameTarget.isFocused();
		}
		public Vector3 getFocusPosition() {
			return gameTarget.getFocusPosition();
			//return selectorState == SelectorState.ON ? new Vector3(targetX, 0f, 0f) : gameTarget.getFocusPosition();
		}
	}
}