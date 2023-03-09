using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour {

    public static Controller instance;

    [SerializeField] Ship ship;
    [SerializeField] CameraSystem cameraSystem;

    private Vector2 desiredDirection;
    private float dTime;

    void Awake() {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

	private void Start() {
		
	}

	// Update is called once per frame
	void Update() {

        dTime = Time.deltaTime;
        HandleMovement();
    }

    /// <summary>
	/// Handle movement actions
	/// </summary>
    private void HandleMovement() {

        ship.Move(desiredDirection);
	}

    public void OnMove(InputAction.CallbackContext context) {

        desiredDirection = context.ReadValue<Vector2>();
	}
}
