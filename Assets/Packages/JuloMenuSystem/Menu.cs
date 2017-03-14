
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using JuloUtil;

namespace JuloMenuSystem {
	public class Menu : MonoBehaviour, Behav {
		public int defaultIndex = 0;
		
		public List<Item> items;
		public int numItems;
		//int currentIndex;
		//int prevIndex = -1;
		
		public void start(InputManager inputManager) {
			//this.inputManager = inputManager;
			
			this.items = JuloFind.allDescendants<Item>(this);
			this.numItems = this.items.Count;
			
			for(int i = 0; i < numItems; i++) {
				Item item = this.items[i];
				item.start();
			}
			
			// TODO right?
			this.hide();
		}
		
		public void update() {}
		
		public void show() {
			Vector3 pos = transform.localPosition;
			transform.localPosition = new Vector3(pos.x, 0, pos.z);
		}
		public void hide() {
			Vector3 pos = transform.localPosition;
			transform.localPosition = new Vector3(pos.x, 1000, pos.z);
		}
		/*public void setVisibility(bool visibility) {
			gameObject.SetActive(visibility);
		}*/
		/*
		public void update() {
			float currentPos = getCursorPos();
			float targetPos  = getTargetPos();
			
			float newPos = currentPos + (targetPos - currentPos) * cursorVelocityFactor;
			setCursorPos(newPos);
			
			if(inputManager.isDownAny("Fire")) {
				fire(currentIndex);
				//currentOption().click();
			} else if(inputManager.mouseIsDown()) {
				int opt = getMouseOptionIndex();
				if(opt >= 0) {
					if(opt != currentIndex) {
						Debug.Log("Raro");
					}
					fire(opt);
					//options[opt].click();
				}
			} else if(inputManager.isDownAny("Back")) {
				if(back != null) {
					back.Invoke();
					if(menuBackClip != null)
						soundsPlayer.playClip(menuBackClip);
				}
			} else if(inputManager.isDownAny("Vertical")) {
				float value = inputManager.getAxis("Vertical", inputManager.who());
				
				if(value != 0f) {
					bool direction = value < 0f;
					
					if(direction && currentIndex < numOptions - 1) {
						switchTo(currentIndex + 1);
					} else if(!direction && currentIndex > 0) {
						switchTo(currentIndex - 1);
					}
				} else {
					Debug.Log("Raro");
				}
			} else if(inputManager.mouseIsMoving()) {
				int opt = getMouseOptionIndex();
				if(opt >= 0 && opt != currentIndex) {
					switchTo(opt);
				}
			}
		}
		
		void fire(int opt) {
			if(menuGoClip != null)
				soundsPlayer.playClip(menuGoClip);
			options[opt].click();
		}
		
		JuloOption currentOption() {
			return options[currentIndex];
		}
		
		int getMouseOptionIndex() {
			Vector2 mousePos = inputManager.getMousePosition();
			if(isWithin(GetComponent<RectTransform>(), mousePos)) {
				for(int i = 0; i < numOptions; i++) {
					if(isWithin(options[i].GetComponent<RectTransform>(), mousePos)) {
						return i;
					}
				}
			}
			return -1;
		}
		
		void switchTo(int newIndex) {
			prevIndex = currentIndex;
			currentIndex = newIndex;
			
			options[prevIndex].trigger("deselect");
			options[newIndex].trigger("select");
			
			if(menuMoveClip != null)
				soundsPlayer.playClip(menuMoveClip);
		}
		
		void setCursorPos(int index) {
			setCursorPos(options[index].transform.position.y);
		}
		/*
		void setCursorPos(int prev, int next, float phase) {
			float prevPos = options[prev].transform.position.y;
			float nextPos = options[next].transform.position.y;
			
			float newPos = prevPos * (1 - phase) + nextPos * phase;
			setPos(newPos);
		}
		* /
		void setCursorPos(float newPos) {
			Vector3 pos = cursor.transform.position;
			cursor.transform.position = new Vector3(pos.x, newPos, pos.z);
		}
		float getCursorPos() {
			return cursor.transform.position.y;
		}
		float getTargetPos() {
			return options[currentIndex].transform.position.y;
		}
		
		bool isWithin(RectTransform rect, Vector2 point) {
			float left = rect.position.x + rect.rect.xMin;
			float rite = rect.position.x + rect.rect.xMax;
			float bott = rect.position.y + rect.rect.yMin;
			float topp = rect.position.y + rect.rect.yMax;
			
			bool xIn = point.x >= left && point.x <= rite;
			bool yIn = point.y >= bott && point.y <= topp;
			
			bool pointIn = xIn&&yIn;
			
			return pointIn;
		}
		*/
	}
}
