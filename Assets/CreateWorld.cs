using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateWorld : MonoBehaviour {

	public GameObject person;
	public float numPeople;
	private List<GameObject> people;
	private float score;

	// Use this for initialization
	void Start () {
		people = new List<GameObject> ();

		for (int i = 0; i < numPeople; i++) {
			GameObject p = Instantiate (person, new Vector3 (Random.Range (-50, 50), 2, Random.Range (-50, 50)), Quaternion.identity) as GameObject;
			people.Add(p);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float howManyHit = 0;
		for (int i = 0; i < people.Count; i++) {
			if (people [i].GetComponent<PersonController> ().hasBeenHit) {
				Debug.Log (people [i].GetComponent<PersonController> ());
				howManyHit++;
			}
		}

		score = (howManyHit / numPeople) * 100;
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 20), score.ToString() + "%");
	}
}
