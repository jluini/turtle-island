
using System;
using System.Collections.Generic;

using UnityEngine;

using JuloUtil;

namespace TurtleIsland {
	public class Level : MonoBehaviour {
		public int defaultTurtles;
		public int minimumTurtles;
		public int maximumTurtles;
		
		private Weapon[] _weapons;
		public Weapon[] weapons {
			get {
				if(!modelsLoaded)
					throw new ApplicationException("Unloaded models");
				return _weapons;
			}
		}
		
		private Transform _characterContainer;
		public Transform characterContainer {
			get {
				if(_characterContainer == null) {
					_characterContainer = JuloFind.byName<Transform>("Characters", this);
				}
				return _characterContainer;
			}
		}
		
		private Transform _weaponContainer;
		public Transform weaponContainer {
			get {
				if(_weaponContainer == null) {
					_weaponContainer = JuloFind.byName<Transform>("Weapons", this);
				}
				return _weaponContainer;
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
		private Character[] leftModels;
		private Character[] rightModels;
		
		public void load(Environment env) {
			this.env = env;
			
			env.options.bottomLeftCorner.position = bottomLeftCorner.position;
			env.options.topRightCorner.position = topRightCorner.position;
			
			loadModels();
		}
		public TurtleIslandGame newGame(Options opt) {
			if(!modelsLoaded)
				throw new ApplicationException("Unloaded models");
			
			int numTurtles = opt.numberOfTurtles;
			int numClones = opt.numberOfClones;
					
			TurtleIslandGame ret = new TurtleIslandGame(env, this, numTurtles * numClones);
			
			for(int k = 0; k < numClones; k++) {
				for(int i = 0; i < numTurtles; i++) {
					Character lChar = getClone(leftModels[i]);
					ret.addCharacter(lChar);
					Character rChar = getClone(rightModels[i]);
					ret.addCharacter(rChar);
					rChar.flip();
				}
			}
			
			return ret;
		}
		public Weapon addNewWeapon(
				TurtleIslandGame game,
				int weaponIndex,
				int weaponValue,
				Vector2 position,
				Vector2 direction,
				float shotTime
		) {
			Weapon model = weapons[weaponIndex];
			Weapon w = UnityEngine.Object.Instantiate(model);
			
			w.transform.SetParent(weaponContainer, false);
			
			game.addObject(w);
			
			w.activate();
			w.init(game);
			
			Vector3 position3D = new Vector3(position.x, position.y, model.transform.position.z);
			w.go(weaponValue, position3D, direction, shotTime);
			
			return w;
		}
		private Character getClone(Character model) {
			Character ret = UnityEngine.Object.Instantiate(model);
			ret.transform.SetParent(characterContainer.transform, false);
			
			return ret;
		}
		
		private void loadModels() {
			leftModels  = new Character[maximumTurtles];
			rightModels = new Character[maximumTurtles];
			
			for(int i = 0; i < maximumTurtles; i++) {
				leftModels[i]  = getModel(TurtleIsland.LeftTeamId,  TurtleIsland.LeftTeamName,  i);
				rightModels[i] = getModel(TurtleIsland.RightTeamId, TurtleIsland.RightTeamName, i);
			}
			
			List<Weapon> weapons = new List<Weapon>();
			foreach(Transform child in weaponContainer) {
				Weapon w = child.GetComponent<Weapon>();
				if(child.gameObject.activeSelf && w != null) {
					weapons.Add(w);
					w.deactivate();
				}
			}
			
			_weapons = weapons.ToArray();
			
			modelsLoaded = true;
		}
		
		private Character getModel(int teamId, string teamName, int modelIndex) {
			string name = teamName + (modelIndex + 1).ToString();
			Character ret = JuloFind.byName<Character>(name, characterContainer);
			ret.teamId = teamId;
			ret.life = env.options.initialTurtleLife;
			ret.deactivate();
			return ret;
		}
	}
}