using UnityEngine;
using System.Collections;

public class CharacterControllerLogic : MonoBehaviour {

	#region private variables

	// Inspector serialized
	[SerializeField]
	private ThirdPersonCamera camera;
	[SerializeField]
	private float rotationDegreesPerSecond = 120f;
	[SerializeField]
	private float directionSpeed = 3.0f;
	[SerializeField]
	private float directionDampTime = .25f;
	[SerializeField]
	private float speedDampTime = .05f;
	[SerializeField]
	private float speedMultiplier = 10f;
	[SerializeField]
	private float rotationDampTime = .1f;

	// private global only
	private float direction = 0;
	private float charAngle = 0;
	private float horizontal = 0;
	private float vertical = 0;
	private AnimatorStateInfo stateInfo;
	private AnimatorTransitionInfo transInfo;

	Vector3 rotationAmount = Vector3.zero;
	Quaternion deltaRotation = Quaternion.identity;

	private Rigidbody rigidbody;

	private int m_IDLE = 0;
	private int m_WALK = 0;
	private int m_RUN = 0;
	private int m_LocomotionId = 0;
	private int m_LocomotionPivotLId = 0;
	private int m_LocomotionPivotRId = 0;
	private int m_LocomotionPivotLTransId = 0;
	private int m_LocomotionPivotRTransId = 0;

	#endregion


	#region public variables
	public Animator animator;
	public float speed = 0;
	public float LocomotionThreshold = .2f;

	#endregion
	
	#region initialization
	void Awake() {
		animator = GetComponent<Animator> ();
		rigidbody = GetComponent<Rigidbody> ();
	}

	void Start () {


		if (animator.layerCount >= 2) {
			animator.SetLayerWeight(1,1);		
		}

		m_IDLE = Animator.StringToHash("Base Layer.IDLE");
		m_WALK = Animator.StringToHash("Base Layer.WALK");
		m_RUN = Animator.StringToHash("Base Layer.RUN");
		m_LocomotionId = Animator.StringToHash ("Base Layer.Locomotion");
		m_LocomotionPivotLId = Animator.StringToHash ("Base Layer.LocomotionPivotL");
		m_LocomotionPivotRId = Animator.StringToHash ("Base Layer.LocomotionPivotR");
		m_LocomotionPivotLTransId = Animator.StringToHash ("Base Layer.Locomotion -> Base Layer.LocomotionPivotL");
		m_LocomotionPivotRTransId = Animator.StringToHash ("Base Layer.Locomotion -> Base Layer.LocomotionPivotR");

	}
	#endregion


	#region gameloop
	void Update () {
		if (animator && camera.camState != ThirdPersonCamera.CamStates.FirstPerson) {

			stateInfo = animator.GetCurrentAnimatorStateInfo(0);
			transInfo = animator.GetAnimatorTransitionInfo(0);

			// pull values from input device
			horizontal = Input.GetAxis ("Horizontal");
			vertical = Input.GetAxis ("Vertical");

			charAngle = 0f;
			direction = 0f;

			// translate inputs into world space
			StickToWorldSpace(this.transform, camera.transform, ref direction, ref speed, ref charAngle, IsInPivot());
			
			animator.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
			animator.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

			if (speed > LocomotionThreshold){
				if (!IsInPivot()){
					animator.SetFloat("Angle", charAngle);
				}
			}
			if (speed < LocomotionThreshold && Mathf.Abs(horizontal) < 0.1f){
				animator.SetFloat("Direction", 0f);
				animator.SetFloat("Angle", 0f);
			}

//			Debug.Log (Vector3.Cross(this.transform.right, camera.transform.forward).y);

		}
	}

	void FixedUpdate() {
		if (IsInLocomotion ()) {
			deltaRotation = Quaternion.identity;
			// Rotation
			if (((direction >= 0 && horizontal >= 0) || (direction < 0 && horizontal < 0))) {
				// Not facing camera
				Debug.Log ("Not facing camera");
				rotationAmount = Vector3.Lerp (Vector3.zero, new Vector3 (0, rotationDegreesPerSecond * (horizontal < 0 ? -1f : 1f), 0), Mathf.Abs (horizontal));
			} 
		else if (((direction >= 0 && horizontal < 0) || (direction < 0 && horizontal >= 0))) {
				// Facing camera
				Debug.Log ("Facing camera");
				rotationAmount = Vector3.Lerp (Vector3.zero, new Vector3 (0, rotationDegreesPerSecond * (horizontal < 0 ? 1f : -1f), 0), Mathf.Abs (horizontal));
			}
		deltaRotation = Quaternion.Euler (rotationAmount * Time.deltaTime);
//		this.transform.rotation = (this.transform.rotation * deltaRotation);
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.transform.rotation * deltaRotation, Time.deltaTime * rotationAmount.sqrMagnitude);
		}
	}

	void LateUpdate() {

	}
	#endregion


	#region methods

	public void StickToWorldSpace(Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting){
		Vector3 rootDirection = root.forward;

		Vector3 stickDirection = new Vector3 (horizontal, 0, vertical);

		speedOut = stickDirection.sqrMagnitude;

		// get camera rotation
		Vector3 cameraDirection = camera.forward;
		cameraDirection.y = 0; // kill y to stay in x-z-plane
		Quaternion referentialShift = Quaternion.FromToRotation (Vector3.forward, cameraDirection);

		// convert joystick input
		Vector3 moveDirection = referentialShift * stickDirection;
		Vector3 axisSign = Vector3.Cross (moveDirection, rootDirection);

		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), axisSign, Color.red);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
		Debug.DrawRay (new Vector3 (root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);

		float angleRootToMove = Vector3.Angle (rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

		if (!isPivoting) {
			angleOut = angleRootToMove;
		}

		if (axisSign.y == 0)
						angleRootToMove = 0;

		angleRootToMove /= 180f;

		directionOut = angleRootToMove * directionSpeed;
	}

	#endregion


	#region functions

	public bool IsInLocomotion(){
		return stateInfo.nameHash == m_LocomotionId || stateInfo.nameHash == m_WALK || stateInfo.nameHash == m_RUN;
	}

	public bool IsInPivot(){
		return stateInfo.nameHash == m_LocomotionPivotLId ||
				stateInfo.nameHash == m_LocomotionPivotRId ||
				transInfo.nameHash == m_LocomotionPivotLTransId ||
				transInfo.nameHash == m_LocomotionPivotRTransId;
	}

	#endregion

	#region colliders
	#endregion

	#region triggers
	#endregion

	#region coroutines
	#endregion

	#region gizmos
	#endregion
}
