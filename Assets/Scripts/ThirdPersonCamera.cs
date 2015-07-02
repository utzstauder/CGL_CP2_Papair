using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

struct CameraPosition{
	// Position to align camera to
	private Vector3 position;

	// Transform used for any rotation
	private Transform xForm;

	public Vector3 Position { get { return position; } set { position = value; } }
	public Transform XForm { get { return xForm; } set { xForm = value; } }

	public void Init(string camName, Vector3 pos, Transform transform, Transform parent){
		position = pos;
		xForm = transform;
		xForm.name = camName;
		xForm.parent = parent;
		xForm.localPosition = Vector3.zero;
		xForm.localPosition = position; 
	}

}

[RequireComponent(typeof(BarsEffect))]
public class ThirdPersonCamera : MonoBehaviour {

	#region private variables
	[SerializeField]
	private Transform parentRig;
	[SerializeField]
	private float distanceAway;
	[SerializeField]
	private float distanceAwayMultiplier = 1.5f;
	[SerializeField]
	private float distanceUp;
	[SerializeField]
	private float distanceUpMultiplier = 5f;
	[SerializeField]
	private float smooth;
	[SerializeField]
	public Transform followTransform;
	[SerializeField]
	public CharacterControllerLogic follow;
	[SerializeField]
	private float widescreen = .2f;
	[SerializeField]
	private float targetingTime = .5f;
	[SerializeField]
	private float firstPersonThreshold = .5f;
	[SerializeField]
	private float fpsRotationDegreePerSecond = 30f;
	[SerializeField]
	private float freeThreshold = -.1f;
	[SerializeField]
	private Vector2 camMinDistFromChar = new Vector2 (1f, -.5f);
	[SerializeField]
	private float rightStickThreshold = .1f;
	[SerializeField]
	private const float freeRotationDegreePerSecond = -5f;

	// private global only
	private Vector3 lookDirection;
	private Vector3 currentLookDirection;
	private Vector3 targetPosition;
	private Vector3 targetDirection = Vector3.zero;
	private BarsEffect barEffect;
	private float xAxisRot = 0f;
	private CameraPosition firstPersonCamPos;
	//private float lookWeight = 0f;
	[SerializeField]
	private float firstPersonLookSpeed = 1.5f;
	[SerializeField]
	private Vector2 firstPersonXAxisClamp = new Vector2 (-70f, 90f);
	private Vector3 savedRigToGoal;
	private float distanceAwayFree;
	private float distanceUpFree;
	private Vector2 rightStickPrevFrame = Vector2.zero;
	
	// smoothing and damping
	private Vector3 velocityCamSmooth = Vector3.zero;
	[SerializeField]
	private float camSmoothDampTime = .1f;
	private float initialSmoothDampTime;
	private Vector3 velocityLookDirection = Vector3.zero;
	[SerializeField]
	private float lookDirectionDampTime = .1f;

	// image effects
	private DepthOfField dof;
	private float initialFocalLength;
	private float initialFocalSize;
	private float initialAperture;

	// references
	private GameManagerScript gameManager;

	#endregion


	#region public variables

	public enum CamStates{
		Behind,
		FirstPerson,
		Target,
		Free,
		FixedPosition,
		Menu
	}

	public CamStates camState = CamStates.Behind;
	
	#endregion


	#region initialization
	void Awake() {
		DontDestroyOnLoad (this.transform.parent.gameObject);
		gameManager = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
	//	followTransform = GameObject.FindWithTag ("Player").transform;
	//	follow = followTransform.parent.GetComponent<CharacterControllerLogic>();
	}

	void Start () {
		initialSmoothDampTime = camSmoothDampTime;
		dof = GetComponent<DepthOfField> ();
		initialFocalLength = dof.focalLength;
		initialFocalSize = dof.focalSize;
		initialAperture = dof.aperture;

		parentRig = this.transform; // .parent
		if (parentRig == null){
			Debug.LogError("Parent camera to empty GameObject.", this);
		}

		lookDirection = followTransform.forward;
		currentLookDirection = followTransform.forward;

		barEffect = GetComponent<BarsEffect> ();
		if (barEffect == null) {
			Debug.LogError("Attach a widescreen BarsEffect script to the camera.", this);		
		}

		// Position and parent a GameObject where first person view should be
		firstPersonCamPos = new CameraPosition ();
		firstPersonCamPos.Init (
			"First Person Camera",
			new Vector3 (0f, 0.3f, 0.32f),
			new GameObject ().transform,
			followTransform.transform
		);

		// Set default values for first free look
		distanceAwayFree = distanceAway;
		distanceUpFree = distanceUp;
	}
	#endregion


	#region gameloop
	void Update () {
	
	}

	void FixedUpdate() {

	}

