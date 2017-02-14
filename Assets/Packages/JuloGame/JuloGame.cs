
using System.Collections.Generic;

using UnityEngine;

using JuloUtil;

namespace JuloGame {
	
	public interface Steppable {
		void step();
	}
	
	public class GameOptions : MonoBehaviour {
		
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
	}
	
	public abstract class GameObj : JuloBehaviour, Steppable {
		public abstract void step();
		public abstract void onDestroy();
	}
	
	public abstract class Game<ObjType, OptType> : Steppable
			where ObjType : GameObj
			where OptType : GameOptions {
		
		public OptType options;
		public List<ObjType> objects = new List<ObjType>();
		
		private List<ObjType> toAdd = new List<ObjType>();
		//private List<ObjType> toRemove = new List<ObjType>();
		
		public Game(OptType options) {
			this.options = options;
		}
		
		public void init() {
			checkLimits();
			applyChanges();
			
			onInit();
		}
		
		public void step() {
			foreach(ObjType obj in objects) {
				if(obj.isActive()) {
					obj.step();
				} else {
					//toRemove.Add(obj);
				}
			}
			
			checkLimits();
			applyChanges();
			
			onStep();
		}
		
		private void checkLimits() {
			foreach(ObjType obj in objects) {
				if(obj.isActive() && !isInFrame(obj)) {
					// Debug.Log("Deactivating object");
					obj.deactivate();
					onDeactivate(obj);
				}
			}
		}
		private void applyChanges() {
			/*
			foreach(ObjType obj in toRemove) {
				obj.deactivate();
				objects.Remove(obj);
				onDestroy(obj);
				GameObject.Destroy(obj.gameObject);
			}
			*/
			//toRemove.Clear();
			foreach(ObjType obj in toAdd) {
				objects.Add(obj);
				//obj.activate();
			}
			toAdd.Clear();
		}
		protected bool isInFrame(ObjType obj) {
			Vector2 pos = obj.transform.position;
			Vector2 bl = options.bottomLeftCorner.position;
			Vector2 tr = options.topRightCorner.position;
			
			return pos.x >= bl.x && pos.x <= tr.x && pos.y >= bl.y && pos.y <= tr.y;
			//return pos.x > options.xMin && pos.x < options.xMax && pos.y > options.yMin && pos.y < options.yMax;
		}
		
		public abstract void onInit();
		public abstract void onStep();
		public abstract bool isOver();
		//public abstract void onDestroy(ObjType objectDestroyed);
		public abstract void onDeactivate(ObjType objectDeactivated);
		
		public void addObject(ObjType obj) {
			toAdd.Add(obj);
			//objects.Add(obj);
		}
	}
	
	
	
	
	
	
	
	
}

