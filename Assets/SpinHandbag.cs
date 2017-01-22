using UnityEngine;
using System.Collections;

public class SpinHandbag : MonoBehaviour {
	public float speed = .05f;
	private Vector3 startPos;


	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.F)) {
			transform.Rotate (new Vector3 (transform.position.x + speed, 0, 0));
		}

		if (Input.GetKeyUp (KeyCode.F)) {
		}
	}
}
