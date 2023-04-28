using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour {

    public float maxRotationSpeed;
	private float rotationSpeed;
	Vector3 orbit;

	private void Awake() {

		int seed = (int)(transform.position.x * 3 + transform.position.z * 7);
		Random.InitState(seed);
		rotationSpeed = Random.Range(0, maxRotationSpeed);

		orbit = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
	}

	private void Update() {

		Rotate();
	}

	private void Rotate() {

		Vector3 rotationValue = rotationSpeed * Time.deltaTime * orbit;
		rotationValue.x %= 360;
		rotationValue.y %= 360;
		rotationValue.z %= 360;
		
		transform.Rotate(rotationValue);
	}
}
