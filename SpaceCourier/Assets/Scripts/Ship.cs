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
    [SerializeField] private List<GameObject> truckFlamesForwards;
    [SerializeField] private List<GameObject> truckFlamesBackwards;
    [SerializeField] private List<GameObject> truckFlamesTurnRight;
    [SerializeField] private List<GameObject> truckFlamesTurnLeft;

    public bool dead = false;
    public int loadCount;

    const float smallFlameBaseScale = 50.0f;
    const float largeFlameBaseScale = 130.0f;

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

        // Apply SFX (Flames)
        SetFlames(usedEForward * dir.y, usedESideward * dir.x);
    }

	private void OnCollisionEnter(Collision collision) {

        powered.RemoveEnergy(100000);
        powered.disabled = true;
        truckModel.transform.localScale = Vector3.zero;
        SetFlames(0, 0);
        RB.velocity = Vector3.zero;
        RB.isKinematic = true;
        // spawn explosion

        SetDead();
    }

    private void SetDead() {

        dead = true;
    }

    private void SetFlames(float force, float torque) {

        float tremblingValue = Mathf.Sin(Time.time / 100) + 0.7f;
        float tremblingForce = force * tremblingValue;
        float tremblingTorque = torque * tremblingValue;

        foreach (GameObject flame in truckFlamesForwards)
            flame.transform.localScale = Vector3.one * Mathf.Min(largeFlameBaseScale * 1.0f, largeFlameBaseScale * Mathf.Max(0, tremblingForce / 9));

        foreach (GameObject flame in truckFlamesBackwards)
            flame.transform.localScale = Vector3.one * Mathf.Min(smallFlameBaseScale * 1.2f, smallFlameBaseScale * Mathf.Max(0, -tremblingForce / 3));

        foreach (GameObject flame in truckFlamesTurnRight)
            flame.transform.localScale = Vector3.one * Mathf.Min(smallFlameBaseScale * 1.2f, smallFlameBaseScale * Mathf.Max(0, tremblingTorque / 3));

        foreach (GameObject flame in truckFlamesTurnLeft)
            flame.transform.localScale = Vector3.one * Mathf.Min(smallFlameBaseScale * 1.2f, smallFlameBaseScale * Mathf.Max(0, -tremblingTorque / 3));
    }
}
