﻿using UnityEngine;

using JuloUtil;
using JuloMenuSystem;
using TurtleIsland;

public class Main : MonoBehaviour {
	InputManager inputManager;
	public Environment environment;
	
	//bool menuIsOpen;
	
	enum State { INIT, MENU, GAME }
	StateMachine<State> machine;
	
	MenuSystem menuSystem;
	
	void Start () {
		machine = new StateMachine<State>(State.INIT);
		
		inputManager = JuloFind.byName<InputManager>("InputManager");
		
		menuSystem = JuloFind.byName<MenuSystem>("MenuContainer");
		menuSystem.start(inputManager);
		
		environment = JuloFind.byName<Environment>("TurtleIslandEnvironment");
		environment.start(inputManager);
		
		// TODO!!!
		VolumeItem musicButton = JuloFind.byName<VolumeItem>("MusicButton", menuSystem);
		musicButton.soundSource = environment.hk.musicPlayer;
		
		VolumeItem soundsButton = JuloFind.byName<VolumeItem>("SoundsButton", menuSystem);
		soundsButton.soundSource = environment.hk.soundsPlayer;
	}
	
	void Update () {
		if(machine.state == State.INIT) {
			if(machine.triggerIfEllapsed(State.MENU, 0.1f)) {
				menuSystem.open();
			}
		} else if(machine.state == State.MENU) {
			menuSystem.update();
			inputManager.inputEnabled = false;
			environment.update();
			inputManager.inputEnabled = true;
		} else if(machine.state == State.GAME) {
			if(inputManager.isDownAny("Start") || inputManager.isDownAny("Back")) {
				pause();
			} else {
				environment.update();
			}			
		} else {
			throw new System.ApplicationException("Unknown state");
		}
	}
	
	public void play() {
		menuSystem.close();
		machine.trigger(State.GAME);
		
		environment.play();
	}
	
	public void pause() {
		//stopTime();
		menuSystem.open();
		machine.trigger(State.MENU);
	}
	
	public void resume() {
		//startTime();
		menuSystem.close();
		machine.trigger(State.GAME);
	}
	
	public void playCpu() {
		menuSystem.close();
		machine.trigger(State.GAME);
		
		environment.playCpu();
	}
	
	public void quitGame() {
		Application.Quit();
		Debug.Log("Game quit");
	}
	
	public void switchFullscreen(bool value) {
		Debug.Log("Fullscreen switch " + (value ? "ON" : "OFF"));
		Screen.fullScreen = value;
	}
	
	public void tryToCloseMenu() {
		if(environment.isPlaying()) {
			resume();
			//return true;
		}
		//return false;
	}
	
	void stopTime() {
		Time.timeScale = 0f; 
	}
	
	void startTime() {
		Time.timeScale = 1f; 
	}
}
