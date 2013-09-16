using UnityEngine;
using System.Collections.Generic;

public class CollisionRecorder : MonoBehaviour {
	public Dictionary<GameObject, Collider> TriggersEntered { get; set; }
	public Dictionary<GameObject, Collider> TriggersExited { get; set; }
	public Dictionary<GameObject, Collider> TriggersStay { get; set; }

	public Dictionary<GameObject, Collision> CollidersEntered { get; set; }
	public Dictionary<GameObject, Collision> CollidersExited { get; set; }
	public Dictionary<GameObject, Collision> CollidersStay { get; set; }


	public void Awake() {
		TriggersEntered = new Dictionary<GameObject, Collider>();
		TriggersExited = new Dictionary<GameObject, Collider>();
		TriggersStay = new Dictionary<GameObject, Collider>();
		CollidersEntered = new Dictionary<GameObject, Collision>();
		CollidersExited = new Dictionary<GameObject, Collision>();
		CollidersStay = new Dictionary<GameObject, Collision>();
	}

	public void OnTriggerEnter(Collider other) {
		TriggersEntered[other.gameObject] = other;
		TriggersStay[other.gameObject] = other;
	}
	
	public void OnTriggerExit(Collider other) {
		TriggersExited[other.gameObject] = other;
		TriggersStay.Remove(other.gameObject);
	}

	public void OnCollisionEnter(Collision other) {
		CollidersEntered[other.gameObject] = other;
		CollidersStay[other.gameObject] = other;
	}

	public void OnCollisionExit(Collision other) {
		CollidersExited[other.gameObject] = other;
		CollidersStay.Remove(other.gameObject);
	}

	public void LateUpdate() {
		TriggersEntered.Clear();
		TriggersExited.Clear();
		CollidersEntered.Clear();
		CollidersExited.Clear();
	}
}