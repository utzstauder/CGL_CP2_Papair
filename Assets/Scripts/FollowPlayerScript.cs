using UnityEngine;
using System.Collections;

public class FollowPlayerScript : MonoBehaviour {

	[SerializeField]
	private Transform followTarget;
	[SerializeField]
	private float speed = 1f;
	[SerializeField]
	private float minDistanceToTarget = 3f;

	private Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Vector3.Distance(this.transform.position, followTarget.position) >= minDistanceToTarget)
			rigidbody.velocity = (followTarget.position - this.transform.position).normalized * speed;
	}
	
}
