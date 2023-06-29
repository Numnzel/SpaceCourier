using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCustom : MonoBehaviour {

    [SerializeField] private bool playButtonSoundClose = false;

    public void PlayButtonSound() {

        if (playButtonSoundClose)
            CanvasManager.instance.PlayButtonSoundClose();
        else
            CanvasManager.instance.PlayButtonSoundOpen();
    }
}