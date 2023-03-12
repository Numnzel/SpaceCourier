using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powered : MonoBehaviour {

    [SerializeField] private int energy;
    [SerializeField] private int maxEnergy;
    internal bool disabled = false;

    public int Energy { get => energy; set => energy = disabled ? energy : Mathf.Clamp(value, 0, maxEnergy); }
    public int MaxEnergy { get => maxEnergy; set => maxEnergy = value; }

	// Returns added value
	public int AddEnergy(int value) {

        if (disabled)
            return 0;

        int addition = Mathf.Min(Mathf.Abs(value), maxEnergy-energy);
        energy += addition;

        return addition; 
	}

    // Returns substracted value
    public int RemoveEnergy(int value) {

        if (disabled)
            return 0;

        int substraction = Mathf.Min(Mathf.Abs(value), energy);
        energy -= substraction;

        return substraction;
    }
}
