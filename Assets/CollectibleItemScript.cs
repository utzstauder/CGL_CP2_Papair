using UnityEngine;
using System.Collections;

public class CollectibleItemScript : MonoBehaviour {

	public InventoryScript.Item itemType;

	// Use this for initialization
	void Awake () {
		InventoryScript inventoryScript = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<InventoryScript> ();

		switch (itemType) {
		case InventoryScript.Item.Key:
			if (inventoryScript.hasKey)
				Destroy(this.gameObject);
			break;
		default:
			break;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
