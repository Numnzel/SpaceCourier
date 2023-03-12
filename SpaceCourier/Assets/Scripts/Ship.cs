using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Powered))]
public class Ship : MonoBehaviour {

    Rigidbody RB;
    [SerializeField] Powered powered;
    [SerializeField] private int impulseForward;
    [SerializeField] private int impulseTorque;
    [SerializeField] private int impulseBackward;
    [SerializeField] private float forceMultiplier;
    [SerializeField] private int passiveEnergyConsumption;
    [SerializeField] private GameObject truckModel;

    private int usedEForward;
    private int usedESideward;
    private int usedETotal;
    private int usedEPassive;

    const int usedEnergyIndicatorMax = 30;

    private int freezeAmount;
    private bool dead = false;

    private Coroutine energyCoroutine;
    private Coroutine freezeCoroutine;


    void Awake() {

        RB = GetComponent<Rigidbody>();
    }

	private void Start() {

        energyCoroutine = StartCoroutine(PassiveConsumption());
	}

	private void Update() {

        // Passive energy consumption
        usedEPassive = powered.RemoveEnergy(Mathf.Min(passiveEnergyConsumption, powered.Energy));

        // Calculate total consumed energy
        usedETotal = dead ? 0 : usedEForward + usedESideward + usedEPassive;

        // Update UI
        UIManager.instance.UpdateEnergyBar(powered.Energy, powered.MaxEnergy);
        UIManager.instance.UpdateImpulseBar(usedETotal, usedEnergyIndicatorMax);
        UIManager.instance.UpdateFreezeBar(freezeAmount, 5);
    }

	public void Move(Vector2 dir) {

        if (dead)
            return;

        // Calculate forward/backward force
        float impulseVertical = dir.y > 0 ? impulseForward : impulseBackward;
        impulseVertical *= Time.deltaTime * dir.y;

        usedEForward = powered.RemoveEnergy(Mathf.CeilToInt(Mathf.Abs(impulseVertical)));
        Vector3 force = transform.forward * usedEForward * dir.y;

        // Calculate sideward force
        float impulseSides = impulseTorque;
        impulseSides *= Time.deltaTime * dir.x;

        usedESideward = powered.RemoveEnergy(Mathf.CeilToInt(Mathf.Abs(impulseSides)));
        Vector3 torque = transform.up * usedESideward * dir.x;

        // Apply forces
        RB.AddForce(force * forceMultiplier, ForceMode.Force);
        RB.AddTorque(torque * forceMultiplier, ForceMode.Force);
    }

	private void OnCollisionEnter(Collision collision) {

        powered.RemoveEnergy(100000);
        truckModel.transform.localScale = Vector3.zero;
        // spawn explosion

        SetDead();
    }

    public IEnumerator PassiveConsumption() {

		while (powered.Energy > 0 && !dead) {

            yield return new WaitForSeconds(1f); // Wait 1 second before starting consumption
            usedEPassive = powered.RemoveEnergy(passiveEnergyConsumption);
        }

        usedEPassive = 0;
        
        if (!dead)
            freezeCoroutine = StartCoroutine(Freeze());

        if (energyCoroutine != null)
            StopCoroutine(energyCoroutine);
    }

    public IEnumerator Freeze() {

        while (powered.Energy == 0 && !dead) {

            yield return new WaitForSeconds(1f);

            if (++freezeAmount == 5)
                SetDead();
        }

        if (!dead)
            energyCoroutine = StartCoroutine(PassiveConsumption());

        if (freezeCoroutine != null)
            StopCoroutine(freezeCoroutine);
    }

    private void SetDead() {

        powered.disabled = true;
        dead = true;
    }
}
