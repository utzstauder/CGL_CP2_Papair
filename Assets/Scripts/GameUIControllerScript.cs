using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUIControllerScript : MonoBehaviour {

	private bool isDisplaying = false;
	[SerializeField]
	private Text textObject;
	[SerializeField]
	private Text areaTextObject;
	[SerializeField]
	private Image inGameTextPanel;
	private Animator textBubbleAnimator;
	[SerializeField]
	private float textFadeTime = .5f;


	// Use this for initialization
	void Awake () {
		textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0);
		areaTextObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 0);
		textBubbleAnimator = inGameTextPanel.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DisplayText(string text, float time){
		if (!isDisplaying) StartCoroutine (Text (textObject, text, time));
	}

	public void DisplayAreaText(string text, float time){
		if (!isDisplaying) StartCoroutine(Text (areaTextObject, text, time));
	}

	public void ClearAll(float time){
		StartCoroutine(FadeText(textObject, 1f, 0f, time));
		StartCoroutine(FadeText(areaTextObject, 1f, 0f, time));
		isDisplaying = false;
	}

	private IEnumerator Text(Text textObject, string text, float time){
		isDisplaying = true;
		textObject.text = text;

		if (textObject != areaTextObject) {
			textBubbleAnimator.SetBool ("showBubble", true);
			yield return new WaitForSeconds (1f);
		}

		StartCoroutine(FadeText(textObject, 0f, 1f, textFadeTime));
		yield return new WaitForSeconds(textFadeTime + time);

		StartCoroutine(FadeText(textObject, 1f, 0f, textFadeTime));
		yield return new WaitForSeconds(textFadeTime);

		if (textObject != areaTextObject) {
			textBubbleAnimator.SetBool ("showBubble", false);
			yield return new WaitForSeconds (1f);
		}

		isDisplaying = false;
	}

	private IEnumerator FadeText(Text textObject, float from, float to, float time){
		for (float t = 0; t <= time; t += Time.deltaTime) {
			textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, Mathf.Lerp (from, to, t/time));
			yield return new WaitForEndOfFrame();
		}
		textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, to);
	}
}
