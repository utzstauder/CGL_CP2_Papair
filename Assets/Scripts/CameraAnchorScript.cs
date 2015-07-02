using UnityEngine;
using System.Collections;

public class CameraAnchorScript : MonoBehaviour {
	
	public Transform cameraAnchorTransform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerStay(Collider other){
		if (other.gameObject.tag == "Player") {
			if (other.GetComponent<CharacterControllerLogic>().cameraAnchorTransform == null)
				other.GetComponent<CharacterControllerLogic>().cameraAnchorTransform = this.cameraAnchorTransform;
		}
	}
}
