using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Powered))]
public class Ship : MonoBehaviour {

    Rigidbody RB;
    [SerializeField] float impulseForward;
    [SerializeField] float impulseTorque;
    [SerializeField] float impulseBackward;
    [SerializeField] Powered powered;

	void Awake() {

        RB = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 dir) {

		if (powered.Energy == 0)
            return;

		Vector3 force = transform.forward * dir.y * Time.deltaTime;
        force *= dir.y > 0 ? impulseForward : impulseBackward;
        Vector3 torque = transform.up * dir.x * impulseTorque * Time.deltaTime;

        RB.AddForce(force, ForceMode.Force);
        RB.AddTorque(torque, ForceMode.Force);

        powered.Add(-Mathf.RoundToInt(force.magnitude + torque.magnitude));

        Debug.Log(powered.Energy);
	}
}
