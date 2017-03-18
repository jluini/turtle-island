
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
		
		public void start(InputManager inputManager) {
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
	}
}
