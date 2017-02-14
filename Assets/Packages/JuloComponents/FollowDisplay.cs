
using UnityEngine;

public class FollowDisplay : MonoBehaviour {
	// TODO detect automatically ?
	public Transform target = null;
	
	public Vector3 worldOffset = new Vector3();
	public Vector3 screenOffset = new Vector3();
	
	void LateUpdate() {
		if(target != null) {
			Vector3 targetPos = target.position;
			Vector3 pos = targetPos + worldOffset;
			Vector3 newPos = Camera.main.WorldToScreenPoint(pos) + screenOffset;
			transform.position = newPos;
		} else {
			Debug.Log("FollowDisplay following null object");
		}
	}
}
