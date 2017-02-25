
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using JuloUtil;

namespace TurtleIsland {
	
	public class Environment : MonoBehaviour, IFocusable, IFocusTarget {
		public enum State { MENU, PLAYING, PAUSED }
		private bool isOver = false;
		private bool autoMenuIsPending = false;
		private float overTime;
		
		public enum Mode { NONE, TWO_PLAYERS, ONE_PLAYER, ZERO_PLAYERS }
		
		[HideInInspector]
		public State state = State.MENU;
		private StateMachine<State> machine;
		
		[HideInInspector]
		public Level currentLevel = null;
		
		//public CircularBuffer<Texture2D> circularBuffer = new CircularBuffer<Texture2D>(15);
		
		//public Texture2D lastFrame;
		
		private TurtleIslandGame currentGame = null;
		private bool fakeGame = false;
		
		private FocusTarget gameTarget = new FocusTarget();
		private float targetX = 0f;
		private enum SelectorState { OFF, HOVER, ON }
		private SelectorState selectorState = SelectorState.OFF;
		
		private Menu _menu;
		private Menu menu {
			get {
				if(_menu == null)
					_menu = JuloFind.byName<Menu>("MenuContainer", this);
				return _menu;
			}
		}
		
		private Hooks _hk;
		public Hooks hk {
			get {
				if(_hk == null) {
					_hk = new Hooks(this);
				}
				return _hk;
			}
		}
		
		private Options _options;
		public Options options {
			get {
				if(_options == null)
					_options = JuloFind.byName<Options>("Options", this);
				return _options;
			}
		}
		
		public void Start() {
			state = State.MENU;
			machine = new StateMachine<State>(state);
			
			currentLevel = JuloFind.byName<Level>("Level");
			currentLevel.load(this);
			
			menu.init(this);
			
			hk.replayManager.display       = hk.replayDisplay;
			hk.replayManager.interval      = options.replayInterval;
			hk.replayManager.maximumLength = options.replayLength;
			
			hk.musicPlayer.init();
			
			if(options.musicInitiallyOn) {
				hk.musicPlayer.play();
			} else {
				hk.musicControl.setOn(false);
			}
			hk.soundsControl.setOn(options.soundsInitiallyOn);
			
			hk.cam.target = this;
			
			//fakeGame = true;
			//playGame(Mode.ZERO_PLAYERS, TurtleIsland.Medium, 3);
		}
		/*
		private void saveFrame() {
			//hk.replayCamera.Render();
			lastFrame = RTImage(hk.replayCamera);
		}
		*/
		/*
		private IEnumerator saveFrame() {
			yield return new WaitForEndOfFrame();
			
			Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);  
			RenderTexture.active=null;  //`enter code here`
			texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);  
			texture.Apply();  
			
			lastFrame = texture;
			//byte[] pngData = texture.EncodeToPNG();
			//MemoryStream pngStream = new MemoryStream(pngData);  
			
			//File.WriteAllBytes("lavida.png", pngData); 
		}
			*/
		
		public void Update() {
			/*
			if(circularBuffer.isRunning()) {
				StartCoroutine(saveFrame());
			} else {
				lastFrame = null;
			}
			*/
			if(isPlaying()) {
				currentGame.step();
				updateSelector();
				hk.cam.updateCamera();
				/*
				if(circularBuffer.isRunning()) {
					Texture2D frameImage = lastFrame;
					if(frameImage != null) {
						Debug.Log("Saving frame");
						circularBuffer.save(frameImage);
					} else {
						Debug.Log("Ignoring frame");
					}
				}
				*/
				if(!isOver && currentGame.isOver()) {
					isOver = true;
					autoMenuIsPending = true;
					overTime = JuloTime.gameTime();
				} else if(autoMenuIsPending) {
					if(JuloTime.gameTimeSince(overTime) >= options.autoMenuTime) {
						setState(State.MENU);
						autoMenuIsPending = false;
					}
				}
			}
		}
		
		public void play() {
			// TODO right ?
			autoMenuIsPending = false;
			
			if(isPlaying())
				clearGame();
			
			Vector3 pos = hk.cam.transform.position;
			pos.x = 0f;
			hk.cam.transform.position = pos;
			
			Mode mode = menu.currentMode;
			int numCharacters = menu.getNumberOfTurtles();
			int difficulty = menu.getDifficulty();
			
			fakeGame = false;
			playGame(mode, difficulty, numCharacters);
			
			isOver = false;
			setState(State.PLAYING);
		}
		private void setState(State newState) {
			state = newState;
			machine.trigger(newState);
			
			menu.onStateChanged();
		}
		public void continueGame() {
			setState(State.PLAYING);
			//state = State.PLAYING;
			//menu.onStateChanged();
		}
		public void backToMenu() {
			setState(State.MENU);
			//state = State.MENU;
			//menu.onStateChanged();
		}
		public bool isPlaying() {
			return currentGame != null;
		}
		public bool menuIsBlocking() {
			return isPlaying() && !currentGame.isOver() && !fakeGame;
		}
		
