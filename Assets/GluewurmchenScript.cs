using UnityEngine;
using System.Collections;

public class GluewurmchenScript : MonoBehaviour {

	[SerializeField]
	private Vector3 speed = Vector3.zero;
	[SerializeField]
	private Vector3 amplitude = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.localPosition = new Vector3 (amplitude.x * Mathf.Sin (Time.time * speed.x), amplitude.y * Mathf.Sin (Time.time * speed.y), amplitude.z * Mathf.Sin (Time.time * speed.z));
	}
}
