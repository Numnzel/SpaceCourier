using System;
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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip soundCrash;
    [SerializeField] private AudioClip soundPropulsion;
    public bool dead = false;
    public bool deathDisabled = false;
    public int loadCount = 1;
    public GameObject truckModel;
    public GameObject[] truckLoad;
    private int usedESidewardMem;
    private int usedEForwardMem;
    private float currentRotation;
    private int rotations;

    public static Action<string, int> OnCrash;
    public static Action<string, int> OnDeath;

    const float smallFlameBaseScale = 50.0f;
    const float largeFlameBaseScale = 130.0f;

	public int Rotations { get => rotations; set => rotations = value; }

	private void Start() {

        // Set load count to match level unloaders amount
        CargoBay[] unloaders = FindObjectsOfType<CargoBay>();
        loadCount = unloaders.Length;
        UpdateShipLoad(loadCount);

        // Count full rotations for achievement
        if (!PlayerData.achievements.Find(x => x.id == "rotate").unlocked)
            StartCoroutine(CountFullRotation());
    }

	private void Update() {

        if (powered.disabled)
            SetDead();
    }

    private IEnumerator CountFullRotation() {

        currentRotation = 0;

        if (PlayerData.statistics.ContainsKey("Rotation"))
            PlayerData.statistics["Rotation"].value = 0;

        DataManager.SavePlayerData();

        while (Rotations < PlayerData.achievements.Find(x => x.id == "rotate").requirement.value) {

            while (Mathf.Abs(currentRotation) < 0.95f) {

                currentRotation = transform.rotation.y;

                yield return new WaitForEndOfFrame();
            }

            while (Mathf.Abs(currentRotation) > 0.05) {

                currentRotation = transform.rotation.y;

                yield return new WaitForEndOfFrame();
            }

            Rotations++;
        }
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

        // Apply flame sound
        if ((usedEForward > 0 && usedEForwardMem == 0) || (usedESideward > 0 && usedESidewardMem == 0))
            audioSource.Stop();

        usedEForwardMem = usedEForward;
        usedESidewardMem = usedESideward;

        if (audioSource != null && !PlayerData.optionValue_mutePropulsion) {
            
            if (usedEForward == 0 && usedESideward == 0)
                audioSource.Stop();
            else if (!audioSource.isPlaying)
                audioSource.PlayOneShot(soundPropulsion, 0.1f);
        }
    }

    public void DisableShip() {
        
        powered.disabled = true;
        SetFlamesSize(0, 0);
        RB.velocity = Vector3.zero;
        RB.isKinematic = true;
    }

	private void OnCollisionEnter(Collision collision) {

        Crash(collision.collider.transform.position, collision.relativeVelocity.magnitude);
    }

    private void Crash(Vector3 crashPosition, float crashSpeed) {
        
        OnCrash?.Invoke("Crash", 1);

        if (!PlayerData.achievements.Find(x => x.id == "summon").unlocked && CanvasManager.instance.LevelTime < 3f)
            OnCrash?.Invoke("AchievementSummoningSickness", 1);

        if (truckModel) {

            List<Transform> allParts = new List<Transform>();
            GetAllParts(truckModel.transform, ref allParts);
            ExplodeParts(allParts, crashPosition, crashSpeed);
        }
        powered.RemoveEnergy(100000);
        DisableShip();

        // play sound
        if (audioSource != null) {

            audioSource.Stop();
            audioSource.PlayOneShot(soundCrash);
        }

        // spawn explosion
        SetDead();
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

    private void ExplodeParts(List<Transform> parts, Vector3 collisionPos, float collisionSpeed) {

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
            float force = Mathf.Pow((part.transform.position - collisionPos).magnitude * UnityEngine.Random.Range(1.0f, 1.3f), 2) * (collisionSpeed * 0.25f);
            RB.angularVelocity = new Vector3(UnityEngine.Random.Range(-force, force), UnityEngine.Random.Range(-force, force), UnityEngine.Random.Range(-force, force)) * 0.005f;
            RB.AddExplosionForce(force * 0.4f, collisionPos, 20f);
        }
    }

    private void SetDead() {

        if (deathDisabled)
            return;

        dead = true;
        OnDeath?.Invoke("Death", 1);
        
        // Remove hardcore level count if not completed
        if (PlayerData.statistics.ContainsKey("LevelHardcore") && !PlayerData.achievements.Find(x => x.id == "safety").unlocked) {

            PlayerData.statistics["LevelHardcore"].value = 0;
            DataManager.SavePlayerData();
        }
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
