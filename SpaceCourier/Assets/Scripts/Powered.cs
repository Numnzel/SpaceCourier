using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powered : MonoBehaviour {

    [SerializeField] private int energy;
    [SerializeField] private int maxEnergy;

    public int Energy { get => energy; set => energy = Mathf.Clamp(value, 0, maxEnergy); }
    public int MaxEnergy { get => maxEnergy; set => maxEnergy = value; }

    // Returns added value
    public int Add(int value) {

        int addition = Mathf.Min(Mathf.Abs(value), maxEnergy-energy);
        energy += addition;

        return addition; 
	}

    // Returns substracted value
    public int Remove(int value) {

        int substraction = Mathf.Min(Mathf.Abs(value), energy);
        energy -= substraction;

        return substraction;
    }
}