	void LateUpdate() {
		// Pull values from controller/keyboard
		float rightX = Input.GetAxis ("C-Stick Horizontal");
		float rightY = Input.GetAxis ("C-Stick Vertical");
		float leftX = Input.GetAxis ("Horizontal");
		float leftY = Input.GetAxis ("Vertical");

		Vector3 characterOffset = followTransform.position + new Vector3(0f, distanceUp, 0f);
		Vector3 lookAt = characterOffset;
		Vector3 targetPosition = Vector3.zero;

		// Determine camera state
		// Fixed Position
		if (follow.cameraAnchorTransform != null && camState != CamStates.FirstPerson) {
			camState = CamStates.FixedPosition;

			// First Person View also possible while in Fixed Position
			if (rightY > firstPersonThreshold && camState != CamStates.Free && !follow.IsInLocomotion()){
				// Reset look before entering the first person mode
				xAxisRot = 0;
				//lookWeight = 0;
				camState = CamStates.FirstPerson;
			}

			// Remove barsEffect
			barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, 0f, targetingTime);

		}
		else if (Input.GetButton ("Button L")) { // Target
			// set initial direction when entering target mode
			if (targetDirection == Vector3.zero) targetDirection = followTransform.forward;

				barEffect.coverage = Mathf.SmoothStep (barEffect.coverage, widescreen, targetingTime);

				camState = CamStates.Target;
			} else {
			barEffect.coverage = Mathf.SmoothStep(barEffect.coverage, 0f, targetingTime);
			targetDirection = Vector3.zero;

			// First Person View
			if (rightY > firstPersonThreshold && camState != CamStates.Free && !follow.IsInLocomotion() && camState != CamStates.FixedPosition){
				// Reset look before entering the first person mode
				xAxisRot = 0;
				//lookWeight = 0;
				camState = CamStates.FirstPerson;
			}

			// Free camera
			if (rightY < freeThreshold || rightX > -1f * freeThreshold || rightX < freeThreshold && camState != CamStates.FirstPerson && camState != CamStates.FixedPosition){
				camState = CamStates.Free;
				savedRigToGoal = Vector3.zero;
			}

			// Behind
			if ( (camState == CamStates.FirstPerson && Input.GetButtonDown("Button B")) ||
			    (camState == CamStates.Target && !Input.GetButton("Button L")) ||
			    follow.cameraAnchorTransform == null && camState != CamStates.Free && camState != CamStates.FirstPerson){
				camState = CamStates.Behind;
			}

			// Menu
			if (gameManager.gameState == GameManagerScript.GameState.MainMenu){

			}
		}

		//follow.animator.SetLookAtWeight (lookWeight);

