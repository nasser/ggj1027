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
	
	// Update is called once per frame
	void Update () {
	
//		float step = speed * Time.deltaTime;
//		transform.position = Vector3.MoveTowards (transform.position, target, step);
//
//		Debug.Log (Vector3.Distance (transform.position, target));
//
//		// choose random point within radius
//		if (Vector3.Distance(transform.position, target) < .5f) {
//			target = new Vector3 (Random.Range(0, radius), 0, Random.Range(0, radius));
//		}

		


	}

	void OnCollisionEnter(Collision collision) {
		// (replay/register-moment (.. other transform position))
		RT.var("game.replay", "register-moment").invoke(transform.position);
		if (collision.transform.root.tag == "Punchable" && fps.GetComponent<PlayerController>().hasHit) {
			if (collision.transform.root.GetComponent<PersonController> ().hasBeenHit == false) {
				collision.rigidbody.AddForce (gameObject.GetComponent<Rigidbody> ().velocity * 10);
				collision.transform.root.GetComponent<PersonController> ().hasBeenHit = true;
				collision.transform.root.GetComponent<MeshRenderer> ().material = hitMaterial;
			}

		}

	}
}
