using UnityEngine;
using System.Collections;
using clojure.lang;

public class BumperBehavior : MonoBehaviour {

	public float power;
	public string dir;
	private float selfPower;
	private int collisionCount;

	public int maxTime;
	private int timer;

	public string soundType;

	public GameObject toInstantiate;
	public AudioSource thud;


	// Use this for initialization
	void Start () {
		selfPower = 1000000;
		collisionCount = 0;

		if (soundType == "") {
			soundType = "thud";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (collisionCount > 0) {
			timer = maxTime;
		} else {
			timer--;
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Record Me") {
			// RT.var("game.replay", "register-moment").invoke(collision.gameObject.transform.position	);
			Vector3 frc = CalculateForce (collision);
			Vector3 me = CalculateMyReaction ();
			collision.rigidbody.AddForce (frc);
			collisionCount++;
			gameObject.GetComponent<Rigidbody> ().AddForce (me);

			ContactPoint firstContact = collision.contacts [0];


			if (toInstantiate) {
				GameObject effect = Instantiate (toInstantiate, firstContact.point, Quaternion.identity) as GameObject;
				effect.GetComponent<Rigidbody> ().AddForce (frc);
			}
		}
	}

	void OnCollisionExit(Collision collision) {
		collisionCount--;
	}

	Vector3 CalculateForce(Collision collision) {
		if (thud != null && !thud.isPlaying) {
			thud.Play ();
		}
		if (dir == "up") {
			return (Vector3.up * power);
		} else if (dir == "bounce") {
			Vector3 curVelocity = collision.gameObject.GetComponent<Rigidbody> ().velocity;
			return (curVelocity * -1 * power);
		} else {
			return (Vector3.forward);
		}




	}

	Vector3 CalculateMyReaction() {
		if (timer > 0) {
			return Vector3.zero;
		}

		if (dir == "up") {
			float m = gameObject.GetComponent<Rigidbody> ().mass;
			return (Vector3.down * selfPower * m);
		}
		else {
			return (Vector3.zero);
		}
	}
}
