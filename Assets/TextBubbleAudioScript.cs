using UnityEngine;
using System.Collections;

public class TextBubbleAudioScript : MonoBehaviour {

	private AudioSource audioSource;

	[SerializeField]
	private AudioClip audioClipOn;
	[SerializeField]
	private AudioClip audioClipOff;

	// Use this for initialization
	void Awake () {
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlayAudioOn(){
		audioSource.PlayOneShot (audioClipOn);
	}

	public void PlayAudioOff(){
		audioSource.PlayOneShot (audioClipOff);
	}
}
