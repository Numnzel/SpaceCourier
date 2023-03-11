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

    private int usedEForward;
    private int usedESideward;
    private int usedETotal;

    const int usedEnergyIndicatorMax = 30;

    void Awake() {

        RB = GetComponent<Rigidbody>();
    }

	private void Update() {

        // Consume energy
        int usedEPassive = powered.Remove(Mathf.Min(passiveEnergyConsumption, powered.Energy));

        // Calculate consumed energy
        usedETotal = usedEForward + usedESideward + usedEPassive;

        // Update UI
        UIManager.instance.UpdateEnergyBar(powered.Energy, powered.MaxEnergy);
        UIManager.instance.UpdateImpulseBar(usedETotal, usedEnergyIndicatorMax);
    }

	public void Move(Vector2 dir) {

        // Calculate forward/backward force
        float impulseVertical = dir.y > 0 ? impulseForward : impulseBackward;
        impulseVertical *= Time.deltaTime * dir.y;

        usedEForward = powered.Remove(Mathf.CeilToInt(Mathf.Abs(impulseVertical)));
        Vector3 force = transform.forward * usedEForward * dir.y;

        // Calculate sideward force
        float impulseSides = impulseTorque;
        impulseSides *= Time.deltaTime * dir.x;

        usedESideward = powered.Remove(Mathf.CeilToInt(Mathf.Abs(impulseSides)));
        Vector3 torque = transform.up * usedESideward * dir.x;

        // Apply forces
        RB.AddForce(force * forceMultiplier, ForceMode.Force);
        RB.AddTorque(torque * forceMultiplier, ForceMode.Force);
    }
}
