using UnityEngine;
using System.Collections;

public class EnergycoreCockpitScript : MonoBehaviour {

	private GameManagerScript gameManagerScript;
	private InventoryScript inventoryScript;

	// Use this for initialization
	void Awake () {
		gameManagerScript = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();
		inventoryScript = GameObject.Find ("GameManager").GetComponent<InventoryScript> ();
	}

	public void OnGrab(){
		if (inventoryScript.hasEnergycore) {
			inventoryScript.RemoveItem (InventoryScript.Item.Energycore);
			gameManagerScript.placedEnergycore = true;
			transform.FindChild ("Object001").gameObject.SetActive (true);
			Destroy (this);
		}
	}
}
