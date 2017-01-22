using UnityEngine;
using System.Collections;

public class BuildingController : MonoBehaviour {
	public GameObject fps;

	// Use this for initialization
	void Start () {
		fps = GameObject.Find("FPSController");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {

		if (collision.transform.root.tag == "Punchable" && fps.GetComponent<PlayerController>().hasHit) {
//			if (collision.transform.root.GetComponent<PersonController> ().hasBeenHit == false) {
				Vector3 me = collision.rigidbody.velocity;
			Debug.Log ("hit!");
			collision.rigidbody.AddForce (new Vector3(-me.x, 0, -me.z) * me.magnitude * 5f);
				collision.transform.root.GetComponent<PersonController> ().hasBeenHit = true;
//				collision.transform.root.GetComponent<MeshRenderer> ().material = hitMaterial;
//			}

		}

	}
}
