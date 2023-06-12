using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

    public float maxRotationSpeed;
	public bool rotateX;
	public bool rotateY;
	public bool rotateZ;

	private float rotationSpeed;
	Vector3 orbit;

	private void Awake() {

		int seed = (int)(transform.position.x * 3 + transform.position.z * 7);
		Random.InitState(seed);
		rotationSpeed = Random.Range(0, maxRotationSpeed);

		float dirX = rotateX ? Random.Range(-1f, 1f) : 0;
		float dirY = rotateY ? Random.Range(-1f, 1f) : 0;
		float dirZ = rotateZ ? Random.Range(-1f, 1f) : 0;

		orbit = new Vector3(dirX, dirY, dirZ).normalized;
	}

	private void Update() {

		Rotate();
	}

	private void Rotate() {

		Vector3 rotationValue = rotationSpeed * Time.deltaTime * orbit;
		rotationValue.x %= 360f;
		rotationValue.y %= 360f;
		rotationValue.z %= 360f;
		
		transform.Rotate(rotationValue);
	}
}
