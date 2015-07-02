using UnityEngine;
using System.Collections;

public class UpAndDown : MonoBehaviour {

	[SerializeField]
	private float range = 2f;
	[SerializeField]
	private float speed = 10f;

	private Vector3 initialPosition;
	private float newY;

	// Use this for initialization
	void Start () {
		initialPosition = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		newY = initialPosition.y + Mathf.Sin (Time.deltaTime * speed) * range;
		this.transform.localPosition = new Vector3 (initialPosition.x, newY, initialPosition.z);
	}
}
