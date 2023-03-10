using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class GravitySource : MonoBehaviour {

    private List<Rigidbody> RBs;
    [SerializeField] Rigidbody RB;
    [SerializeField] private SphereCollider area;
    [SerializeField] private float areaMultiplier = 10;
    [SerializeField] bool sizeFromMass;

    void Awake() {

        RBs = new List<Rigidbody>();

        if (area == null) {

            Debug.LogWarning("This " + gameObject.name + " has no collider.");
            return;
        }

        if (sizeFromMass)
            transform.localScale = Vector3.one * RB.mass;

        area.radius = RB.mass * areaMultiplier;
        area.isTrigger = true;
    }

	private void OnTriggerEnter(Collider other) {

		if (!other.attachedRigidbody.isKinematic)
            RBs.Add(other.attachedRigidbody);
	}

	private void OnTriggerExit(Collider other) {

        if (!other.attachedRigidbody.isKinematic)
            RBs.Remove(other.attachedRigidbody);
	}

	private void OnDrawGizmos() {

        Gizmos.DrawWireSphere(transform.position, RB.mass * areaMultiplier);

        if (sizeFromMass)
            transform.localScale = Vector3.one * RB.mass;
    }

	void Update() {
        
		foreach (Rigidbody RB in RBs) {

            float dist = Vector3.Distance(transform.position, RB.position);
            float gForce = (this.RB.mass * RB.mass) / (dist * dist);
            Vector3 dir = new Vector3(transform.position.x - RB.transform.position.x, 0, transform.position.z - RB.transform.position.z);

            RB.AddForce(dir.normalized * gForce, ForceMode.Force);
        }
    }
}
