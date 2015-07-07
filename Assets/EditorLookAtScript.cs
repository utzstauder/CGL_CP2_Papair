using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorLookAtScript : MonoBehaviour {

	public Transform cameraTarget;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.LookAt(cameraTarget);
	}
}
