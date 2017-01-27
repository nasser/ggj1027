using UnityEngine;
using System.Collections;
using clojure.lang;

public class PersonController : MonoBehaviour {

	public int radius;
	public float speed;
	private Vector3 target;
	public bool pushedByPlayer;
	public bool hasBeenHit;
	public GameObject fps;
	public Material hitMaterial;

	// Use this for initialization
	void Start () {
		pushedByPlayer = false;
//		target = new Vector3 (Random.Range(0, radius), 0, Random.Range(0, radius));
		fps = GameObject.Find("FPSController"); 
	}
	
	void OnCollisionEnter(Collision collision) {
		if (transform.root != collision.transform.root && collision.transform.root.tag == "Record Me") {
			// if(GetComponent<Animator>() != null)
				// GetComponent<Animator>().enabled = false;
			// Debug.Log(gameObject.name + " collided " + collision.gameObject.name + " tag " + collision.gameObject.tag + " root tag " + collision.transform.root.tag);
			// Debug.Log(gameObject.name + " collided " + collision.gameObject.name);
			// GetComponent<Rigidbody>().AddForce (collision.relativeVelocity * 1000);
			// collision.rigidbody.AddForce (collision.relativeVelocity * 1000);
			// collision.transform.root.GetComponent<PersonController> ().hasBeenHit = true;
			// Destroy(GetComponent<BoxCollider>());
		}

	}
}
