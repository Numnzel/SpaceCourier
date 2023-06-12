using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powered : MonoBehaviour {

    [SerializeField] private int energy;
    [SerializeField] private int maxEnergy;
    [SerializeField] private bool usesOxygen;
    [SerializeField] private int passiveEnergyConsumption;
    internal bool disabled = false;

    private Coroutine passiveConsumptionCR;
    private Coroutine updateAveragesCR;
    private Coroutine oxygenCR;

    private int oxygenAmount;
    private int averageEnergyGain;
    private int averageEnergyLoss;

    const int usedEnergyIndicatorMax = 100;
    const int oxygenMaximum = 500;
    const int oxygenEnergyThreshold = 1000;

	public int Energy { get => energy; }
	public int MaxEnergy { get => maxEnergy; }

	private void Start() {

        oxygenAmount = oxygenMaximum;
        passiveConsumptionCR = StartCoroutine(PassiveConsumption());
        updateAveragesCR = StartCoroutine(UpdateAverageValues());
    }

	public IEnumerator PassiveConsumption() {

        while (energy > 0 && !disabled) {

            yield return new WaitForSeconds(0.01f); // Wait before starting consumption

            RemoveEnergy(passiveEnergyConsumption);

            if (energy > oxygenEnergyThreshold)
                oxygenAmount = Mathf.Min(oxygenMaximum, oxygenAmount+1);
        }

        if (usesOxygen && !disabled)
            oxygenCR = StartCoroutine(LoseOxygen());

        if (passiveConsumptionCR != null)
            StopCoroutine(passiveConsumptionCR);
    }

    public IEnumerator LoseOxygen() {

        while (energy == 0 && !disabled) {

            yield return new WaitForSeconds(0.01f);

            if (--oxygenAmount <= 0)
                disabled = true;
        }

        if (!disabled)
            passiveConsumptionCR = StartCoroutine(PassiveConsumption());

        if (oxygenCR != null)
            StopCoroutine(oxygenCR);
    }

    public IEnumerator UpdateAverageValues() {

        while (!disabled) {

            yield return new WaitForSeconds(0.01f);

            UpdateUI();

            // Reset averages
            averageEnergyGain = 0;
            averageEnergyLoss = 0;
        }

        // Reset averages
        averageEnergyGain = 0;
        averageEnergyLoss = 0;

        UpdateUI();
    }

    private void UpdateUI() {

        UIManager.instance.UpdateEnergyBar(energy, maxEnergy);
        UIManager.instance.UpdateImpulseBar(averageEnergyLoss, usedEnergyIndicatorMax);
        UIManager.instance.UpdateSolarBar(averageEnergyGain, usedEnergyIndicatorMax);
        UIManager.instance.UpdateFreezeBar(oxygenAmount, oxygenMaximum);
    }

    public bool SetEnergy(int value) {

        if (disabled)
            return false;

        energy = Mathf.Clamp(value, 0, maxEnergy);
        return true;
    }

	// Returns added value
	public int AddEnergy(int value) {

        if (disabled)
            return 0;

        int addition = Mathf.Min(Mathf.Abs(value), maxEnergy-energy);
        SetEnergy(energy + addition);
        averageEnergyGain += Mathf.Abs(value);

        return addition;
	}

    // Returns substracted value
    public int RemoveEnergy(int value) {

        if (disabled)
            return 0;

        int substraction = Mathf.Min(Mathf.Abs(value), energy);
        SetEnergy(energy - substraction);
        averageEnergyLoss += Mathf.Abs(value);

        return substraction;
    }
}
