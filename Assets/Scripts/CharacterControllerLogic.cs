using UnityEngine;
using System.Collections;

public class CharacterControllerLogic : MonoBehaviour {

	#region private variables

	// Inspector serialized
	[SerializeField]
	public ThirdPersonCamera camera;
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

	[Header("For the KICKS!")]
	[SerializeField]
	private LayerMask kickableLayers;
	[SerializeField]
	private Transform kickArea;
	[SerializeField]
	private float kickRadius = .35f;
	[SerializeField]
	private float kickForce = 10f;

	[Header("Grab everything!")]
	[SerializeField]
	private LayerMask grabbableLayers;
	[SerializeField]
	private Transform grabArea;
	[SerializeField]
	private float grabRadius = .35f;

	[Header("Catch your breath!")]
	[SerializeField]
	private float runThreshold = 10f;
	[SerializeField]
	private float restThreshold = 3f;

	// private global only
	private float direction = 0;
	private float charAngle = 0;
	private float horizontal = 0;
	private float vertical = 0;
	private AnimatorStateInfo stateInfo;
	private AnimatorTransitionInfo transInfo;
	private float timeRunning = 0;
	private float timeResting = 0;

	Vector3 rotationAmount = Vector3.zero;
	Quaternion deltaRotation = Quaternion.identity;

	private Rigidbody rigidbody;

	private GameManagerScript gameManager;
	private GameUIControllerScript gameUIcontroller;
	private InventoryScript inventoryScript;

	private int m_IDLE = 0;
	private int m_BREATHING = 0;
	private int m_WALK = 0;
	private int m_RUN = 0;
	private int m_KICK = 0;
	private int m_KICK_RUN = 0;
	private int m_GRAB = 0;
	private int m_GRAB_RUN = 0;

	#endregion


	#region public variables
	public Animator animator;
	public float speed = 0;
	public float LocomotionThreshold = .2f;
	public Transform cameraAnchorTransform;

	#endregion
	
	#region initialization
	void Awake() {
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
		gameUIcontroller = gameManager.gameObject.GetComponent<GameUIControllerScript> ();
		inventoryScript = gameManager.gameObject.GetComponent<InventoryScript> ();

		DontDestroyOnLoad (this.gameObject);

		camera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<ThirdPersonCamera>();
		camera.follow = this;
		camera.followTransform = this.transform.FindChild ("follow").transform;
		camera.enabled = true;


		animator = GetComponent<Animator> ();
		rigidbody = GetComponent<Rigidbody> ();
	}

	void Start () {


		if (animator.layerCount >= 2) {
			animator.SetLayerWeight(1,1);		
		}

		m_IDLE = Animator.StringToHash("Base Layer.IDLE");
		m_BREATHING = Animator.StringToHash("Base Layer.BREATHING");
		m_WALK = Animator.StringToHash("Base Layer.WALK");
		m_RUN = Animator.StringToHash("Base Layer.RUN");
		m_KICK = Animator.StringToHash("Base Layer.KICK");
		m_KICK_RUN = Animator.StringToHash("Base Layer.KICK_RUN");
		m_GRAB = Animator.StringToHash("Base Layer.GRAB");
		m_GRAB_RUN = Animator.StringToHash("Base Layer.GRAB_RUN");


	}
	#endregion


	#region gameloop
	void Update () {
		// update only when playing
		if (gameManager.gameState == GameManagerScript.GameState.Playing) {
			if (animator && camera.camState != ThirdPersonCamera.CamStates.FirstPerson) {

				stateInfo = animator.GetCurrentAnimatorStateInfo (0);
				transInfo = animator.GetAnimatorTransitionInfo (0);

				// pull values from input device
				horizontal = Input.GetAxis ("Horizontal");
				vertical = Input.GetAxis ("Vertical");

				charAngle = 0f;
				direction = 0f;

				// translate inputs into world space
				StickToWorldSpace (this.transform, camera.transform, ref direction, ref speed, ref charAngle);
			
				animator.SetFloat ("Speed", speed, speedDampTime, Time.deltaTime);
				animator.SetFloat ("Direction", direction, directionDampTime, Time.deltaTime);

				if (speed > LocomotionThreshold) {
					animator.SetFloat ("Angle", charAngle);
				}
				if (speed < LocomotionThreshold && Mathf.Abs (horizontal) < 0.1f) {
					animator.SetFloat ("Direction", 0f);
					animator.SetFloat ("Angle", 0f);
				}

//			Debug.Log (Vector3.Cross(this.transform.right, camera.transform.forward).y);

				// Handle other button inputs
				if (Input.GetButtonDown ("Button B"))
					animator.SetBool ("Kick", true);
				if (Input.GetButtonDown ("Button A"))
					animator.SetBool ("Grab", true);

				// Stop kick at end of animation
				if (IsKicking ())
					animator.SetBool ("Kick", false);
				if (IsGrabbing ())
					animator.SetBool ("Grab", false);
			}

			// Run and rest logic
			if (IsRunning ()) {
				timeResting = 0;
				timeRunning += Time.deltaTime;
				if (timeRunning >= runThreshold)
					animator.SetBool ("OutOfBreath", true);
			}
			if (IsResting ()) {
				timeRunning = 0;
				timeResting += Time.deltaTime;
				if (timeResting >= restThreshold)
					animator.SetBool ("OutOfBreath", false);
			}
		} else if (gameManager.gameState == GameManagerScript.GameState.Transition) {
			// keep walking straight
			animator.SetFloat("Speed", speed);
			animator.SetFloat("Direction", 0f);
			animator.SetFloat("Angle", 0f);
		}
	}

