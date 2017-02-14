
using UnityEngine;
using UnityEngine.UI;

using JuloUtil;

public class TextDisplay : JuloBehaviour {
	
	private string animatorState = null;
	
	private Text _textDisplay = null;
	public Text textDisplay {
		get {
			if(_textDisplay == null) {
				_textDisplay = JuloFind.byName<Text>("Text", this);
			}
			return _textDisplay;
		}
	}
	
	private Image _imageDisplay = null;
	public Image imageDisplay {
		get {
			if(_imageDisplay == null) {
				_imageDisplay = JuloFind.descendant<Image>(this);
			}
			return _imageDisplay;
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
	
	public void OnEnable() {
		if(animatorState != null)
			anim.SetTrigger(animatorState);
	}
	public void trigger(string action) {
		animatorState = action;
		if(anim.isInitialized) {
			anim.SetTrigger(animatorState);
		}
	}
	
	public void showText(string text) {
		textDisplay.text = text;
	}
	
	public void showFloat(float number, JuloMath.TruncateMethod tm, int decimals) {
		showText(JuloMath.floatToString(number, tm, decimals));
	}
	
	public void setImage(Sprite sprite) {
		imageDisplay.sprite = sprite;
	}
	public void setColor(Color c) {
		textDisplay.color = c;
	}
	
}