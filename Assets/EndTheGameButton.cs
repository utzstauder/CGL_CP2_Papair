using UnityEngine;
using System.Collections;

public class EndTheGameButton : MonoBehaviour {

	private GameManagerScript gameManagerScript;
	private GameUIControllerScript gameUiControllerScript;

	[SerializeField]
	private Animator cockpitWindowAnimator;

	// Use this for initialization
	void Awake () {
		gameManagerScript = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
		gameUiControllerScript = GameObject.Find ("GameManager").GetComponent<GameUIControllerScript> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnGrab(){
		if (gameManagerScript.placedEnergycore && gameManagerScript.activatedSwitch){
			// end the game
			cockpitWindowAnimator.SetBool("Open", true);
			gameManagerScript.StartOutro(3f);
		} else if (gameManagerScript.placedEnergycore) {
		gameUiControllerScript.DisplayText("There is still no power.", 2f);
		} else {
			gameUiControllerScript.DisplayText("There is no power.", 2f);
		}
	}
}
