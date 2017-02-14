
using UnityEngine;

using JuloUtil;

namespace TurtleIsland {
	public class TeamDisplay : MonoBehaviour {
		
		private TextDisplay _teamName;
		public TextDisplay teamName {
			get {
				if(_teamName == null)
					_teamName = JuloFind.byName<TextDisplay>("TeamName", this);
				return _teamName;
			}
		}
		
		private TextDisplay _weaponDisplay;
		public TextDisplay weaponDisplay {
			get {
				if(_weaponDisplay == null)
					_weaponDisplay = JuloFind.byName<TextDisplay>("WeaponDisplay", this);
				return _weaponDisplay;
			}
		}
		
		private Animator _anim;
		private Animator anim {
			get {
				if(_anim == null)
					_anim = GetComponent<Animator>();
				return _anim;
			}
		}
		
		public void trigger(string action) {
			anim.SetTrigger(action);
		}
		
		public void setTeamColor(Color c) {
			teamName.setColor(c);
		}
		
		public void setTeamName(string name) {
			teamName.showText(name);
		}
		
		public void setWeapon(Weapon w, int value) {
			weaponDisplay.setImage(w.getImage());
			weaponDisplay.showText(w.getInfo(value));
		}
	}
	
}