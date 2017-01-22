using UnityEngine;
using System.Collections;

public class ToggleKinematicOnCollide : MonoBehaviour {

	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {
		// if (collision.transform.root.gameObject.GetComponent<PersonController> ().hasBeenHit) {
			if (rb.isKinematic) {
				rb.isKinematic = false;
			}
		// }


	}
}
