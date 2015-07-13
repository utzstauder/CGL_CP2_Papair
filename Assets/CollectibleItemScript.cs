using UnityEngine;
using System.Collections;

public class CollectibleItemScript : MonoBehaviour {

	public InventoryScript.Item itemType;

	public int butterflyNumber = 0;

	private InventoryScript inventoryScript;
	private GameManagerScript gameManagerScript;
	private GameUIControllerScript gameUIControllerScript;

	// Use this for initialization
	void Awake () {
		inventoryScript = GameObject.Find ("GameManager").GetComponent<InventoryScript> ();
		gameManagerScript = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
		gameUIControllerScript = GameObject.Find ("GameManager").GetComponent<GameUIControllerScript> ();

		if (itemType== InventoryScript.Item.Energycore && gameManagerScript.collectedEnergycore)
				Destroy(this.gameObject);
		else if (itemType == InventoryScript.Item.Butterfly){
			switch (butterflyNumber){
			case 1:
				if (gameManagerScript.collectedButterfly1)
					Destroy(this.gameObject);
				break;
			case 2:
				if (gameManagerScript.collectedButterfly2)
					Destroy(this.gameObject);
				break;
			case 3:
				if (gameManagerScript.collectedButterfly3)
					Destroy(this.gameObject);
				break;
			case 4:
				if (gameManagerScript.collectedButterfly4)
					Destroy(this.gameObject);
				break;
			case 5:
				if (gameManagerScript.collectedButterfly5)
					Destroy(this.gameObject);
				break;
			case 6:
				if (gameManagerScript.collectedButterfly6)
					Destroy(this.gameObject);
				break;
			default:
				break;
			}
		}

	}
	
	// Update is called once per frame
	public void OnGrab () {
		if (itemType == InventoryScript.Item.Energycore && (!gameManagerScript.activatedSwitch1 || !gameManagerScript.activatedSwitch2)) {
			gameUIControllerScript.DisplayText("I can't grab this just yet. There is still some juices flowing!", 2f);
		} else {
			inventoryScript.AddItem(itemType);
			gameManagerScript.SetItemSwitch(itemType, butterflyNumber);
			Destroy (this.gameObject);
		}
	}
	
}
