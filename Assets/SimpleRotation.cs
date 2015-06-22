using UnityEngine;
using System.Collections;

public class SimpleRotation : MonoBehaviour {

	[SerializeField]
	private float speed = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		this.transform.Rotate (0, speed * Time.deltaTime, 0);

	}
}
