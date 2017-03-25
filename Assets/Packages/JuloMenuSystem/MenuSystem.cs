
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using JuloUtil;
using JuloAudio;

namespace JuloMenuSystem {
	
	public class MenuSystem : MonoBehaviour, Behav {
		public float cursorVelocityFactor = 0.2f;
		
		public UnityEvent back;
		
		// TODO remove from here
		SoundSource soundsPlayer;
		public AudioClip menuGoClip;
		public AudioClip menuMoveClip;
		public AudioClip menuBackClip;
		
		InputManager inputManager;
		
		Cursor cursor;
		GameObject overlay;
		
		List<Menu> menus;
		int numMenus;
		int currentMenuIndex;
		Menu currentMenu;
		int currentItemIndex;
		Item currentItem;
		
		class MenuEntry {
			public int menuIndex;
			public int itemIndex;
			
			public MenuEntry(int menuIndex, int itemIndex) {
				this.menuIndex = menuIndex;
				this.itemIndex = itemIndex;
			}
		}
		
		Stack<MenuEntry> navigation;
		
		public void start(InputManager inputManager) {
			this.inputManager = inputManager;
			this.soundsPlayer = JuloFind.byName<SoundSource>("SoundsPlayer");
			
			this.cursor = JuloFind.byName<Cursor>("Cursor", this);
			this.cursor.hide();
			
			this.overlay = JuloFind.byName("Overlay");
			
			this.menus = JuloFind.allDescendants<Menu>(this);
			this.numMenus = this.menus.Count;
			
			for(int m = 0; m < this.numMenus; m++) {
				Menu menu = this.menus[m];
				menu.gameObject.SetActive(true);
				menu.start(inputManager);
			}
			
			navigation = new Stack<MenuEntry>();
		}
		
		public void open() {
			open(0);
		}
		
		public void open(string menuName, int optionIndex = -1) {
			int index = -1;
			for(int m = 0; m < numMenus; m++) {
				Menu thisMenu = menus[m];
				if(thisMenu.name == menuName) {
					if(index >= 0)
						throw new ApplicationException("Found twice");
					index = m;
				}
			}
			if(index < 0)
				throw new ApplicationException("Menu not found");
			
			open(index, optionIndex);
		}
		void open(int itemIndex, int optionIndex = -1) {
			navigation.Clear();
			
			currentMenuIndex = itemIndex;
			currentMenu = menus[currentMenuIndex];
			if(optionIndex < 0)
				optionIndex = currentMenu.defaultIndex;
			currentItemIndex = optionIndex;
			currentItem = currentMenu.items[currentItemIndex];
			
			overlay.SetActive(true);
			
			currentMenu.show();
			currentItem.select();
			cursor.setPosition(currentItem.getPosition());
		}
		
		public void close() {
			overlay.SetActive(false);
			cursor.hide();
			
			currentItem.deselect();
			currentMenu.hide();
		}
		
		public void update() {
			float currentPos = cursor.transform.position.y;
			float targetPos  = currentItem.getPosition();
			
			float newPos = currentPos + (targetPos - currentPos) * cursorVelocityFactor;
			//setCursorPos(newPos);
			cursor.setPosition(newPos);
			
			// Debug.Log("Update: " + Time.frameCount + " --- " + inputManager.getAxis("Vertical"));
			
			if(inputManager.isDownAny("Start")) {
				if(back != null) {
					back.Invoke();
				}
			} else if(inputManager.isDownAny("Fire")) {
				fire(currentItem);
			} else if(inputManager.isDownAny("Horizontal")) {
				float value = inputManager.getAxis("Horizontal", inputManager.who()); 
				if(value == 0f) {
					Debug.Log("Raro");
				} else {
					currentItem.move(this, value > 0f);
				}
			} else if(inputManager.mouseIsDown()) {
				Vector2 mousePos = inputManager.getMousePosition();
				int opt = getItemIndexForMousePosition(mousePos);
				
				if(opt >= 0) {
					if(opt != currentItemIndex) {
						Debug.Log("Raro");
						currentItemIndex = opt;
						currentItem = currentMenu.items[opt];
					}
					if(currentItem.isClickable(mousePos)) {
						fire(currentItem);
					}
				}
			} else if(inputManager.isDownAny("Back")) {
				goBack();
			} else if(inputManager.isDownAny("Vertical")) {
				//Debug.Log("Moving cursor");
				float value = inputManager.getAxis("Vertical", inputManager.who());
				
				if(value != 0f) {
					bool direction = value < 0f;
					
					if(direction) {
						int newIndex = currentItemIndex;
						bool moved = false;
						do {
							newIndex++;
							if(newIndex >= currentMenu.numItems)
								newIndex = 0;
							if(currentMenu.items[newIndex].isEnabled())
								moved = true;
						} while(!moved);
						
						switchToItem(newIndex);
					} else if(!direction) {
						int newIndex = currentItemIndex;
						bool moved = false;
						do {
							newIndex--;
							if(newIndex < 0)
								newIndex = currentMenu.numItems - 1;
							if(currentMenu.items[newIndex].gameObject.activeSelf)
								moved = true;
						} while(!moved);
						
						switchToItem(newIndex);
					}
				} else {
					Debug.Log("Raro");
				}
			} else if(inputManager.isDownKey("home") || inputManager.isDownKey("page up")) {
				switchToFirstItem();
			} else if(inputManager.isDownKey("end") || inputManager.isDownKey("page down")) {
				switchToLastItem();
			} else if(inputManager.mouseIsMoving()) {
				Vector2 mousePos = inputManager.getMousePosition();
				int opt = getItemIndexForMousePosition(mousePos);
				if(opt >= 0) {
					switchToItem(opt);
				}
			}
		}
		
