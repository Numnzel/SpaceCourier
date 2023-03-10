using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour {

    public GameObject followed;

	private void Update() {

		transform.position = followed.transform.position;
	}
}
