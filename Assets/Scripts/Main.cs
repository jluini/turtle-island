﻿using UnityEngine;
﻿using UnityEngine.UI;

using JuloUtil;
using JuloMenuSystem;
using TurtleIsland;

public class Main : MonoBehaviour {
	[HideInInspector]
	public Environment environment;
	
	InputManager inputManager;
	
	enum State { INIT, MENU, GAME }
	StateMachine<State> machine;
	
	MenuSystem menuSystem;
	
	VolumeItem musicButton;
	VolumeItem soundsButton;
	GameObject continueButton;
	//SwitchItem fullscreenButton;
	
	bool waitingToOpenMenu = false;
	float gameOverTimestamp;
	
	Animator title;
	Text authors;
	
	void Start () {
		machine = new StateMachine<State>(State.INIT);
		
		inputManager = JuloFind.byName<InputManager>("InputManager");
		
		environment = JuloFind.byName<Environment>("TurtleIslandEnvironment");
		environment.start(inputManager);
		
		menuSystem = JuloFind.byName<MenuSystem>("MenuContainer");
		menuSystem.gameObject.SetActive(true);
		
		// TODO!!!
		musicButton = JuloFind.byName<VolumeItem>("MusicButton", menuSystem);
		musicButton.soundSource = environment.hk.musicPlayer;
		
		soundsButton = JuloFind.byName<VolumeItem>("SoundsButton", menuSystem);
		soundsButton.soundSource = environment.hk.soundsPlayer;
		
		continueButton = JuloFind.byName("ContinueButton", menuSystem);
		
		//fullscreenButton = JuloFind.byName<SwitchItem>("FullscreenButton", menuSystem);
		
		menuSystem.start(inputManager);
		
		title = JuloFind.byName<Animator>("Title");
		authors = JuloFind.byName<Text>("Authors");
	}
	
	void Update () {
		if(inputManager.isDownKey("m")) {
			musicButton.click(menuSystem);
		}
		if(inputManager.isDownKey("s")) {
			soundsButton.click(menuSystem);
		}
		//if(inputManager.isDownKey("f11")) {
		//	fullscreenButton.click(menuSystem);
		//}
		if(inputManager.isDownKey("f3")) {
			nextLanguage();
		}
		bool control = inputManager.isKey("left ctrl") || inputManager.isKey("right ctrl");
		bool editor = Application.isEditor;
		if(inputManager.isDownKey("n") && (control || editor) && environment.hk.musicPlayer.isPlaying()) {
			environment.hk.musicPlayer.next();
		}
		if(machine.state == State.INIT) {
			if(machine.triggerIfEllapsed(State.MENU, 0.1f)) {
				menuSystem.open();
			}
		} else if(machine.state == State.MENU) {
			menuSystem.update();
			// TODO improve this!
			inputManager.inputEnabled = false;
			environment.update();
			inputManager.inputEnabled = true;
		} else if(machine.state == State.GAME) {
			if(inputManager.isDownAny("Start") || inputManager.isDownAny("Back")) {
				pause();
			} else if(!environment.isPlaying()) {
				if(waitingToOpenMenu) {
					float overEllapsed = JuloTime.applicationTimeSince(gameOverTimestamp);
					if(overEllapsed >= environment.options.autoMenuTime) {
						pause();
					}
				} else {
					waitingToOpenMenu = true;
					gameOverTimestamp = JuloTime.applicationTime();
				}
			} else {
				environment.update();
			}
		} else {
			throw new System.ApplicationException("Unknown state");
		}
	}
	
	void beforePlay() {
		title.SetTrigger("quiet");
		authors.color = new Color(0.631f, 0.616f, 0.533f, 0.110f);
		
		waitingToOpenMenu = false;
		resume();
	}
	public void play() {
		beforePlay();
		environment.play();
	}
	
	public void playCpuEasy()    { playCpu(TurtleIsland.TurtleIsland.Easy);    }
	public void playCpuMedium()  { playCpu(TurtleIsland.TurtleIsland.Medium);  }
	public void playCpuHard()    { playCpu(TurtleIsland.TurtleIsland.Hard);    }
	public void playCpuMaximum() { playCpu(TurtleIsland.TurtleIsland.Maximum); }
	
	public void playCpu(int difficulty) {
		beforePlay();
		environment.playCpu(difficulty);
	}
	
	public void pause() {
		waitingToOpenMenu = false;
		
		if(machine.state != State.GAME) {
			Debug.Log("Invalid call of pause()");
			return;
		}
		
		int index;
		
		if(environment.isPlaying()) {
			JuloTime.stopGame();
			
			index = 0;
			continueButton.SetActive(true);
		} else {
			index = 1;
			continueButton.SetActive(false);
		}
		menuSystem.open("MainMenu", index);
		//menuSystem.open("PauseMenu");
		machine.trigger(State.MENU);
	}
	
	public void resume() {
		if(machine.state != State.MENU) {
			Debug.LogWarning("Invalid call of resume()");
			return;
		}
			
		if(!JuloTime.gameIsRunning())
			JuloTime.resumeGame();
		
		menuSystem.close();
		machine.trigger(State.GAME);
	}
	
	public void quitGame() {
		Application.Quit();
		Debug.Log("Game quit");
	}
	
	public void switchFullscreen(bool value) {
		//Debug.Log("Fullscreen switch " + (value ? "ON" : "OFF"));
		Screen.fullScreen = value;
	}
	
	public void tryToCloseMenu() {
		if(environment.isPlaying()) {
			resume();
		}
	}
	
	public void setSpanish() { setLang(Language.Spanish); }
	public void setEnglish() { setLang(Language.English); }
	
	void setLang(Language newLanguage) {
		//this.language = newLanguage;
		Internationalization.setLanguage(newLanguage);
	}
	
	void nextLanguage() {
		Language language = Internationalization.currentLanguage;
		if(language == Language.English) {
			setSpanish();
		} else {
			setEnglish();
		}
	}
}
