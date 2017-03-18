
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using JuloUtil;
using JuloAudio;

namespace TurtleIsland {
	public class Hooks {
		public Camera mainCamera;
		public Camera minimapCamera;
		public ReplayManager replayManager;
		public Image replayDisplay;
		public SmartCamera cam;
		
		//public Transform menuContainer;
		public OcultableDisplay displayContainer;
		
		public SoundSource musicPlayer;
		public SoundSource soundsPlayer;
		
		//public VolumeControl musicControl;
		//public VolumeControl soundsControl;
		//public Toggle fullscreenToggle;
		
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
			replayManager = JuloFind.byName<ReplayManager>("ReplayManager");
			replayDisplay = JuloFind.byName<Image>("ReplayDisplay");
			cam = mainCamera.GetComponent<SmartCamera>();
			
			displayContainer = JuloFind.byName<OcultableDisplay>("ControlsDisplay", env);
			
			musicPlayer = JuloFind.byName<SoundSource>("MusicPlayer");
			soundsPlayer = JuloFind.byName<SoundSource>("SoundsPlayer");
			
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