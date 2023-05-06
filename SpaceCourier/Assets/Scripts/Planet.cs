using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

	private void Awake() {

		InitialInclination();
	}

	private void InitialInclination() {

		float maxDegrees = (1.0f + Mathf.Sin(transform.position.x)) + (1.0f + Mathf.Cos(transform.position.z)) * 5.0f;

		Vector3 inclination = new Vector3(transform.localRotation.x, transform.localRotation.y + Random.Range(0, 360f), transform.localRotation.z + Random.Range(0, maxDegrees));
		
		transform.localRotation = Quaternion.Euler(inclination);
	}
}
