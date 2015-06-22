using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour {

	[SerializeField]
	private ThirdPersonCamera camera;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt (camera.transform);
	}
}
