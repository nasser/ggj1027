using UnityEngine;
using System.Collections;

public class StartWorld : MonoBehaviour {

	public GameObject jerk;
	public int numJerks;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < numJerks; i++) {
			Instantiate (jerk, new Vector3 (Random.Range (-250, -220), 0, Random.Range (-250, -220)), Quaternion.identity);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
