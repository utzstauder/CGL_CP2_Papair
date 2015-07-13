using UnityEngine;
using System.Collections;

public class CaveIntroScript : MonoBehaviour {

	private GameManagerScript gameManagerScript;
	private GameUIControllerScript gameUiControllerScript;

	// Use this for initialization
	void Awake () {
		gameManagerScript = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
		gameUiControllerScript = GameObject.Find ("GameManager").GetComponent<GameUIControllerScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DisplayCutsceneText(string text){
		gameUiControllerScript.DisplayText (text, 2f);
	}
}
