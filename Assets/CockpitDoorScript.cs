using UnityEngine;
using System.Collections;

public class CockpitDoorScript : MonoBehaviour {

	[SerializeField]
	private Animator doorAnimatorLeft;
	[SerializeField]
	private Animator doorAnimatorRight;

	// Use this for initialization
	void Awake () {
	}
	
	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			doorAnimatorLeft.SetBool("Open", true);
			doorAnimatorRight.SetBool("Open", true);
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player") {
			doorAnimatorLeft.SetBool("Open", false);
			doorAnimatorRight.SetBool("Open", false);
		}
	}
}
