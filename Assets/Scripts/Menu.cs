
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using JuloUtil;

namespace TurtleIsland {
	public class Menu : MonoBehaviour {
		public Sprite buttonOff;
		public Sprite buttonOn;
		
		[HideInInspector]
		public Environment.Mode currentMode;
		
		[HideInInspector]
		public bool confirming = false;
		
		private Environment env;
		
		private EventSystem eventSystem;
		
		private GameObject overlay;
		private GameObject menu;
		
		private SwitchContainer menu1;
		private Dictionary<Environment.Mode, Button> modeButtons;
		private GameObject newGameDialog;
		private GameObject difficulty;
		private Dropdown difficultyDropdown;
		private Dropdown numberOfTurtlesDropdown;
		private Button playButton;
		
		private Button backButton;
		private Button continueButton;
		private Button quitButton;
		
		private Toggle fullscreenToggle;
		
		private GameObject quitConfirm;
		private Button confirmYesButton;
		private Button confirmNoButton;
		
		private GameObject selectedObject = null;
		
		private static Environment.State MENU    = Environment.State.MENU;
		private static Environment.State PLAYING = Environment.State.PLAYING;
		private static Environment.State PAUSED  = Environment.State.PAUSED;
		
		private static Environment.Mode NONE         = Environment.Mode.NONE;
		private static Environment.Mode TWO_PLAYERS  = Environment.Mode.TWO_PLAYERS;
		private static Environment.Mode ONE_PLAYER   = Environment.Mode.ONE_PLAYER;
		private static Environment.Mode ZERO_PLAYERS = Environment.Mode.ZERO_PLAYERS;
		
		public void init(Environment env) {
			this.env = env;
			this.currentMode = NONE;
			
			this.load();
			
			// LISTENERS
			/*
			foreach(Button modeButton in modeButtons.Values) {
				modeButton.onSelect.AddListener(this.onModeButtonSelect);
				modeButton.onDeselect.AddListener(this.onModeButtonDeselect);
			}
			*/
			modeButtons[TWO_PLAYERS].onClick.AddListener(this.onTwoPlayerClick);
			modeButtons[ONE_PLAYER].onClick.AddListener(this.onOnePlayerClick);
			modeButtons[ZERO_PLAYERS].onClick.AddListener(this.onZeroPlayerClick);
			
			playButton.onClick.AddListener(env.play);
			backButton.onClick.AddListener(env.backToMenu);
			continueButton.onClick.AddListener(env.continueGame);
			quitButton.onClick.AddListener(confirm);
			
			fullscreenToggle.onValueChanged.AddListener(this.toggleFullscreen);
			
			confirmYesButton.onClick.AddListener(this.confirmYes);
			confirmNoButton.onClick.AddListener(this.confirmNo);
			
			newGameDialog.SetActive(false);
			
			onStateChanged();
		}
		public void onTwoPlayerClick() {
			if(currentMode != TWO_PLAYERS) {
				setMode(TWO_PLAYERS);
			} else {
				setMode(NONE);
			}
		}
		public void onOnePlayerClick() {
			if(currentMode != ONE_PLAYER) {
				setMode(ONE_PLAYER);
			} else {
				setMode(NONE);
			}
		}
		public void onZeroPlayerClick() {
			if(currentMode != ZERO_PLAYERS) {
				setMode(ZERO_PLAYERS);
			} else {
				setMode(NONE);
			}
		}
		/*
		private void onModeButtonSelect() {
			Debug.Log("Selecting  mode button: " + getSelected());
		}
		
		private void onModeButtonDeselect() {
			Debug.Log("Deselecting  mode button");
		}
		*/
		private void setMode(Environment.Mode mode) {
			if(currentMode != NONE) {
				setButtonImage(modeButtons[currentMode], buttonOff);
			}
			
			currentMode = mode;
			
			if(mode == NONE) {
				newGameDialog.SetActive(false);
			} else {
				setButtonImage(modeButtons[mode], buttonOn);
				newGameDialog.SetActive(true);
				difficulty.SetActive(mode != TWO_PLAYERS);
				select(playButton);
			}
		}
		
