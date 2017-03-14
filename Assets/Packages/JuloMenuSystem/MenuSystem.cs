
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using JuloUtil;
using JuloAudio;

namespace JuloMenuSystem {
	
	public class MenuSystem : MonoBehaviour, Behav {
		public float cursorVelocityFactor = 0.2f;
		
		public Sprite yesSprite;
		public Sprite noSprite;
		
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
				menu.start(inputManager);
			}
			
			navigation = new Stack<MenuEntry>();
		}
		
		public void open() {
			navigation.Clear();
			
			currentMenuIndex = 0;
			currentMenu = menus[currentMenuIndex];
			currentItemIndex = currentMenu.defaultIndex;
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
			
			if(inputManager.isDownAny("Start")) {
				if(back != null) {
					back.Invoke();
				}
			} else if(inputManager.isDownAny("Fire")) {
				fire(currentItem);
			} else if(inputManager.mouseIsDown()) {
				int opt = getMouseItemIndex();
				if(opt >= 0) {
					if(opt != currentItemIndex) {
						Debug.Log("Raro");
						currentItemIndex = opt;
						currentItem = currentMenu.items[opt];
					}
					fire(currentItem);
				}
			} else if(inputManager.isDownAny("Back")) {
				goBack();
			} else if(inputManager.isDownAny("Vertical")) {
				float value = inputManager.getAxis("Vertical", inputManager.who());
				
				if(value != 0f) {
					bool direction = value < 0f;
					
					if(direction && currentItemIndex < currentMenu.numItems - 1) {
						switchToItem(currentItemIndex + 1);
					} else if(!direction && currentItemIndex > 0) {
						switchToItem(currentItemIndex - 1);
					}
				} else {
					Debug.Log("Raro");
				}
			} else if(inputManager.mouseIsMoving()) {
				int opt = getMouseItemIndex();
				if(opt >= 0 && opt != currentItemIndex) {
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
		
		public void openMenu(Menu menu) {
			// TODO remove this ?
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
			
			navigation.Push(new MenuEntry(currentMenuIndex, currentItemIndex));
			switchToMenu(index);
		}
		
		public Sprite getSprite(bool value) {
			return value ? yesSprite : noSprite;
		}
		
		void fire(Item item) {
			if(item.click(this)) {
				soundsPlayer.playClip(menuGoClip);
			}
		}
		
		int getMouseItemIndex() {
			Vector2 mousePos = inputManager.getMousePosition();
			if(isWithin(currentMenu.GetComponent<RectTransform>(), mousePos)) {
				for(int i = 0; i < currentMenu.numItems; i++) {
					if(isWithin(currentMenu.items[i].GetComponent<RectTransform>(), mousePos)) {
						return i;
					}
				}
			}
			return -1;
		}
		
		void switchToItem(int newIndex) {
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
	}
}
