using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwitchButton : MonoBehaviour {

	public bool value;
	[SerializeField] private TextMeshProUGUI text;

	public void SwitchValue() {

		SetValue(!value);
	}

	public void SetValue(bool value) {

		this.value = value;

		text.text = this.value ? "On" : "Off";
	}
}
