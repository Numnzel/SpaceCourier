using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class Controller : MonoBehaviour {

    public static Controller instance;

    private Ship ship;
    [SerializeField] private CameraSystem cameraSystem;
    [SerializeField] private Joystick joystickLeft;
    [SerializeField] private Joystick joystickRight;

    private Vector2 desiredDirection;
    private float dTime;

    void Awake() {

        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
            Destroy(gameObject);
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

        if (!GameManager.instance.RunningWindows) {

            desiredDirection.x = joystickLeft.Horizontal;
            desiredDirection.y = joystickRight.Vertical;
        }

        if (ship != null)
            ship.Move(desiredDirection);
    }

    public void SetShip(Ship ship) {

        this.ship = ship;
        cameraSystem.followed = ship.gameObject;
    }

    public void OnMove(InputAction.CallbackContext context) {

        desiredDirection = context.ReadValue<Vector2>();
	}

    public void EnterMenuOrReturn(InputAction.CallbackContext context) {

        if (context.started)
            GameManager.instance.EnterMenuOrReturn();
    }
    public void RestartLevel(InputAction.CallbackContext context) {

        if (context.started)
            GameManager.instance.RestartScene();
    }
}
