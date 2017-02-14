
//using UnityEngine;

namespace JuloUtil {
	
	public class StateMachine<T> {
		public T state;
		
		float lastUpdateTime;
		
		public StateMachine(T defaultState) {
			this.state = defaultState;
			this.lastUpdateTime = JuloTime.gameTime();
		}
		
		public T current() {
			return state;
		}
		
		public void trigger(T newState) {
			//Debug.Log("Switching to " + newState);
			this.lastUpdateTime = JuloTime.gameTime();
			state = newState;
		}
		
		public bool triggerIfEllapsed(T state, float time) {
			if(isOver(time)) {
				trigger(state);
				return true;
			} else {
				return false;
			}
		}
		
		public float ellapsed() {
			return JuloTime.gameTimeSince(this.lastUpdateTime);
		}
		
		public bool isOver(float timeover) {
			return ellapsed() >= timeover;
		}
	}
	
}