using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTransformAxis : MonoBehaviour {

	public bool lockAxisX;
	public bool lockAxisY;
	public bool lockAxisZ;

	Vector3 startingPos;

	private void Start() {

		startingPos = transform.position;
	}

	private void Update() {

		Vector3 currentPos = transform.position;
		
		if (lockAxisX && currentPos.x != startingPos.x)
			currentPos.x = startingPos.x;
		if (lockAxisY && currentPos.y != startingPos.y)
			currentPos.y = startingPos.y;
		if (lockAxisZ && currentPos.z != startingPos.z)
			currentPos.z = startingPos.z;

		transform.position = currentPos;
	}
}
