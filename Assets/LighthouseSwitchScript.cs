using UnityEngine;
using System.Collections;

public class LighthouseSwitchScript : MonoBehaviour {

	[SerializeField]
	[Range(1,3)]
	private int switchId = 0;

	private GameManagerScript gameMangagerScript;
	private Animator animator;
	private AudioSource audioSource;
	public bool activated = false;

	// Use this for initialization
	void Awake () {
		if (switchId == 0)
			Debug.LogError ("The switchId must be either 1 or 2"); 

		gameMangagerScript = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
		animator = GetComponent<Animator> ();
		audioSource = GetComponent<AudioSource> ();

		// set active if already activated
		if ((switchId == 1 && gameMangagerScript.activatedSwitch1) || (switchId == 2 && gameMangagerScript.activatedSwitch2) || (switchId == 3 && gameMangagerScript.activatedSwitch)) {
			activated = true;
			animator.SetBool("Activated", activated); 
		}
	}

	public void OnKick(){
		if (!activated) {

			if (!(switchId == 3 && !gameMangagerScript.placedEnergycore)){
			audioSource.Play();
			activated = true;
			animator.SetBool("Activated", activated);
			gameMangagerScript.SetSwitch(switchId);
			}

			if (switchId == 1)
				gameMangagerScript.PlayCutsceneExternal(GameObject.Find ("Cutscene_LighthouseSwitch1"), 4f);
			else if (switchId == 2)
				gameMangagerScript.PlayCutsceneExternal(GameObject.Find ("Cutscene_LighthouseSwitch2"), 4f);
		}
	}
}
