using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour {

	public enum Item{
		Key,
		Butterfly,
		Blueprint_finished,
		Energycore
	}

	[Header("Items")]
	public bool hasKey = false;
	public int butterflyCount = 0;
	public bool hasBlueprint = false;
	public bool hasCore = false;

	[Header("Fade animation")]
	[SerializeField]
	private float fadeTime = .5f;

	private Image imageKey;
	private Image imageButterfly;
	private Image imageBlueprint;
	private Image imageEnergycore;

	// Use this for initialization
	void Awake () {
		imageKey = this.transform.FindChild ("GameCanvas/InGameOverlay/InventoryOverlay/Key").GetComponent<Image> ();
		ClearImageColor (imageKey);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddItem(Item item){
		switch (item) {
		case Item.Key:
			StartCoroutine(FadeImage(imageKey, 0f, 1f, fadeTime));
			hasKey = true;
			break;
		case Item.Butterfly:
			butterflyCount += 1;
			CheckButterflyCount();
			break;
		case Item.Energycore:
			hasCore = true;
			break;
		default:
			break;
		}
	}

	public void RemoveItem(Item item){
		switch (item) {
		case Item.Key:
			StartCoroutine(FadeImage(imageKey, 1f, 0f, fadeTime));
			hasKey = false;
			break;
		case Item.Butterfly:
			butterflyCount -= 1;
			CheckButterflyCount();
			break;
		case Item.Energycore:
			hasCore = false;
			break;
		default:
			break;
		}
	}

	public void Show(){
		FillImageColor (imageKey);
	}

	public void Hide(){
		ClearImageColor (imageKey);
	}

	private void CheckButterflyCount(){
		if (butterflyCount >= 6)
			hasBlueprint = true;
		else if (butterflyCount < 0)
			butterflyCount = 0;
	}

	private void ClearImageColor(Image image){
		image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
	}

	private void FillImageColor (Image image){
		image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
	}

	private IEnumerator FadeImage(Image image, float from, float to, float time){
		for (float t = 0; t < time; t += Time.deltaTime) {
			image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp (from, to, t/time));
			yield return new WaitForEndOfFrame();
		}
	}

}
