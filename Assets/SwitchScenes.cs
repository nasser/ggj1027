using UnityEngine;
using System.Collections;

public class SwitchScenes : MonoBehaviour {

	public string nextScene;
	public string triggerKey;
	public int fadeOutTimer;
	private bool isFadingOut;

	// Use this for initialization
	void Start () {
		isFadingOut = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (triggerKey)) {
			UnityEditor.SceneManagement.EditorSceneManager.LoadScene (nextScene);
		}
	}
}
