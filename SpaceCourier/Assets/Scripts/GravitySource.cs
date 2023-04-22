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

        if (other.GetComponent<Rigidbody>() && !other.attachedRigidbody.isKinematic)
            atractedObjects.Add(other.attachedRigidbody);
	}

	private void OnTriggerExit(Collider other) {

        if (!other.attachedRigidbody.isKinematic)
            atractedObjects.Remove(other.attachedRigidbody);
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

            float dist = Vector3.Distance(transform.position, RB.position);
            float gForce = (this.RB.mass * RB.mass) / (dist * dist);
            Vector3 dir = new Vector3(transform.position.x - RB.transform.position.x, 0, transform.position.z - RB.transform.position.z);

            RB.AddForce(dir.normalized * gForce * gravityMultiplier * Time.deltaTime, ForceMode.Force);
        }
    }
}
