using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float forceRadius;
	public float forcePower;
	public bool hasHit;

	public PhysicMaterial startPhysicMaterial;
	public PhysicMaterial endPhysicMaterial;

	private GameObject floor;

	public AudioSource whack;

	// Use this for initialization
	void Start () {
		hasHit = false;
		floor = GameObject.Find ("Floor");
//		floor.GetComponent<BoxCollider> ().material = startPhysicMaterial;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			Collider[] nearbyColliders = Physics.OverlapSphere (transform.position, forceRadius);
			for (int i = 0; i < nearbyColliders.Length; i++) {
				Rigidbody r = nearbyColliders [i].GetComponent<Rigidbody> ();
				if (r != null) {
					Vector3 ourForward = gameObject.transform.forward;
					r.AddForce (new Vector3(ourForward.x, 0.1f, ourForward.z) * forcePower);
					r.AddTorque (transform.up * 2 * 2);
//					hasHit = true;
//					if (!whack.isPlaying) {
						whack.Play ();
//					}

				}
			}
		}

		if (hasHit == true) {

//			floor.GetComponent<BoxCollider> ().material = endPhysicMaterial;
		}
	}



}
