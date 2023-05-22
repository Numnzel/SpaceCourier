using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Powered))]
public class Ship : MonoBehaviour {

    [SerializeField] Powered powered;
    [SerializeField] private int baseImpulseForward;
    [SerializeField] private int baseImpulseTorque;
    [SerializeField] private int baseImpulseBackward;
    [SerializeField] private float baseImpulseMultiplier;
    [SerializeField] private List<GameObject> truckFlamesForwards;
    [SerializeField] private List<GameObject> truckFlamesBackwards;
    [SerializeField] private List<GameObject> truckFlamesTurnRight;
    [SerializeField] private List<GameObject> truckFlamesTurnLeft;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody RB;
    public bool dead = false;
    public int loadCount = 1;
    public GameObject truckModel;
    public GameObject[] truckLoad;

    const float smallFlameBaseScale = 50.0f;
    const float largeFlameBaseScale = 130.0f;

	private void Start() {

        // Set load count to match level unloaders amount
        CargoBay[] unloaders = FindObjectsOfType<CargoBay>();
        loadCount = unloaders.Length;
        UpdateShipLoad(loadCount);
    }

	private void Update() {

        if (powered.disabled)
            SetDead();
	}

    private void UpdateShipLoad(int amount) {

		for (int i = truckLoad.Length; i > 0; i--)
            if (amount < i)
                truckLoad[i-1].transform.localScale = Vector3.zero;

        RB.mass = 1.0f + (amount * 0.2f);
    }

    public void Unload() {

        loadCount--;
        truckLoad[loadCount].transform.localScale = Vector3.zero;
        UpdateShipLoad(loadCount);
    }

	public void Move(Vector2 dir) {

        if (dead)
            return;

        // Calculate forward/backward force
        float impulseVertical = dir.y > 0 ? baseImpulseForward : baseImpulseBackward;
        impulseVertical *= Time.deltaTime * dir.y;

        int usedEForward = powered.RemoveEnergy(Mathf.CeilToInt(Mathf.Abs(impulseVertical)));
        Vector3 force = transform.forward * usedEForward * dir.y;

        // Calculate sideward force
        float impulseSides = baseImpulseTorque;
        impulseSides *= Time.deltaTime * dir.x;

        int usedESideward = powered.RemoveEnergy(Mathf.CeilToInt(Mathf.Abs(impulseSides)));
        Vector3 torque = transform.up * usedESideward * dir.x;

        // Apply forces
        RB.AddForce(force * baseImpulseMultiplier, ForceMode.Force);
        RB.AddTorque(torque * baseImpulseMultiplier, ForceMode.Force);

        // Apply SFX (Flames)
        SetFlamesSize(usedEForward * dir.y, usedESideward * dir.x);
    }

    public void DisableShip() {

        powered.disabled = true;
        SetFlamesSize(0, 0);
        RB.velocity = Vector3.zero;
        RB.isKinematic = true;
    }

	private void OnCollisionEnter(Collision collision) {

        Crash(collision.collider.transform.position);
        SetDead();
    }

    private void Crash(Vector3 crashPosition) {

        if (truckModel) {

            List<Transform> allParts = new List<Transform>();
            GetAllParts(truckModel.transform, ref allParts);
            ExplodeParts(allParts, crashPosition);
        }
        powered.RemoveEnergy(100000);
        DisableShip();

        // spawn explosion
    }

    private void GetAllParts(Transform obj, ref List<Transform> allParts) {
        
        if (obj == null)
            return;

        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(obj);

        while (queue.Count > 0) {

            Transform currentObj = queue.Dequeue();
            allParts.Add(currentObj);

            foreach (Transform child in currentObj)
                queue.Enqueue(child);
        }
    }

    private void ExplodeParts(List<Transform> parts, Vector3 collisionPos) {

        if (parts == null)
            return;
        
        Rigidbody RB;
        foreach (Transform part in parts) {

            RB = part.GetComponent<Rigidbody>();

            if (RB == null)
                continue;

            part.parent = null;
            RB.useGravity = true;
            RB.isKinematic = false;
            // Force = dP^2 * v
            float force = Mathf.Pow((part.transform.position - collisionPos).magnitude * Random.Range(1.0f, 1.3f), 2) * this.RB.velocity.magnitude;
            RB.angularVelocity = new Vector3(Random.Range(-force, force), Random.Range(-force, force), Random.Range(-force, force)) * 0.005f;
            RB.AddExplosionForce(force * 0.4f, collisionPos, 20f);
        }
    }

    private void SetDead() {

        dead = true;
    }

    private void SetFlamesSize(float force, float torque) {

        float normalizedForce = Mathf.Clamp(force, -5.0f, 5.0f) / 5.0f; // clamp and normalize force
        float normalizedTorque = Mathf.Clamp(torque, -5.0f, 5.0f) / 5.0f; // clamp and normalize torque
        float tremblingFactor = 1.0f - (Mathf.Abs(Mathf.Sin(Time.time * 16.0f)) / 5.0f); // oscillates between 0.8 and 1.0
        float tremblingForce = normalizedForce * tremblingFactor; // apply oscillation
        float tremblingTorque = normalizedTorque * tremblingFactor; // apply oscillation

        // Apply flame transforms
        foreach (GameObject flame in truckFlamesForwards)
            flame.transform.localScale = Vector3.one * Mathf.Min(largeFlameBaseScale, largeFlameBaseScale * Mathf.Max(0, tremblingForce));

        foreach (GameObject flame in truckFlamesBackwards)
            flame.transform.localScale = Vector3.one * Mathf.Min(smallFlameBaseScale, smallFlameBaseScale * Mathf.Max(0, -tremblingForce));

        foreach (GameObject flame in truckFlamesTurnRight)
            flame.transform.localScale = Vector3.one * Mathf.Min(smallFlameBaseScale, smallFlameBaseScale * Mathf.Max(0, tremblingTorque));

        foreach (GameObject flame in truckFlamesTurnLeft)
            flame.transform.localScale = Vector3.one * Mathf.Min(smallFlameBaseScale, smallFlameBaseScale * Mathf.Max(0, -tremblingTorque));

        // Apply ship motor animations
        animator.SetFloat("BlendMotorFrontR", Mathf.Max(0, -normalizedForce));
        animator.SetFloat("BlendMotorFrontL", Mathf.Max(0, -normalizedForce));
        animator.SetFloat("BlendMotorBack", Mathf.Max(0, normalizedForce));
        animator.SetFloat("BlendMotorLeftF", Mathf.Max(0, normalizedTorque));
        animator.SetFloat("BlendMotorLeftB", Mathf.Max(0, -normalizedTorque));
        animator.SetFloat("BlendMotorRightF", Mathf.Max(0, -normalizedTorque));
        animator.SetFloat("BlendMotorRightB", Mathf.Max(0, normalizedTorque));
    }
}
