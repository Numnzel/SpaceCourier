using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Achievement {

    public string id;
    public string name;
    public string description;
    public Sprite icon;
    public Stat requirement;
    public bool unlocked = false;
}