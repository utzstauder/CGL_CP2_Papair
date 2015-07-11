using UnityEngine;
using System.Collections;

public class CharacterAudioControllerScript : MonoBehaviour {
	
	// private global only
	private AudioSource audioSource;

	/*
	 * 0 = none
	 * 1 = grass
	 * 2 = rock
	 */
	public int currentSurfaceType = 2;

	[SerializeField]
	[Range(0,1f)]
	private float footStepVolume = .3f;
	[SerializeField]
	[Range(0,1f)]
	private float hummingVolume = .2f;
	[SerializeField]
	[Range(0,1f)]
	private float breathingVolume = .5f;
	[SerializeField]
	[Range(0,1f)]
	private float kickingVolume = 1f;

	// All audioclips for footsteps
	private AudioClip[] footStepsAudioClips_Grass;
	private AudioClip[] footStepsAudioClips_Rock;

	// 
	private AudioClip[] footStepsAudioClips_Bridge;

	// All voide audioclips
	private AudioClip[] runningAudioClips;
	private AudioClip[] breathingAudioClips;
	private AudioClip[] kickingAudioClips;

	void Awake(){
		audioSource = GetComponent<AudioSource> ();

		// Load all audio clips at runtime
		footStepsAudioClips_Grass = Resources.LoadAll<AudioClip>("Audio/PapairSounds/Footsteps/Grass");
		footStepsAudioClips_Rock = Resources.LoadAll<AudioClip>("Audio/PapairSounds/Footsteps/Rock");
		footStepsAudioClips_Bridge = Resources.LoadAll<AudioClip> ("Audio/PapairSounds/Footsteps/Bridge");

		runningAudioClips = Resources.LoadAll<AudioClip>("Audio/PapairSounds/Ruedi/ShortHumms");
		breathingAudioClips = Resources.LoadAll<AudioClip>("Audio/PapairSounds/Ruedi/Breathe");
		kickingAudioClips = Resources.LoadAll<AudioClip>("Audio/PapairSounds/Ruedi/Kick");

	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FootStep(){
		switch (currentSurfaceType) {
		case 0:
			// don't play any audio
			break;
		case 1:
			// grass
			AudioClip grass = footStepsAudioClips_Grass [Random.Range (0, footStepsAudioClips_Grass.Length)];
			audioSource.PlayOneShot(grass, footStepVolume);
			break;
		case 2:
			// rock
			AudioClip rock = footStepsAudioClips_Rock [Random.Range (0, footStepsAudioClips_Rock.Length)];
			audioSource.PlayOneShot(rock, footStepVolume);
			break;
		case 3:
			// bridge
			AudioClip bridge = footStepsAudioClips_Bridge [Random.Range (0, footStepsAudioClips_Bridge.Length)];
			audioSource.PlayOneShot(bridge, footStepVolume);
			break;
		}
	}

	public void KickPlayAudio(){

	}

	public void RunningPlayAudio(){
		audioSource.PlayOneShot (runningAudioClips [Random.Range (0, runningAudioClips.Length)], hummingVolume);
	}

	public void BreathingPlayAudio(){
		audioSource.PlayOneShot (breathingAudioClips [Random.Range (0, breathingAudioClips.Length)], breathingVolume);
	}

	public void KickingPlayAudio(){
		audioSource.PlayOneShot (kickingAudioClips [Random.Range (0, kickingAudioClips.Length)], kickingVolume);
	}

	private void OnTriggerEnter(Collider other){

		switch (other.gameObject.tag) {
		case "SurfaceGrass":
			currentSurfaceType = 1;
			break;
		case "SurfaceRock":
			currentSurfaceType = 2;
			break;
		case "SurfaceBridge":
			currentSurfaceType = 3;
			break;
		default:
			break;
		}

	}

}
