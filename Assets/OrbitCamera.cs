using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour {

	public GameObject thingToLookAt;
	private float startingRadius;
	private Transform startingPos;

	public GUIText titleText;

	// Use this for initialization
	void Start () {
		startingPos = transform;
		startingRadius = Vector3.Distance (transform.position, thingToLookAt.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = 2 * (new Vector3 (Mathf.Sin (Time.time*.5f) * startingRadius, startingPos.position.y, Mathf.Cos (Time.time*.5f) * startingRadius));
		transform.LookAt(thingToLookAt.transform);
	}

	void OnGUI() {

	}
}
