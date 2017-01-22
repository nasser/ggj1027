using UnityEngine;
using System.Collections;

public class SetTitleBG : MonoBehaviour {
	public Texture2D textureToDisplay;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void OnGUI() {
		GUI.Label(new Rect(-Screen.width/2, Screen.height/2 - 70, 100000, 100000), textureToDisplay);
	}
}
