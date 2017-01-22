using UnityEngine;
using System.Collections;

public class SpinItem : MonoBehaviour {

	public string dir;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (dir == "horizontal") {
			transform.Rotate (0, 0, 1);
		} else {
			transform.Rotate (0, 1, 0);
		}
			

	}
}
