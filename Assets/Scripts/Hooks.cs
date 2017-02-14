
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using JuloUtil;
using JuloAudio;

namespace TurtleIsland {
	public class Hooks {
		public Camera mainCamera;
		public Camera minimapCamera;
		public SmartCamera cam;
		
		public Transform menuContainer;
		public OcultableDisplay displayContainer;
		
		public SoundSource musicPlayer;
		public SoundSource soundsPlayer;
		
		public VolumeControl musicControl;
		public VolumeControl soundsControl;
		public Toggle fullscreenToggle;
		
		public Dictionary<int, TeamDisplay> teamDisplays;
		public TextDisplay mainTimeDisplay;
		
		public Controller userController;
		public Controller leftRobotController;
		public Controller rightRobotController;
		
		//public Transform minimapDisplay;
		public TextDisplay minimapSelector;
		
		public Hooks(Environment env) {
			mainCamera = JuloFind.byName<Camera>("MainCamera");
			minimapCamera = JuloFind.byName<Camera>("MinimapCamera");
			cam = mainCamera.GetComponent<SmartCamera>();
			
			menuContainer = JuloFind.byName<Transform>("Menu", env);
			displayContainer = JuloFind.byName<OcultableDisplay>("Display", env);
			
			musicPlayer = JuloFind.byName<SoundSource>("MusicPlayer", env);
			musicControl = JuloFind.byName<VolumeControl>("MusicControl", env);
			musicControl.soundSource = musicPlayer;
			
			soundsPlayer = JuloFind.byName<SoundSource>("SoundsPlayer", env);
			soundsControl = JuloFind.byName<VolumeControl>("SoundsControl", env);
			soundsControl.soundSource = soundsPlayer;
			
			fullscreenToggle = JuloFind.byName<Toggle>("FullscreenToggle", menuContainer);
			
			teamDisplays = new Dictionary<int, TeamDisplay>();
			teamDisplays.Add(TurtleIsland.LeftTeamId,  JuloFind.byName<TeamDisplay>("LeftTeamDisplay",  displayContainer));
			teamDisplays.Add(TurtleIsland.RightTeamId, JuloFind.byName<TeamDisplay>("RightTeamDisplay", displayContainer));			
			
			mainTimeDisplay = JuloFind.byName<TextDisplay>("MainTimeDisplay", displayContainer);
			
			userController = JuloFind.byName<Controller>("UserController", env);
			leftRobotController = JuloFind.byName<Controller>("LeftRobotController", env);
			rightRobotController = JuloFind.byName<Controller>("RightRobotController", env);
			
			//minimapDisplay = JuloFind.byName<Transform>("MinimapDisplay", env);
			minimapSelector = JuloFind.byName<TextDisplay>("MinimapSelector", env);
		}
	}
}