using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour {

	public enum Item{
		Butterfly,
		Blueprint_finished,
		Energycore
	}

	[Header("Items")]
	public int butterflyCount = 0;
	public bool hasBlueprint = false;
	public bool hasEnergycore = false;

	[Header("Fade animation")]
	[SerializeField]
	private float fadeTime = .5f;

	private Image imageButterfly;
	private Text textButterflyCount;
	private Image imageBlueprint;
	private Image imageEnergycore;

	private GameUIControllerScript gameUiControllerScript;

	// Use this for initialization
	void Awake () {
		imageButterfly = transform.FindChild ("GameCanvas/InGameOverlay/InventoryOverlay/Butterfly").GetComponent<Image> ();
		textButterflyCount = transform.FindChild ("GameCanvas/InGameOverlay/InventoryOverlay/ButterflyCount").GetComponent<Text> ();
		imageBlueprint = transform.FindChild ("GameCanvas/InGameOverlay/InventoryOverlay/Blueprint").GetComponent<Image> ();
		imageEnergycore = transform.FindChild ("GameCanvas/InGameOverlay/InventoryOverlay/Energycore").GetComponent<Image> ();

		gameUiControllerScript = GetComponent<GameUIControllerScript> ();

		ClearImageColor (imageButterfly);
		ClearTextColor (textButterflyCount);
		ClearImageColor (imageBlueprint);
		ClearImageColor (imageEnergycore);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddItem(Item item){
		switch (item) {
		case Item.Butterfly:
			butterflyCount += 1;
			CheckButterflyCount();
			break;
		case Item.Blueprint_finished:
			StartCoroutine(FadeImage(imageBlueprint, 0, 1f, fadeTime));
			StartCoroutine(FadeImage(imageButterfly, 1f, 0, fadeTime));
			StartCoroutine(FadeText(textButterflyCount, 1f, 0, fadeTime));
			hasBlueprint = true;

			gameUiControllerScript.DisplayText("Now that I put the blueprints back together I need to get that energycore from the lighthouse.", 3f);

			break;
		case Item.Energycore:
			StartCoroutine(FadeImage(imageEnergycore, 0, 1f, fadeTime));
			hasEnergycore = true;
			break;
		default:
			break;
		}
	}

	public void RemoveItem(Item item){
		switch (item) {
		case Item.Butterfly:
			butterflyCount -= 1;
			CheckButterflyCount();
			break;
		case Item.Energycore:
			hasEnergycore = false;
			break;
		default:
			break;
		}
	}

	public void Show(){
		if (hasEnergycore)
			FillImageColor (imageEnergycore);
		if (butterflyCount > 0 && !hasBlueprint) {
			FillImageColor (imageButterfly);
			FillTextColor (textButterflyCount);
		}
		if (hasBlueprint)
			FillImageColor (imageBlueprint);
	}

	public void Hide(){
		ClearImageColor (imageEnergycore);
		ClearImageColor (imageButterfly);
		ClearTextColor (textButterflyCount);
		ClearImageColor (imageBlueprint);
	}

	private void CheckButterflyCount(){
		textButterflyCount.text = " x " + butterflyCount;
		if (butterflyCount == 1) {
			StartCoroutine(FadeImage(imageButterfly, 0, 1f, fadeTime));
			StartCoroutine(FadeText(textButterflyCount, 0, 1f, fadeTime));
		}
		else if (butterflyCount >= 6)
			AddItem (Item.Blueprint_finished);
		else if (butterflyCount < 0)
			butterflyCount = 0;
	}

	private void ClearImageColor(Image image){
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
	}

	private void FillImageColor (Image image){
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
	}

	private void ClearTextColor(Text text){
		text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
	}
	
	private void FillTextColor(Text text){
		text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
	}

	private IEnumerator FadeImage(Image image, float from, float to, float time){
		for (float t = 0; t < time; t += Time.deltaTime) {
			image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp (from, to, t/time));
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator FadeText(Text text, float from, float to, float time){
		for (float t = 0; t < time; t += Time.deltaTime) {
			text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp (from, to, t/time));
			yield return new WaitForEndOfFrame();
		}
	}

}