		public void goBack() {
			if(navigation.Count == 0) {
				if(back != null)
					back.Invoke();
			} else {
				soundsPlayer.playClip(menuBackClip);
				
				MenuEntry last = navigation.Pop();
				switchToMenu(last.menuIndex, last.itemIndex);
			}
		}
		
		int getMenuIndex(Menu menu) {
			int index = -1;
			for(int m = 0; m < numMenus; m++) {
				Menu thisMenu = menus[m];
				if(thisMenu == menu) {
					if(index >= 0)
						throw new ApplicationException("Found twice");
					index = m;
				}
			}
			if(index < 0)
				throw new ApplicationException("Menu not found");
			
			return index;
		}
		
		public void openMenu(Menu menu) {
			// TODO remove this ?
			int index = getMenuIndex(menu);
			
			navigation.Push(new MenuEntry(currentMenuIndex, currentItemIndex));
			switchToMenu(index);
		}
		
		void fire(Item item) {
			if(item.click(this)) {
				soundsPlayer.playClip(menuGoClip);
			}
		}
		
		int getItemIndexForMousePosition(Vector2 mousePos) {
			RectTransform menuRect = currentMenu.GetComponent<RectTransform>();
			if(Utils.pointIsWithinRect(mousePos, menuRect)) {
				for(int i = 0; i < currentMenu.numItems; i++) {
					Item item = currentMenu.items[i];
					RectTransform itemRect = item.GetComponent<RectTransform>();
					if(item.isEnabled() && Utils.pointIsWithinRect(mousePos, itemRect)) {
						return i;
					}
				}
			}
			return -1;
		}
		
		void switchToFirstItem() {
			int newIndex = 0;
			bool moved = false;
			do {
				if(currentMenu.items[newIndex].isEnabled())
					moved = true;
				else
					newIndex++;
			} while(!moved);
			
			switchToItem(newIndex);
		}
		
		void switchToLastItem() {
			int newIndex = currentMenu.numItems - 1;
			bool moved = false;
			do {
				if(currentMenu.items[newIndex].isEnabled())
					moved = true;
				else
					newIndex--;
			} while(!moved);
			
			switchToItem(newIndex);
		}
		
		void switchToItem(int newIndex) {
			if(newIndex == currentItemIndex) {
				return;
			}
			
			Item prevItem = currentItem;
			currentItemIndex = newIndex;
			currentItem = currentMenu.items[newIndex];
			
			prevItem.deselect();
			currentItem.select();
			
			soundsPlayer.playClip(menuMoveClip);
		}
		void switchToMenu(int menuIndex, int itemIndex = -1) {
			currentItem.deselect();
			currentMenu.hide();
			currentMenuIndex = menuIndex;
			currentMenu = menus[currentMenuIndex];
			currentMenu.show();
			
			if(itemIndex < 0)
				itemIndex = currentMenu.defaultIndex;
			
			currentItemIndex = itemIndex;
			currentItem = currentMenu.items[currentItemIndex];
			cursor.setPosition(currentItem.getPosition());
			currentItem.select();
		}
		/*
		bool isWithin(RectTransform rect, Vector2 point) {
			float left = rect.position.x + rect.rect.xMin;
			float rite = rect.position.x + rect.rect.xMax;
			float bott = rect.position.y + rect.rect.yMin;
			float topp = rect.position.y + rect.rect.yMax;
			
			bool xIn = point.x >= left && point.x <= rite;
			bool yIn = point.y >= bott && point.y <= topp;
			
			bool pointIn = xIn&&yIn;
			
			return pointIn;
		}*/
	}
}
