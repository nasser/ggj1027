using UnityEngine;
using System.Collections;

public class OrbitCamera : MonoBehaviour {

	public GameObject thingToLookAt;
	public float startingRadius;
	public Vector3 startingPos;

	public GUIText titleText;

	// Use this for initialization
	void Start () {
		startingPos = transform.position;
		startingRadius = Vector3.Distance (transform.position, thingToLookAt.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = 2 * (new Vector3 (Mathf.Sin (Time.time*.5f) * startingRadius, startingPos.y, Mathf.Cos (Time.time*.5f) * startingRadius));
		transform.LookAt(thingToLookAt.transform);
	}

	void OnGUI() {

	}
}
