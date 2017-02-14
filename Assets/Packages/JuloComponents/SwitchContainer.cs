
using UnityEngine;

public class SwitchContainer : MonoBehaviour {
	private int index;
	
	public void setIndex(int index) {
		for(int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).gameObject.SetActive(i == index);
		}
	}
	
}
