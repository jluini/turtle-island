
using UnityEngine;
using System.Collections;

public class BackgroundLayer : MonoBehaviour {
	
	public float d = 10f;
	private float x;
	private float z;
	
	
	public void Start () {
		Vector3 pos = transform.position;
		x = pos.x;
		z = pos.z;
	}
	
	public void LateUpdate () {
		Vector3 pos = transform.position;
		float desp = Camera.main.transform.position.x;
		
		pos.x = x + desp * (1 - d / (d + z));
		
		transform.position = pos;
	}
	
}
