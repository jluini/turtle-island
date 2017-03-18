
using System;

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using JuloUtil;

public class LifeDisplay : JuloBehaviour {
	public int life;
	public Color teamColor;
	public int damage;
	
	public float factor = 0f;
	public float visibility = 1f;
	
	public float opacity = 0.5f;
	
	private TextDisplay _lifeDisplay;
	private TextDisplay lifeDisplay {
		get {
			if(_lifeDisplay == null)
				_lifeDisplay = JuloFind.byName<TextDisplay>("Life", this);
			return _lifeDisplay;
		}
	}
	private TextDisplay _damageDisplay;
	private TextDisplay damageDisplay {
		get {
			if(_damageDisplay == null)
				_damageDisplay = JuloFind.byName<TextDisplay>("Damage", this);
			return _damageDisplay;
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

	public enum LDState { IDLE, SHOW_DAMAGE, APPLY_DAMAGE };
	public StateMachine<LDState> machine;
	
	public void init(int life, Color teamColor) {
		this.damageDisplay.gameObject.SetActive(false);
		
		setColor(teamColor);
		this.lifeDisplay.showText(life.ToString());
		
		machine = new StateMachine<LDState>(LDState.IDLE);
		
		this.life = life;
		this.damage = 0;
	}
	
	public void setColor(Color color) {
		this.teamColor = color;
		color.a = opacity;
		this.lifeDisplay.imageDisplay.color = color;
	}
	
	public void showDamage(int damage) {
		if(machine.state != LDState.IDLE)
			throw new ApplicationException("Invalid call of showDamage()");
		
		this.damageDisplay.gameObject.SetActive(true);
		machine.trigger(LDState.SHOW_DAMAGE);
		this.anim.SetTrigger("showDamage");
		
		this.damage = damage;
		this.damageDisplay.showText("-" + damage.ToString());
	}
	public void applyDamage() {
		if(machine.state != LDState.SHOW_DAMAGE)
			throw new ApplicationException("Invalid call of showDamage()");
		
		machine.trigger(LDState.APPLY_DAMAGE);
		this.anim.SetTrigger("applyDamage");
		
		life = life - damage;
	}
	public void LateUpdate() {
		if(machine == null) {
			Debug.Log("Rarisimo");
			return;
		}
		if(machine.state == LDState.APPLY_DAMAGE) {
			if(machine.triggerIfEllapsed(LDState.IDLE, 1f)) {
				damageDisplay.gameObject.SetActive(false);
				this.lifeDisplay.showText(life.ToString());
				if(life <= 0) {
					deactivate();
				}
			} else {
				updateTransparency();
			}
			
			int displayedLife = (int)((1f - factor) * (life + damage) + factor * (life));
			this.lifeDisplay.showText(displayedLife.ToString());
		} else if(machine.state == LDState.SHOW_DAMAGE) {
			updateTransparency();
		}
	}
	
	private void updateTransparency() {
		Color c = damageDisplay.textDisplay.color;
		c.a = visibility;
		damageDisplay.textDisplay.color = c;
	}
}
