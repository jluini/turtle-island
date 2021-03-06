
using UnityEngine;

using JuloUtil;

namespace JuloMenuSystem {
	public abstract class Item : MonoBehaviour {
		RectTransform clickableArea;
		Animator anim;
		bool selected;
		
		public void start() {
			clickableArea = JuloFind.byName<RectTransform>("ClickableArea", this, false);
			if(clickableArea == null) {
				clickableArea = GetComponent<RectTransform>();
			}
			anim = GetComponent<Animator>();
			selected = false;
			
			onStart();
		}
		
		public abstract bool click(MenuSystem menuSystem); // { onClick.Invoke(); }
		public virtual bool move(MenuSystem menuSystem, bool direction) { return false; }
		public virtual void onStart() { }
		
		public float getPosition() {
			return transform.position.y;
		}
		
		public void select() {
			if(!selected) {
				selected = true;
				trigger("select");
			} else {
				Debug.LogWarning("Already selected");
			}
		}
		
		public void deselect() {
			if(selected) {
				selected = false;
				trigger("deselect");
			} else {
				Debug.LogWarning("Already deselected");
			}
		}
		
		public bool isEnabled() {
			return gameObject.activeSelf;
		}
		
		public virtual bool isClickable(Vector2 position) {
			return Utils.pointIsWithinRect(position, clickableArea);
		}
		
		void trigger(string newState) {
			anim.SetTrigger(newState);
		}
	}
}