using UnityEngine;
using System.Collections;

public class SimpleRotation : MonoBehaviour {

	[SerializeField]
	private Vector3 rotationSpeed = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		this.transform.Rotate (rotationSpeed.x * Time.deltaTime, rotationSpeed.y * Time.deltaTime, rotationSpeed.z * Time.deltaTime);

	}
}
