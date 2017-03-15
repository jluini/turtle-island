
using UnityEngine;

namespace TurtleIsland {
	public class UserController : Controller {
		
		private TurtleIslandGame game;
		//private Character character;
		
		private TTPlayStatus status;
		
		public override void initialize(TurtleIslandGame game, int difficulty) {
			this.game = game;
		}
		
		public override void play(Character c) {
			//character = c;
			status = TTPlayStatus.PREPARE;
		}
		
		public override void dischargeForced() {
			status = TTPlayStatus.DONE;
		}
		
		public override void step() {
			float hAxis = game.env.inputManager.getAxis("Horizontal");
			float vAxis = game.env.inputManager.getAxis("Vertical");
			bool fireDown = game.env.inputManager.isDownAny("Fire");
			float fireValue = game.env.inputManager.getAxis("Fire");
			
			bool passDown = game.env.inputManager.isDownAny("Pass");
			
			if(status == TTPlayStatus.PREPARE && fireDown) {
				game.charge();
				status = TTPlayStatus.CHARGE;
			} else if(status == TTPlayStatus.PREPARE && passDown) {
				game.passTurn();
				status = TTPlayStatus.DONE;
			} else if(status == TTPlayStatus.CHARGE && fireValue == 0f) {
				game.discharge();
				status = TTPlayStatus.DONE;
			} else {
				if(status != TTPlayStatus.CHARGE && hAxis != 0f) {
					game.walk(hAxis);
				}
				if(status != TTPlayStatus.DONE && vAxis != 0f) {
					game.moveTarget(vAxis);
				}
			}
		}
		
		public override void setValue(int number) {
			game.setWeaponValue(number);
		}
		public override void nextWeapon() {
			game.nextWeapon();
		}
		public override void incrementValue() {
			game.incrementWeaponValue();
		}
		public override void decrementValue() {
			game.decrementWeaponValue();
		}
		
	}
}
