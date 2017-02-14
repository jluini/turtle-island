
using System.Collections.Generic;

using UnityEngine;

namespace TurtleIsland {
	public class Team {
		public int id;
		public string name;
		public Color color;
		public int weaponIndex = 0;
		public int weaponValue;
		public Controller controller;
		
		private List<Character> characters;
		
		public Team(int id, string name, Color color, Controller controller) {
			this.id = id;
			this.name = name;
			this.color = color;
			this.controller = controller;
			
			this.characters = new List<Character>();
		}
		
		public IEnumerator<Character> GetEnumerator() {
			return characters.GetEnumerator();
		}
		
		public void addCharacter(Character c) {
			characters.Add(c);
		}
		public void removeCharacter(Character c) {
			characters.Remove(c);
		}
		public bool isReady() {
			foreach(Character c in characters) {
				if(c.isReady()) {
					return true;
				}
			}
			return false;
		}
		public void setWeapon(int index, int value) {
			this.weaponIndex = index;
			this.weaponValue = value;
		}
	}
}