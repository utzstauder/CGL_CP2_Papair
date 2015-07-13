using UnityEngine;
using System.Collections;

public class WaterpumpButtonScript : MonoBehaviour {
	
	private Animator animator;

	// Use this for initialization
	void Awake () {
		animator = transform.parent.GetComponent<Animator> ();
	}

	public void OnKick(){
		animator.SetBool ("Activated", true);
	}
}
