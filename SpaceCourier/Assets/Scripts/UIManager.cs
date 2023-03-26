using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    [SerializeField] Canvas canvas;
    [SerializeField] Image energyBar;
    [SerializeField] Image freezeBar;
    [SerializeField] Image impulseBar;
    [SerializeField] Image solarBar;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
    }

    private void UpdateBar(Image bar, float value, float max) {

        bar.fillAmount = Mathf.Clamp(value / max, 0.0f, 1.0f);
	}

	public void UpdateEnergyBar(float value, float max) {

        UpdateBar(energyBar, value, max);
    }

    public void UpdateSolarBar(float value, float max) {

        UpdateBar(solarBar, value, max);
    }

    public void UpdateImpulseBar(float value, float max) {

        UpdateBar(impulseBar, value, max);
    }

    public void UpdateFreezeBar(float value, float max) {

        UpdateBar(freezeBar, value, max);
    }
}