		// Execute camera state
		switch (camState) {
			case CamStates.Behind:
				ResetCamera();

			// Only update camera look when moving
			if (follow.speed > follow.LocomotionThreshold && follow.IsInLocomotion()){
				lookDirection = Vector3.Lerp (followTransform.right * (leftX < 0 ? 1f : -1f), followTransform.forward * (leftY < 0 ? -1f : 1f), Mathf.Abs (Vector3.Dot (this.transform.forward, followTransform.forward)));

				// Calculate direction from camera to player
				currentLookDirection = Vector3.Normalize(characterOffset - this.transform.position);
				currentLookDirection.y = 0;

				// Damping makes it so we don't update targetPosition while pivoting; camera shouldn't rotate around player
				currentLookDirection = Vector3.SmoothDamp(currentLookDirection, lookDirection, ref velocityLookDirection, lookDirectionDampTime);
			}

			targetPosition = characterOffset + followTransform.up * distanceUp - Vector3.Normalize(currentLookDirection) * distanceAway;
			break;

			case CamStates.Target:
				ResetCamera();
				camSmoothDampTime = 0.05f;
				lookDirection = targetDirection;
				targetPosition = characterOffset + followTransform.up * distanceUp - lookDirection * distanceAway;

			break;

			case CamStates.FirstPerson:
				dof.focalTransform = null;
				dof.focalLength = 6f;
				dof.focalSize = 8f;
				dof.aperture = 20f;

				// Looking up and down
				// Calculate the amount of rotation and apply to the firstPersonCamPos GameObject
				xAxisRot -= (leftY * firstPersonLookSpeed);
				xAxisRot = Mathf.Clamp(xAxisRot, firstPersonXAxisClamp.x, firstPersonXAxisClamp.y);
				firstPersonCamPos.XForm.localRotation = Quaternion.Euler(xAxisRot, 0, 0);

				// Superimpose firstPersonCamPos GameObjects's rotation on camera
				Quaternion rotationShift = Quaternion.FromToRotation(this.transform.forward, firstPersonCamPos.XForm.forward);
				this.transform.rotation = rotationShift * this.transform.rotation;

				//TODO: not working
				// Move character model's head
				follow.animator.SetLookAtPosition(firstPersonCamPos.XForm.position + firstPersonCamPos.XForm.forward);
				//lookWeight = Mathf.Lerp (lookWeight, 1f, Time.deltaTime * firstPersonLookSpeed);

				// Looking left and right
				Vector3 rotationAmount = Vector3.Lerp (Vector3.zero,new Vector3(0, fpsRotationDegreePerSecond * (leftX < 0 ? -1f : 1f), 0), Mathf.Abs(leftX));
				Quaternion deltaRotation = Quaternion.Euler (rotationAmount * Time.deltaTime);
				follow.transform.rotation = follow.transform.rotation * deltaRotation;

				// Move camera to firstPersonCamPos
				targetPosition = firstPersonCamPos.XForm.position;

				// Smoothly transition look direction towards firstPersonCamPos when entering first person mode
				lookAt = Vector3.Lerp (targetPosition + followTransform.forward, this.transform.position + this.transform.forward, camSmoothDampTime * Time.deltaTime);

				// Choose lookAt Target based on distance
				lookAt = Vector3.Lerp (this.transform.position + this.transform.forward, lookAt, Vector3.Distance(this.transform.position,firstPersonCamPos.XForm.position));

				break;

			case CamStates.Free:
				//lookWeight = Mathf.Lerp (lookWeight, 0, Time.deltaTime * firstPersonLookSpeed);

				// Move height and distance in separate parentRig transform
				Vector3 rigToGoalDirection = Vector3.Normalize(characterOffset - this.transform.position);
				rigToGoalDirection.y = 0f;

				Vector3 rigToGoal = characterOffset - parentRig.position;
				rigToGoal.y = 0f;
				Debug.DrawRay(parentRig.transform.position, rigToGoal, Color.red);

				// Moving camera in and out
				// If statement works for positive values; don't tween if stick not increasing in either direction; also don't tween if user is rotating
				// Checked against RIGHT X THRESHOLD because very small calues for rightY mess up the Lerp function
				if (rightY < -1f * rightStickThreshold && rightY <= rightStickPrevFrame.y /* && Mathf.Abs (rightX) < rightStickThreshold */){
					distanceUpFree = Mathf.Lerp(distanceUp, distanceUp * distanceUpMultiplier, Mathf.Abs (rightY));
					distanceAwayFree = Mathf.Lerp (distanceAway, distanceAway * distanceAwayMultiplier, Mathf.Abs (rightY));
					targetPosition = characterOffset + followTransform.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;
				} else if (rightY > rightStickThreshold && rightY >= rightStickPrevFrame.y /* && Mathf.Abs (rightX) < rightStickThreshold */){
					// Subtract height of camera from height of player to find Y distance
					distanceUpFree = Mathf.Lerp (Mathf.Abs (transform.position.y - characterOffset.y), camMinDistFromChar.y, Mathf.Abs (rightY));
					// Use magnitude function to find X distance
					distanceAwayFree = Mathf.Lerp (rigToGoal.magnitude, camMinDistFromChar.x, Mathf.Abs (rightY));

					targetPosition = characterOffset + followTransform.up * distanceUpFree - rigToGoalDirection * distanceAwayFree;
				}

				// Store direction only if right stick inactive
				if (rightX != 0 || rightY != 0){
					savedRigToGoal = rigToGoalDirection;
				}

				parentRig.RotateAround(characterOffset, followTransform.up, freeRotationDegreePerSecond * (Mathf.Abs (rightX) > rightStickThreshold ? rightX : 0f));

				// Track camera behind player
				if (targetPosition == Vector3.zero){
					targetPosition = characterOffset + followTransform.up * distanceUpFree - savedRigToGoal * distanceAwayFree;
				}

		//		SmoothPosition(transform.position, targetPosition);
		//		transform.LookAt(lookAt);

				break;

			case CamStates.FixedPosition:
				ResetCamera();
				camSmoothDampTime = 0.1f;
				targetPosition = follow.cameraAnchorTransform.position;
				break;

		default: break;
		}

	//	if (camState != CamStates.Free) {
			CompensateForWalls (characterOffset, ref targetPosition);

			SmoothPosition (this.transform.position, targetPosition);

			// look at the target
			transform.LookAt (lookAt);
	//	}

		rightStickPrevFrame = new Vector2 (rightX, rightY);
	}
	#endregion


	#region methods

	private void SmoothPosition(Vector3 fromPos, Vector3 toPos){
		this.transform.position = Vector3.SmoothDamp (fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
	}

	private void CompensateForWalls (Vector3 fromObject, ref Vector3 toTarget){
		Debug.DrawLine (fromObject, toTarget, Color.cyan);
		// Compensate for walls between camera
		RaycastHit wallHit = new RaycastHit ();
		if (Physics.Linecast(fromObject,toTarget, out wallHit)){
			Debug.DrawRay(wallHit.point, Vector3.left, Color.red);
			toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z);
		}
	}

	private void ResetCamera(){
		dof.focalTransform = followTransform;
		dof.focalLength = initialFocalLength;
		dof.focalSize = initialFocalSize;
		dof.aperture = initialAperture;

		camSmoothDampTime = initialSmoothDampTime;
		//lookWeight = Mathf.Lerp (lookWeight, 0, Time.deltaTime * firstPersonLookSpeed);
		transform.localRotation = Quaternion.Lerp (transform.localRotation, Quaternion.identity, Time.deltaTime);

		lookDirection = followTransform.forward;
		currentLookDirection = followTransform.forward;
	}


	//TODO: machen machen
	private void EnableEffects(){

	}

	#endregion


	#region functions
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
