using UnityEngine;
using System.Collections;

public class ButterflySoundScript : MonoBehaviour {

	private AudioSource audioSource;

	private AudioClip[] wingsAudioClips;

	[SerializeField]
	[Range(0,1f)]
	private float wingVolume = 1f; 

	// Use this for initialization
	void Awake () {
		audioSource = GetComponent<AudioSource> ();

		wingsAudioClips = Resources.LoadAll<AudioClip>("Audio/PapairSounds/Wildlife/Butterfly");
	}
	
	// Update is called once per frame
	public void WingsPlayAudio () {
		audioSource.PlayOneShot (wingsAudioClips [Random.Range (0, wingsAudioClips.Length)], wingVolume);
	}
}