	void FixedUpdate() {
		// only update when playing
		if (gameManager.gameState == GameManagerScript.GameState.Playing) {
			// Only rotate when moving
			if (IsInLocomotion ()) {
				// Reset everything first
				deltaRotation = Quaternion.identity;
				rotationAmount = Vector3.zero;

				// Calculate rotation
				if (((direction > 0.1f && horizontal > 0) || (direction < -0.1f && horizontal < 0))) {
					// Not facing camera
					rotationAmount = Vector3.Lerp (Vector3.zero, new Vector3 (0, rotationDegreesPerSecond * (horizontal < 0 ? -1f : 1f), 0), Mathf.Abs (horizontal));
				} else if (((direction > 0.1f && horizontal < 0) || (direction < -0.1f && horizontal > 0))) {
					// Facing camera
					rotationAmount = Vector3.Lerp (Vector3.zero, new Vector3 (0, rotationDegreesPerSecond * (horizontal < 0 ? 1f : -1f), 0), Mathf.Abs (horizontal));
				} else if (Mathf.Abs (direction) > 0.1f) {
					// Turn around
					rotationAmount = Vector3.Lerp (Vector3.zero, new Vector3 (0, rotationDegreesPerSecond * (direction < 0 ? -1f : 1f), 0), Mathf.Abs (vertical));
				}
				// Rotate!
				deltaRotation = Quaternion.Euler (rotationAmount * Time.deltaTime);
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, this.transform.rotation * deltaRotation, Time.deltaTime * rotationAmount.sqrMagnitude);
			}
		}
	}

	void LateUpdate() {

	}
	#endregion


	#region methods

	public void StickToWorldSpace(Transform root, Transform camera, ref float directionOut, ref float speedOut, ref float angleOut){
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

		angleOut = angleRootToMove;

		if (axisSign.y == 0)
						angleRootToMove = 0;

		angleRootToMove /= 180f;

		directionOut = angleRootToMove * directionSpeed;
	}

	public void Kick(){
		RaycastHit[] sphereCast = Physics.SphereCastAll(kickArea.position, kickRadius, Vector3.one * kickRadius, kickableLayers);
		if (sphereCast.Length > 0)
			foreach (RaycastHit hit in sphereCast){
				if (hit.transform.gameObject.GetComponent<Rigidbody>() && hit.transform.gameObject.tag != "Player"){
					Rigidbody hitRigidbody = hit.transform.gameObject.GetComponent<Rigidbody>();
					hitRigidbody.AddForce((hit.transform.position - this.transform.position).normalized * kickForce, ForceMode.Impulse);

					GetComponent<CharacterAudioControllerScript> ().KickPlayAudio (); // Play audio
				}
				
				if (hit.transform.gameObject.GetComponent<LockedDoorScript>()){
				hit.transform.gameObject.GetComponent<LockedDoorScript>().OnKick();
				}
			}
//		if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, kickableLayers)){
//			if (hit.distance <= kickReach && hit.transform.GetComponent<Rigidbody>()){
//				// Kick
//				Debug.Log("Raycast hit " + hit.transform.gameObject.name);
//				Rigidbody hitRigidbody = hit.transform.GetComponent<Rigidbody>();
//				hitRigidbody.AddForce((hit.transform.position - this.transform.position).normalized * kickForce, ForceMode.Impulse);
//				Debug.Log("Kick!");
//			}
//		}
	}

	public void Grab(){
		RaycastHit[] sphereCast = Physics.SphereCastAll(grabArea.position, grabRadius, Vector3.one * grabRadius, grabbableLayers);
		if (sphereCast.Length > 0)
		foreach (RaycastHit hit in sphereCast){
			if (hit.transform.gameObject.GetComponent<CollectibleItemScript>() && hit.transform.gameObject.tag != "Player"){
				Debug.Log ("Grabbing something!");
				inventoryScript.AddItem(hit.transform.gameObject.GetComponent<CollectibleItemScript>().itemType);
				Destroy (hit.transform.gameObject);
				 // TODO: Play audio?
			}

			if (hit.transform.gameObject.GetComponent<LockedDoorScript>()){
				hit.transform.gameObject.GetComponent<LockedDoorScript>().OnGrab();
			}
		}
	}

	#endregion


	#region functions

	public bool IsInLocomotion(){
		return stateInfo.nameHash == m_WALK || stateInfo.nameHash == m_RUN || stateInfo.nameHash == m_KICK_RUN;
	}

	public bool IsRunning(){
		return stateInfo.nameHash == m_RUN || stateInfo.nameHash == m_KICK_RUN || stateInfo.nameHash == m_GRAB_RUN;
	}

	public bool IsResting(){
		return stateInfo.nameHash == m_BREATHING;
	}

	public bool IsKicking(){
		return stateInfo.nameHash == m_KICK || stateInfo.nameHash == m_KICK_RUN;
	}

	public bool IsGrabbing(){
		return stateInfo.nameHash == m_GRAB || stateInfo.nameHash == m_GRAB_RUN;
	}


	#endregion

	#region colliders
	#endregion

	#region triggers
	public void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "CameraAnchor" && gameManager.gameState == GameManagerScript.GameState.Playing)
			if (other.GetComponent<CameraAnchorScript>().cameraAnchorTransform == this.cameraAnchorTransform)
				this.cameraAnchorTransform = null;
	}

	public void OnTriggerEnter(Collider other){
		if (other.GetComponent<TextAreaTriggerScript> ()) {
			TextAreaTriggerScript textScript = other.GetComponent<TextAreaTriggerScript> ();
			gameUIcontroller.DisplayText(textScript.text, textScript.time);
		}
	}

	#endregion

	#region coroutines
	#endregion

	#region gizmos
	#endregion
}
