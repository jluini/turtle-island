
using UnityEngine;

public class SmartCamera : MonoBehaviour {
	public bool automaticMode = false;
	public float cameraSpeed      = 2.5f;
	
	public Vector3 focusSpeedFactor = new Vector3(3f, 0f, 0f);
	public Vector3 offset = new Vector3();
	
	public bool fixX = false;
	public bool fixY = false;
	public bool fixZ = false;
	
	public IFocusTarget target = null;
	
	public void LateUpdate() {
		if(automaticMode) {
			updateCamera();
		}
	}
	
	public void updateCamera() {
		if(target != null) {
			if(target.isFocused())
				goTo(target.getFocusPosition());
		} else {
			Debug.Log("Warning: updating with no target");
		}
	}
	
	private void goTo(Vector3 targetPosition) {
		Vector3 realTargetPosition = transform.position;
		
		if(!fixX) realTargetPosition.x = targetPosition.x + offset.x;
		if(!fixY) realTargetPosition.y = targetPosition.y + offset.y;
		if(!fixZ) realTargetPosition.z = targetPosition.z + offset.z;
		
		Vector3 currentPosition = transform.position;
		Vector3 velocity = realTargetPosition - currentPosition;
		
		velocity.x *= focusSpeedFactor.x;
		velocity.y *= focusSpeedFactor.y;
		velocity.z *= focusSpeedFactor.z;
		
		transform.position = currentPosition + Time.deltaTime * velocity;
	}
}
