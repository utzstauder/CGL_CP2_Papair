using UnityEngine;
using System.Collections;

public class CharacterAudioController : MonoBehaviour {
	
	// private global only
	private AudioSource audioSource;
	
	private int currentSurfaceType = 1;

	// All audioclips for footsteps
	private AudioClip[] footStepsAudioClips_SolidMetal;
	private AudioClip[] kickAudioClips;

	void Awake(){
		audioSource = GetComponent<AudioSource> ();

		// Load all audio clips at runtime
		footStepsAudioClips_SolidMetal = Resources.LoadAll<AudioClip>("Audio/SFX/Footsteps/SolidMetal");
		kickAudioClips = Resources.LoadAll<AudioClip> ("Audio/SFX/Kick");
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FootStep(){
		AudioClip audioClip = footStepsAudioClips_SolidMetal [Random.Range (0, footStepsAudioClips_SolidMetal.Length)];

		switch (currentSurfaceType) {
		case 0:
			// don't play any audio
			break;
		case 1:
			// SolidMetal
			audioSource.PlayOneShot(audioClip);
			break;
		}
	}

	public void KickPlayAudio(){
		AudioClip audioClip = kickAudioClips [Random.Range (0, kickAudioClips.Length)];
		audioSource.PlayOneShot(audioClip);
	}

}
