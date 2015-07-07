using UnityEngine;
using System.Collections;

public class DisableInPlaymode : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		this.gameObject.SetActive(false);
	}

}
