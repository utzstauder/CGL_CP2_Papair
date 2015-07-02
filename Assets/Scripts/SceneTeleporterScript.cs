using UnityEngine;
using System.Collections;

public class SceneTeleporterScript: MonoBehaviour {
	
	public GameManagerScript.Level targetScene;
	[HideInInspector]
	private GameManagerScript gameManager;

	// Use this for initialization
	void Awake () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManagerScript> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/* 
		0: MainMenu
		1: Cave
		2: Yard
		3: Forest
		4: Testing Grounds
		5: Lighthouse
		6: Cockpit
	*/
	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Player") {
			Debug.Log ("Player entered the loading zone.");
			gameManager.loadScene(targetScene);
		}
	}
}
