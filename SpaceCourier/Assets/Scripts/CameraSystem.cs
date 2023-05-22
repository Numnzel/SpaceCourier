using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour {

	private static CameraSystem instance;
    internal GameObject followed;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
    }

    private void Update() {

        if (followed != null)
		    transform.position = followed.transform.position;
	}

    public void RestartCamera() {

        followed = null;
        transform.position = Vector3.zero;
    }
}
