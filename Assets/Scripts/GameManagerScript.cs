using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

	public enum GameState{
		MainMenu,
		Playing,
		Transition,
		Cutscene,
		Paused
	}

//		0: MainMenu
//		1: Cave
//		2: Yard
//		3: Forest
//		4: Testing Grounds
//		5: Lighthouse
//		6: Cockpit
	public enum Level{
		MainMenu,
		Cave,
		Yard,
		Forest,
		TestingGrounds,
		Lighthouse,
		Cockpit
	}
	
	//
	public GameState gameState;

	// Prefabs
	[SerializeField]
	private GameObject playerPrefab;
	
	// References
	[SerializeField]
	private GameObject player;
	[SerializeField]
	private GameObject gameCamera;
	private Canvas gameCanvas;
	private Transform mainMenu;
	private Transform inGameOverlay;
	private Transform inventoryOverlay;
	private Image fadeImage;
	private GameUIControllerScript gameUIcontrollerScript;
	private InventoryScript inventoryScript;

	// Scene transition
	public Level currentScene;
	public Level previousScene;
	[SerializeField]
	private float fadeTime = 1f;
	[SerializeField]
	private float levelSetupTime = 1f;
	private ScreenOverlay screenOverlay;

	// Cutscenes
	private float cutsceneDuration = 5f;

	#region scene variables
	[Header("1-Cave switches")]
	public bool visitedCave = false;
	public bool doorInCaveOpen = false;

	[Header("2-Yard switches")]
	public bool visitedYard = false;

	[Header("3-Forest switches")]
	public bool visitedForest = false;

	[Header("4-TestingGround switches")]
	public bool visitedTestingGround = false;

	[Header("5-Lighthouse switches")]
	public bool visitedLighthouse = false;

	[Header("6-Cockpit switches")]
	public bool visitedCockpit = false;
	#endregion

	// Use this for initialization
	void Awake () {
		// Keep this script and all of its children alive!
		DontDestroyOnLoad (this);

		// Hide the mouse during play.
		Cursor.visible = false;

		gameUIcontrollerScript = GetComponent<GameUIControllerScript> ();
		inventoryScript = GetComponent<InventoryScript> ();
		gameCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		gameCanvas = transform.FindChild ("GameCanvas").GetComponent<Canvas> ();
		mainMenu = gameCanvas.transform.FindChild ("MainMenu");
		inGameOverlay = gameCanvas.transform.FindChild ("InGameOverlay");
		inventoryOverlay = inGameOverlay.FindChild ("InventoryOverlay");
		fadeImage = gameCanvas.transform.FindChild ("FadeImage").GetComponent<Image>();
		screenOverlay = gameCamera.GetComponent<ScreenOverlay> ();

		// if not in debug mode go into the main menu
		if (!player) {
			gameState = GameState.MainMenu;
			currentScene = Level.MainMenu;
		}

		//StartCoroutine (Fade (1f, 0f, fadeTime));
	}
	
	// Update is called once per frame
	void Update () {
	
		// Receive button inputs
		if (gameState == GameState.MainMenu) {
			if (!mainMenu.gameObject.activeSelf) mainMenu.gameObject.SetActive(true);
			if (Input.GetButtonDown ("Button A")) {
				StartGame();
				gameState = GameState.Playing;
			}
			else if (Input.GetButtonDown("Button B")){
				QuitGame();
			}
		}
		if (gameState == GameState.Playing) {
			if (!inGameOverlay.gameObject.activeSelf) inGameOverlay.gameObject.SetActive(true);
		}
		if (gameState == GameState.Playing || gameState == GameState.Paused) {
			if (!inventoryOverlay.gameObject.activeSelf) inventoryOverlay.gameObject.SetActive(true);
		}

	}

	public void loadScene(Level targetScene){
		StartCoroutine(SceneTransition(targetScene));
	}
	
	#region mainmenu

	public void StartGame(){
		StartCoroutine(SceneTransition(Level.Cave));
		StartCoroutine (ToggleGameObjectAfterTime (mainMenu.gameObject, false, fadeTime));
	}

	public void QuitGame(){
		StartCoroutine (Fade (0f, 1f, fadeTime));
		Application.Quit ();
	}

	#endregion

	#region coroutines

	/* Gets called once a scene is loaded
		Use this to place Objects
	*/
	private IEnumerator OnLevelWasLoaded(int targetScene){
		Debug.Log ("Level " + targetScene + " was loaded successfully!");
		Vector3 newPlayerPosition = Vector3.zero;
		Quaternion newPlayerRotation = Quaternion.identity;
		
		GameObject[] SpawnPoints = GameObject.FindGameObjectsWithTag ("SpawnPoint");
		
		// Determine the players new position depending where you came from
		foreach (GameObject spawnPoint in SpawnPoints) {
			if (spawnPoint.transform.parent.GetComponent<SceneTeleporterScript>().targetScene == previousScene){
				newPlayerPosition = spawnPoint.transform.position;
				newPlayerRotation = spawnPoint.transform.rotation;
			}
		}
		
		//
		
		// Warp player to spawn point
		if (player == null) {
			// Spawn player if not already alive
			player = Instantiate (playerPrefab, newPlayerPosition, newPlayerRotation) as GameObject;
		} else {
			player.transform.localPosition = newPlayerPosition;
			player.transform.localRotation = newPlayerRotation;
		}
		
		currentScene = GetLevelFromIndex(targetScene);

		yield return new WaitForSeconds (levelSetupTime);

		// Check if area already visited or game should play cutscene
		switch (targetScene) {
		case 1:
			if (!visitedCave){
				visitedCave = true;
			}
			break;
		case 2:
			if (!visitedYard){
				visitedYard = true;
				GameObject cutsceneObject = GameObject.Find("Cutscene_YardIntro");
				StartCoroutine(PlayCutscene(cutsceneObject, cutsceneDuration));
			}
			break;
		case 3:
			if (!visitedForest){
				visitedForest = true;
			}
			break;
		case 4:
			if (!visitedTestingGround){
				visitedTestingGround = true;
			}
			break;
		case 5:
			if (!visitedLighthouse){
				visitedLighthouse = true;
			}
			break;
		case 6:
			if (!visitedCockpit){
				visitedCockpit = true;
			}
			break;
		}

		StartCoroutine (Fade (1f, 0f, fadeTime));

		yield return new WaitForSeconds (fadeTime);

		if (GetLevelFromIndex (targetScene) != Level.MainMenu && gameState != GameState.Cutscene) {
			gameState = GameState.Playing;
		}
	}


	private IEnumerator Fade(float from, float to, float time){
		for (float t = 0; t <= time; t += Time.deltaTime) {
			fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Lerp (from, to, t/time));
//			screenOverlay.intensity = Mathf.Lerp (to, from, t/time);
			yield return new WaitForEndOfFrame();
		}
		fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, to);
	}

	private IEnumerator SceneTransition(Level nextScene){
		// set state
		gameState = GameState.Transition;

		// keep player walking
		if (player)	player.GetComponent<CharacterControllerLogic> ().speed = .15f;

		// Fade out
		StartCoroutine (Fade (0f, 1f, fadeTime));
		yield return new WaitForSeconds (fadeTime);

		// clear all text
		gameUIcontrollerScript.ClearAll (Time.deltaTime);
		
		// Load new scene
		previousScene = currentScene;
		Application.LoadLevel (GetSceneIndex(nextScene));
	}

	private IEnumerator ToggleGameObjectAfterTime(GameObject target, bool state, float time){
		yield return new WaitForSeconds (time);
		target.SetActive (state);
	}

	private IEnumerator PlayCutscene(GameObject cutsceneObject, float duration){
		// clear inventory overlay
		inventoryScript.Hide ();

		// set camera references
		ThirdPersonCamera camera = gameCamera.GetComponent<ThirdPersonCamera> ();
		camera.cutsceneAnchor = cutsceneObject.transform.FindChild ("CameraAnchor");
		camera.cutsceneTarget = cutsceneObject.transform.FindChild ("CameraTarget");

		// show text
		gameUIcontrollerScript.DisplayAreaText (cutsceneObject.GetComponent<CutsceneScript>().levelName, duration);

		// play cutscene animation
		gameState = GameState.Cutscene;
		cutsceneObject.GetComponent<Animator>().SetBool ("isPlaying", true);

		// wait until its over
		// TODO: right now you have to manually set all the animations to last [duration] seconds
		yield return new WaitForSeconds (duration);

		// show inventory again
		inventoryScript.Show ();
		gameState = GameState.Playing;
	}

	#endregion


	#region functions

	private int GetSceneIndex(Level level){
		int targetSceneIndex = 0;
		switch (level){
		case GameManagerScript.Level.MainMenu: targetSceneIndex = 0;
			break;
		case GameManagerScript.Level.Cave: targetSceneIndex = 1;
			break;
		case GameManagerScript.Level.Yard: targetSceneIndex = 2;
			break;
		case GameManagerScript.Level.Forest: targetSceneIndex = 3;
			break;
		case GameManagerScript.Level.TestingGrounds: targetSceneIndex = 4;
			break;
		case GameManagerScript.Level.Lighthouse: targetSceneIndex = 5;
			break;
		case GameManagerScript.Level.Cockpit: targetSceneIndex = 6;
			break;
		default: break;
		}
		return targetSceneIndex;
	}

	private Level GetLevelFromIndex(int sceneIndex){
		Level level = Level.MainMenu;
		switch (sceneIndex){
		case 0: level = GameManagerScript.Level.MainMenu;
			break;
		case 1: level = GameManagerScript.Level.Cave;
			break;
		case 2: level = GameManagerScript.Level.Yard;
			break;
		case 3: level = GameManagerScript.Level.Forest;
			break;
		case 4 : level = GameManagerScript.Level.TestingGrounds;
			break;
		case 5: level = GameManagerScript.Level.Lighthouse;
			break;
		case 6: level = GameManagerScript.Level.Cockpit;
			break;
		default: break;
		}
		return level;
	}

	#endregion

}
