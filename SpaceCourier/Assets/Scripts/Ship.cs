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
    [SerializeField] private float impulseMultiplier;
    [SerializeField] private GameObject truckModel;
    [SerializeField] private GameObject truckLoad;

    public bool dead = false;
    public int loadCount;

    void Awake() {

        RB = GetComponent<Rigidbody>();
    }

	private void Update() {

        if (powered.disabled)
            SetDead();

        if (loadCount == 0)
            truckLoad.transform.localScale = Vector3.zero;
	}

	public void Move(Vector2 dir) {

        if (dead)
            return;

        // Calculate forward/backward force
        float impulseVertical = dir.y > 0 ? impulseForward : impulseBackward;
        impulseVertical *= Time.deltaTime * dir.y;

        int usedEForward = powered.RemoveEnergy(Mathf.CeilToInt(Mathf.Abs(impulseVertical)));
        Vector3 force = transform.forward * usedEForward * dir.y;

        // Calculate sideward force
        float impulseSides = impulseTorque;
        impulseSides *= Time.deltaTime * dir.x;

        int usedESideward = powered.RemoveEnergy(Mathf.CeilToInt(Mathf.Abs(impulseSides)));
        Vector3 torque = transform.up * usedESideward * dir.x;

        // Apply forces
        RB.AddForce(force * impulseMultiplier, ForceMode.Force);
        RB.AddTorque(torque * impulseMultiplier, ForceMode.Force);
    }

	private void OnCollisionEnter(Collision collision) {

        powered.RemoveEnergy(100000);
        powered.disabled = true;
        truckModel.transform.localScale = Vector3.zero;
        RB.velocity = Vector3.zero;
        RB.isKinematic = true;
        // spawn explosion

        SetDead();
    }

    private void SetDead() {

        dead = true;
    }
}
