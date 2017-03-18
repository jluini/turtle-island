
using System;
using System.Collections.Generic;

using UnityEngine;

using JuloUtil;

namespace TurtleIsland {
	public class Level : MonoBehaviour {
		public int defaultTurtles;
		public int minimumTurtles;
		public int maximumTurtles;
		
		private Transform _characterContainer;
		public Transform characterContainer {
			get {
				if(_characterContainer == null) {
					_characterContainer = JuloFind.byName<Transform>("Characters", this);
				}
				return _characterContainer;
			}
		}
		
		private Transform _waterLeft;
		public Transform waterLeft {
			get {
				if(_waterLeft == null)
					_waterLeft = JuloFind.byName<Transform>("WaterLeft", this);
				return _waterLeft;
			}
		}
		private Transform _waterRight;
		public Transform waterRight {
			get {
				if(_waterRight == null)
					_waterRight = JuloFind.byName<Transform>("WaterRight", this);
				return _waterRight;
			}
		}
		
		private Transform _bottomLeftCorner;
		public Transform bottomLeftCorner {
			get {
				if(_bottomLeftCorner == null) {
					_bottomLeftCorner = JuloFind.byName<Transform>("BottomLeftCorner", this);
				}
				return _bottomLeftCorner;
			}
		}
		
		private Transform _topRightCorner;
		public Transform topRightCorner {
			get {
				if(_topRightCorner == null) {
					_topRightCorner = JuloFind.byName<Transform>("TopRightCorner", this);
				}
				return _topRightCorner;
			}
		}
		
		private Environment env = null;
		
		private bool modelsLoaded = false;
		private Transform[] leftModels;
		private Transform[] rightModels;
		
		public void load(Environment env) {
			this.env = env;
			
			env.options.bottomLeftCorner.position = bottomLeftCorner.position;
			env.options.topRightCorner.position = topRightCorner.position;
			
			loadModels();
		}
		public TurtleIslandGame newGame(/*Options opt*/) {
			if(!modelsLoaded)
				throw new ApplicationException("Unloaded models");
			
			int numTurtles = env.options.numberOfTurtles;
			int numClones = env.options.numberOfClones;
					
			TurtleIslandGame ret = new TurtleIslandGame(env, this, numTurtles * numClones);
			
			for(int k = 0; k < numClones; k++) {
				for(int i = 0; i < numTurtles; i++) {
					Character lChar = getClone(leftModels[i], TurtleIsland.LeftTeamId);
					lChar.display = newLifeDisplay();
					ret.addCharacter(lChar);
					
					Character rChar = getClone(rightModels[i], TurtleIsland.RightTeamId);
					rChar.display = newLifeDisplay();
					ret.addCharacter(rChar);
					
					rChar.flip();
				}
			}
			
			return ret;
		}
		
		LifeDisplay newLifeDisplay() {
			return UnityEngine.Object.Instantiate(env.options.lifeDisplayPrefab);
		}
		
		private Character getClone(Transform model, int team) {
			Character ret = UnityEngine.Object.Instantiate(env.options.characterPrefab);
			ret.teamId = team;
			ret.life = env.options.initialTurtleLife;
			ret.transform.SetParent(characterContainer.transform, false);
			ret.transform.position = model.position;
			
			return ret;
		}
		
		private void loadModels() {
			leftModels  = new Transform[maximumTurtles];
			rightModels = new Transform[maximumTurtles];
			
			for(int i = 0; i < maximumTurtles; i++) {
				leftModels[i]  = getModel(TurtleIsland.LeftTeamId,  TurtleIsland.LeftTeamName,  i);
				rightModels[i] = getModel(TurtleIsland.RightTeamId, TurtleIsland.RightTeamName, i);
			}
			
			modelsLoaded = true;
		}
		
		Transform getModel(int teamId, string teamName, int modelIndex) {
			string name = teamName + (modelIndex + 1).ToString();
			Transform ret = JuloFind.byName<Transform>(name, characterContainer);
			//ret.teamId = teamId;
			return ret;
		}
	}
}