		void OnGUI() {
			Event e = Event.current;
			
			if(e.type == EventType.KeyDown) {
				bool used = true;
				State st = state;
				bool confirm = menu.confirming;
				bool editor = Application.isEditor;
				KeyCode cod = e.keyCode; //char ch = e.character;
				
				if(confirm && cod == KeyCode.Escape) {
					menu.confirmNo();
				} else if(!confirm && (cod == KeyCode.P || cod == KeyCode.Escape)) {
					setState(st == State.PLAYING ? (menuIsBlocking() ? State.PAUSED : State.MENU) : State.PLAYING);
					//state = st == State.PLAYING ? (menuIsBlocking() ? State.PAUSED : State.MENU) : State.PLAYING;
					//menu.onStateChanged();
				} else if(cod == KeyCode.Q && e.control) {
					Application.Quit();
				} else if(cod == KeyCode.M) {
					hk.musicControl.setOn(!hk.musicControl.isOn());
				} else if(cod == KeyCode.S) {
					hk.soundsControl.setOn(!hk.soundsControl.isOn());
				} else if(cod == KeyCode.F11) {
					hk.fullscreenToggle.isOn = !hk.fullscreenToggle.isOn;
				//} else if((editor && cod == KeyCode.F2) || (!editor && cod == KeyCode.R && e.control)) {
					// restart
				} else if(cod == KeyCode.N && (e.control || editor) && hk.musicControl.soundSource.isPlaying()) {
					hk.musicControl.soundSource.next();
				} else if(cod >= KeyCode.Alpha1 && cod <= KeyCode.Alpha5 && sendToController()) {
					int number = (int)(cod - KeyCode.Alpha1) + 1;
					currentGame.activeController.setValue(number);
				} else if(cod >= KeyCode.Keypad1 && cod <= KeyCode.Keypad5 && sendToController()) {
					int number = (int)(cod - KeyCode.Keypad1) + 1;
					currentGame.activeController.setValue(number);
				} else if(cod == KeyCode.Tab && sendToController()) {
					currentGame.activeController.nextWeapon();
				} else {
					used = false;
					// ...
				}
				if(used) {
					//Debug.Log("Using");
					e.Use();
				}
			} else if(e.type == EventType.KeyUp) {
				// ...
			} else {
				// ...
			}
		}
		
		private bool sendToController() {
			return isPlaying() && currentGame.activeController != null;
		}
		
		private void playGame(Mode mode, int difficult, int numCharacters) {
			setControl(mode);
			options.difficulty = difficult;
			
			options.numberOfTurtles = numCharacters;
			
			currentGame = currentLevel.newGame(options);
			currentGame.init();
		}
		
		private void clearGame() {
			foreach(TurtleIslandObject obj in currentGame.objects) {
				obj.onDestroy();
				GameObject.Destroy(obj.gameObject);
			}
			
			gameTarget.blur();
			
			currentGame = null;
		}
		
		private void setControl(Mode mode) {
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
		
		private void updateSelector() {
			Vector2 mousePos = Input.mousePosition;
			
			RectTransform selectorRt = (RectTransform)hk.minimapSelector.transform;
			
			float relativeBottom = hk.minimapCamera.rect.yMin;
			float relativeTop    = hk.minimapCamera.rect.yMax;
			float relativeHeight = hk.minimapCamera.rect.height;
			
			float bottom = Screen.height * relativeBottom;
			float top    = Screen.height * relativeTop;
			
			float relativeX = mousePos.x / Screen.width;
			float width  = relativeHeight * Screen.width * hk.mainCamera.orthographicSize / hk.minimapCamera.orthographicSize;
			
			selectorRt.anchorMin = new Vector2(relativeX, relativeBottom);
			selectorRt.anchorMax = new Vector2(relativeX, relativeTop);
			
			selectorRt.offsetMin = new Vector2(-width / 2f, 0f);
			selectorRt.offsetMax = new Vector2(+width / 2f, 0f);
			
			bool mouseInRange = mousePos.y >= bottom && mousePos.y <= top;
			bool click = Input.GetMouseButton(0);
			
			if(state == State.PLAYING && !click && mouseInRange) {
				if(selectorState != SelectorState.HOVER) {
					selectorState = SelectorState.HOVER;
					hk.minimapSelector.trigger("Hover");
				}
			} else if(state == State.PLAYING && click && (mouseInRange || selectorState == SelectorState.ON)) {
				if(selectorState != SelectorState.ON) {
					selectorState = SelectorState.ON;
					hk.minimapSelector.trigger("On");
				}
				float factor = 2f * hk.minimapCamera.orthographicSize / relativeHeight;
				float worldX = factor * (mousePos.x - Screen.width / 2f) / Screen.height;
				targetX = worldX;
			} else {
				if(selectorState != SelectorState.OFF) {
					selectorState = SelectorState.OFF;
					hk.minimapSelector.trigger("Off");
				}
			}
		}
		
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
			return selectorState == SelectorState.ON || gameTarget.isFocused();
		}
		public Vector3 getFocusPosition() {
			return selectorState == SelectorState.ON ? new Vector3(targetX, 0f, 0f) : gameTarget.getFocusPosition();
		}
	}
}
