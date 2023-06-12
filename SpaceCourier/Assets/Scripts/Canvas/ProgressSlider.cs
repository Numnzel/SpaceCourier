using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressSlider : MonoBehaviour {

	public TextMeshProUGUI sliderName;
	[SerializeField] private TextMeshProUGUI sliderValue;
	[SerializeField] private Slider bar;

	public void SetProgress(int currentValue, int maxValue) {

		bar.maxValue = maxValue;
		bar.value = Mathf.Min(currentValue, maxValue);
		sliderValue.text = Mathf.RoundToInt(Mathf.Min(currentValue, maxValue)).ToString() + "/" + maxValue.ToString();
	}
}
