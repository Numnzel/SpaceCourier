using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powered : MonoBehaviour {

    [SerializeField] private int energy;
    [SerializeField] private int maxEnergy;
    public int Energy { get => energy; set => energy = Mathf.Clamp(value, 0, maxEnergy); }
    public int MaxEnergy { get => maxEnergy; set => maxEnergy = value; }

    public void Add(int value) {

        Energy += value;
	}
}
