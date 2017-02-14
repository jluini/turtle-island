
using System;
using System.Collections.Generic;

using UnityEngine;

using JuloUtil;

public interface IFocusTarget {
	bool isFocused();
	Vector3 getFocusPosition();
}

public interface IFocusable {
	void focusObject(Component obj);
	void focusAny<T>(IList<T> objects) where T : Component;
	void focusPosition(Vector3 position);
	void blur();
}

public class FocusTarget : IFocusable, IFocusTarget {
	private enum Type { NONE, OBJECT, POSITION }
	private Type focusType;
	
	private Transform targetObject;
	private Vector3 targetPosition;
	
	
	public FocusTarget() {
		focusType = Type.NONE;
	}
	
	public bool isFocused() {
		return focusType != Type.NONE;
	}
	
	public Vector3 getFocusPosition() {
		if(focusType == Type.OBJECT) {
			if(!targetObject.gameObject.activeSelf)
				blur();
			return targetObject.position;
		} else if(focusType == Type.POSITION) {
			return targetPosition;
		} else {
			throw new ApplicationException("No target");
		}
	}
	
	public void focusObject(Component obj) {
		focusType = Type.OBJECT;
		targetObject = obj.transform;
	}
	
	public void focusAny<T>(IList<T> objects) where T : Component {
		int numCandidates = objects.Count;
		
		if(numCandidates == 0) {
			Debug.Log("Focusing zero objects");
			return;
		}
		bool alreadyFocused = false;
		
		if(focusType == Type.OBJECT) {
			foreach(Component c in objects) {
				if(c.gameObject == targetObject.gameObject) {
					alreadyFocused = true;
					break;
				}
			}
		}
		
		if(!alreadyFocused) {
			focusObject(objects[JuloMath.randomInt(0, numCandidates - 1)]);
		}
	}
	
	public void focusPosition(Vector3 position) {
		focusType = Type.POSITION;
		targetPosition = position;
	}
	
	public void blur() {
		focusType = Type.NONE;
	}
}
