
namespace JuloUtil {
	
	public class StateMachine<T> {
		public T state;
		float lastUpdateTime;
		bool useApplicationTime;
		
		public StateMachine(T defaultState, bool useApplicationTime = false) {
			this.useApplicationTime = useApplicationTime;
			this.state = defaultState;
			this.lastUpdateTime = getTime();
		}
		
		public T current() {
			return state;
		}
		
		public void trigger(T newState) {
			//Debug.Log("Switching to " + newState);
			this.lastUpdateTime = getTime();
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
			return getTimeSince(lastUpdateTime);
		}
		
		public bool isOver(float timeover) {
			return ellapsed() >= timeover;
		}
		
		float getTime() {
			return useApplicationTime ? JuloTime.applicationTime() : JuloTime.gameTime();
		}
		float getTimeSince(float when) {
			return useApplicationTime ? JuloTime.applicationTimeSince(when) : JuloTime.gameTimeSince(when);
		}
	}
	
}