		private void setButtonImage(Button but, Sprite img) {
			but.GetComponent<Image>().sprite = img;
		}
		private void load() {
			eventSystem = JuloFind.byName<EventSystem>("EventSystem");
			
			overlay = JuloFind.byName("MenuOverlay", this);
			menu = JuloFind.byName("Menu", this);
			
			menu1 = JuloFind.byName<SwitchContainer>("Menu1", this);
			menu1.setIndex(0);
			modeButtons = new Dictionary<Environment.Mode, Button>();
			modeButtons.Add(TWO_PLAYERS,  JuloFind.byName<Button>("TwoPlayerButton", this));
			modeButtons.Add(ONE_PLAYER,   JuloFind.byName<Button>("OnePlayerButton", this));
			modeButtons.Add(ZERO_PLAYERS, JuloFind.byName<Button>("ZeroPlayerButton", this));
			
			newGameDialog = JuloFind.byName("NewGameDialog", this);
			difficulty = JuloFind.byName("Difficulty", this);
			difficultyDropdown = JuloFind.byName<Dropdown>("Dropdown", difficulty.transform);
			numberOfTurtlesDropdown = JuloFind.byName<Dropdown>("NumberOfTurtles", this);
			
			List<string> notOptions = new List<string>();
			for(int t = env.currentLevel.minimumTurtles; t <= env.currentLevel.maximumTurtles; t++) {
				notOptions.Add("" + t + " tortuga" + (t == 1 ? "" : "s") + "");
			}
			numberOfTurtlesDropdown.ClearOptions();
			numberOfTurtlesDropdown.AddOptions(notOptions);
			numberOfTurtlesDropdown.value = env.currentLevel.defaultTurtles;
			
			playButton = JuloFind.byName<Button>("PlayButton", this);
			
			backButton     = JuloFind.byName<Button>("BackToMenuButton", this);
			continueButton = JuloFind.byName<Button>("ContinueButton", this);
			quitButton     = JuloFind.byName<Button>("QuitButton", this);
			
			fullscreenToggle = JuloFind.byName<Toggle>("FullscreenToggle", this);
			
			quitConfirm = JuloFind.byName("ConfirmContainer", this);
			confirmYesButton = JuloFind.byName<Button>("ConfirmYes", this);
			confirmNoButton = JuloFind.byName<Button>("ConfirmNo", this);
		}
		
		public void Update() {
			if(env.state == MENU) {
				GameObject nowSelected = getSelected();
				if(nowSelected != selectedObject) {
					if(selectedObject != null) {
						setLabelColor(selectedObject, Color.black);
					}
					selectedObject = nowSelected;
					if(selectedObject != null) {
						setLabelColor(selectedObject, env.options.selectedColor);
					}
				}
			}
		}
		
		private void setLabelColor(GameObject obj, Color color) {
			try {
				Text label = JuloFind.byName<Text>("Label", obj.transform);
				label.color = color;
			} catch(Exception) {
				// nothing
			}
		}
		
		public void confirm() {
			confirming = true;
			
			if(env.state == PLAYING) {
				//env.hk.cam.setMouseEnabled(false);
				Time.timeScale = 0f;
				overlay.SetActive(true);
				menu.SetActive(true);
				menu1.setIndex(env.isPlaying() ? 1 : 0);
				
				newGameDialog.SetActive(false);
				
				backButton.gameObject.SetActive(false);
				continueButton.gameObject.SetActive(false);
			}
			quitConfirm.SetActive(true);
			
			select(confirmNoButton);
		}
		public void confirmYes() {
			exitConfirm();
			Application.Quit();
		}
		public void confirmNo() {
			exitConfirm();
		}
		public void toggleFullscreen(bool value) {
			bool fullScreen = fullscreenToggle.isOn;
			Debug.Log("Setting fullscreen to " + fullScreen);
			
			if(fullScreen) {
				//int width = Screen.width;
				//int height = Screen.height;
				
				Screen.fullScreen = true;
				//Screen.SetResolution(width, height, true);
			} else {
				Screen.fullScreen = false;
			}
		}
		
		private void exitConfirm() {
			confirming = false;
			
			if(env.state == PLAYING) {
				//env.hk.cam.setMouseEnabled(true);
				onStateChanged();
			} else {
				quitConfirm.SetActive(false);
			}
		}
		
		public void onStateChanged() {
			Environment.State st = env.state;
			
			quitConfirm.SetActive(false);
			//env.hk.cam.setMouseEnabled(st == PLAYING);
			
			if(st == MENU) {
				Time.timeScale = env.menuIsBlocking() ? 0f : 1f;
				gameObject.SetActive(true);
				menu1.setIndex(0);
				env.hk.displayContainer.hide();
				
				backButton.gameObject.SetActive(false);
				continueButton.gameObject.SetActive(env.isPlaying());
				
				if(currentMode == Environment.Mode.NONE) {
					select(modeButtons[TWO_PLAYERS]);
				} else {
					select(playButton);
				}
			} else if(st == PAUSED) {
				Time.timeScale = 0f;
				gameObject.SetActive(true);
				menu1.setIndex(1);
				env.hk.displayContainer.hide();
				
				backButton.gameObject.SetActive(true);
				continueButton.gameObject.SetActive(true);
				select(continueButton);
			} else if(st == PLAYING) {
				Time.timeScale = 1f;
				gameObject.SetActive(false);
				env.hk.displayContainer.setVisibility(env.isPlaying());
				select(null);
			}
		}
		public int getDifficulty() {
			return difficultyDropdown.value;
		}
		public int getNumberOfTurtles() {
			return numberOfTurtlesDropdown.value + env.currentLevel.minimumTurtles;
		}
		private void select(Component component) {
			GameObject obj = (component == null ? null : component.gameObject);
			eventSystem.SetSelectedGameObject(obj);
		}
		private GameObject getSelected() {
			return eventSystem.currentSelectedGameObject;
		}
	}
}