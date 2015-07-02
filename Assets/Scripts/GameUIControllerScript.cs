using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUIControllerScript : MonoBehaviour {

	private bool isDisplaying = false;
	[SerializeField]
	private Text textObject;
	[SerializeField]
	private float textFadeTime = .5f;

	// Use this for initialization
	void Awake () {
		textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisplayText(string text, float time){
		if (!isDisplaying) StartCoroutine (Text (text, time));
	}

	private IEnumerator Text(string text, float time){
		isDisplaying = true;
		textObject.text = text;

		StartCoroutine(FadeText(0f, 1f, textFadeTime));
		yield return new WaitForSeconds(textFadeTime + time);

		StartCoroutine(FadeText(1f, 0f, textFadeTime));
		yield return new WaitForSeconds(textFadeTime);

		isDisplaying = false;
	}

	private IEnumerator FadeText(float from, float to, float time){
		for (float t = 0; t <= time; t += Time.deltaTime) {
			textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, Mathf.Lerp (from, to, t/time));
			yield return new WaitForEndOfFrame();
		}
		textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, to);
	}
}
