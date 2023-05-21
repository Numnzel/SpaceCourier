using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class GravitySource : MonoBehaviour {

    private List<Rigidbody> atractedObjects;
    [SerializeField] private Rigidbody RB;
    [SerializeField] private SphereCollider triggerSphere;
    [SerializeField] private float gravitySphereRadiusMultiplier;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private bool sizeFromMass;

    void Awake() {

        atractedObjects = new List<Rigidbody>();

        if (triggerSphere == null) {

            Debug.LogWarning("This " + gameObject.name + " has no collider.");
            return;
        }

        SetScale();
        SetGravitySphere();
    }

	private void OnTriggerEnter(Collider other) {

        Rigidbody RB = other.attachedRigidbody;

        if (!other.gameObject.isStatic && RB != null && !atractedObjects.Contains(RB))
            atractedObjects.Add(RB);
	}

	private void OnTriggerExit(Collider other) {

        Rigidbody RB = other.attachedRigidbody;

		if (RB != null && atractedObjects.Contains(RB))
            atractedObjects.Remove(RB);
	}

	private void OnDrawGizmos() {
        
        SetScale();
        SetGravitySphere();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerSphere.radius * transform.localScale.x);
    }

    private void SetScale() {

        if (sizeFromMass)
            transform.localScale = Vector3.one * (1 + Mathf.Log(RB.mass));
    }

    private void SetGravitySphere() {

        triggerSphere.radius = RB.mass * gravitySphereRadiusMultiplier;
        triggerSphere.isTrigger = true;
    }

    private void Update() {
        
		foreach (Rigidbody RB in atractedObjects) {

            if (RB == null)
                return;

            float dist = Vector3.Distance(transform.position, RB.position);
            float gForce = (this.RB.mass * RB.mass) / (dist * dist);
            Vector3 dir = new Vector3(transform.position.x - RB.transform.position.x, 0, transform.position.z - RB.transform.position.z);

            RB.AddForce(dir.normalized * gForce * gravityMultiplier * Time.deltaTime, ForceMode.Force);
        }
    }
}
