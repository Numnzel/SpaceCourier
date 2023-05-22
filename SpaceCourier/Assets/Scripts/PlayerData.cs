using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class PlayerData {

    public static int progression;
    public static int progressionExpert;
    public static float optionValue_sound;
    public static float optionValue_music;
    public static float optionValue_mapAlpha;
    public static float optionValue_arrowsAlpha;
    public static float optionValue_uiScale;

    public static void SetDefaults() {

        progression = 0;
        progressionExpert = 0;
        optionValue_sound = 1.0f;
        optionValue_music = 1.0f;
        optionValue_mapAlpha = 1.0f;
        optionValue_arrowsAlpha = 1.0f;
        optionValue_uiScale = 1.0f;
    }
}