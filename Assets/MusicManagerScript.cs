using UnityEngine;
using System.Collections;

public class MusicManagerScript : MonoBehaviour {

	[SerializeField]
	private AudioSource musicMenu;
	[SerializeField]
	private AudioSource musicCaveArea;
	[SerializeField]
	private AudioSource musicYardArea;
	[SerializeField]
	private AudioSource musicForestArea;
	[SerializeField]
	private AudioSource musicTestingGroundArea;
	[SerializeField]
	private AudioSource musicLighthouseArea;
	[SerializeField]
	private AudioSource musicCockpitArea;

	AudioSource[] audioSources;

	// Use this for initialization
	void Awake () {
		audioSources = new AudioSource[] {musicMenu, musicCaveArea, musicYardArea, musicForestArea, musicTestingGroundArea, musicLighthouseArea, musicCockpitArea};

		// silence all music
		foreach (AudioSource audioSource in audioSources) {
			audioSource.volume = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// Keep audio tracks in sync each frame
		foreach (AudioSource slave in audioSources) {
			if (slave != musicCaveArea) slave.timeSamples = musicCaveArea.timeSamples;
		}
	}

	public void PlayAll(){
		foreach (AudioSource audioSource in audioSources) {
			audioSource.Play ();
		}
	}

	public IEnumerator FadeToMusicOfLevel(int level, float fadeTime){
		for (float t = 0; t <= fadeTime; t += Time.deltaTime){
			for (int i = 0; i < audioSources.Length; i++){
				if (i != level && audioSources[i].volume > 0){
					audioSources[i].volume = Mathf.Lerp(1f, 0, t/fadeTime);
				} else if (i == level) audioSources[i].volume = Mathf.Lerp(0, 1f, t/fadeTime);
			}
			yield return new WaitForEndOfFrame();
		}
		foreach (AudioSource audioSource in audioSources) {
			audioSource.volume = 0;
		}
		audioSources [level].volume = 1;
	}
}
