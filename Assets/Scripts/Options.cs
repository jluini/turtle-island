
using UnityEngine;

using JuloUtil;
using JuloGame;

namespace TurtleIsland {
	
	public class Options : GameOptions {
		public LifeDisplay lifeDisplayPrefab;
		public Character characterPrefab;
		
		[Header("Game options")]
		public int numberOfTurtles = 3;
		public int numberOfClones = 1;
		
		public int initialTurtleLife = 200;
		public int initialWeaponValue = 3;
		
		public bool musicInitiallyOn = true;
		public bool soundsInitiallyOn = true;
		
		[Header("Replay")]
		public bool showReplay = false;
		public int replayMaxSize = 100;
		public float replayDelay = 5f;
		public float replayMinimumInterval = 0f;
		
		[Header("Colors")]
		public Color leftColor;
		public Color rightColor;
		public Color deadColor;
		public Color groundColor;
		
		public Color selectedColor;
		
		[Header("Tuning")]
		public float walkSpeed = 0.45f;
		public float targetSpeed = 70f;
		public float weaponSpeedFactor = 15f;
		public float grenadeTorque = 0.3f;
		
		public float minTargetAngle =  0f;
		public float maxTargetAngle = 90f;
		
		[Header("Timing")]
		[Tooltip("Time to wait between turns")]
		public float waitTime = 1f;
		[Tooltip("Time to wait while applying damage")]
		public float applyDamageTime = 1f;
		[Tooltip("Time to wait on game over to auto-open the menu")]
		public float autoMenuTime = 6f;
		
		[Tooltip("Time to wait while character is preparing to play")]
		public float prepareTime = 1f;
		[Tooltip("Turn time")]
		public float playTime = 15f;
		[Tooltip("Time to play after shot")]
		public float postTime = 2.5f;
		[Tooltip("Maximum charge time")]
		public float maximumChargeTime = 2f;
		
		public float deathTime = 2f;
		public float overTime = 2f;
		public float celebrateTime = 8f;
		
		[Header("Sounds")]
		public AudioClip shot1Clip;
		public AudioClip shot2Clip;
		public float shotClipThreshold = 0.3f;
		public AudioClip explosionClip;
		public AudioClip winClip;
		public AudioClip drawClip;
		public AudioClip sinkClip;
		
		[HideInInspector]
		public Controller leftController = null;
		[HideInInspector]
		public Controller rightController = null;
		[HideInInspector]
		public int difficulty;
	}
	
}