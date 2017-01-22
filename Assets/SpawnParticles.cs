using UnityEngine;
using System.Collections;

public class SpawnParticles : MonoBehaviour {

	public ParticleSystem particles;
	private bool hasSpawnedParticles;

	// Use this for initialization
	void Start () {
		hasSpawnedParticles = false;
		particles.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Punchable") {
			
			if (!hasSpawnedParticles) {

				particles.gameObject.SetActive (true);

				ContactPoint firstContact = collision.contacts [0];
				particles.transform.position = firstContact.point;
				particles.Play ();
				hasSpawnedParticles = true;
			}
		}
	}

	void OnCollisionExit(Collision collision) {
		if (collision.gameObject.tag == "Punchable") {
			hasSpawnedParticles = false;
		}
	}
}
