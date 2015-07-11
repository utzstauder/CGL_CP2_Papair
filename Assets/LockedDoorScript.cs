using UnityEngine;
using System.Collections;

public class LockedDoorScript : MonoBehaviour {

	private GameManagerScript gameManager;
	private InventoryScript inventoryScript;
	private GameUIControllerScript gameUIcontrollerScript;
	[SerializeField]
	private bool itemRequired = true;
	public bool locked = true;
	private bool open = false;
	public InventoryScript.Item requiredItem;

	// Use this for initialization
	void Awake () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManagerScript> ();
		inventoryScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryScript> ();
		gameUIcontrollerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameUIControllerScript> ();

		if (gameManager.doorInCaveOpen)
			OpenDoor ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnKick(){
//		if (!open) {
//			switch (requiredItem){
//			case InventoryScript.Item.Key:
//				if (inventoryScript.hasKey){
//					if (locked)	gameUIcontrollerScript.DisplayText("It's still locked but I got the key now.", 1f);
//					else OpenDoor ();
//				} else {
//					gameUIcontrollerScript.DisplayText("Kicking isn't any good as long as this door is locked.", 1.5f);
//				}
//				break;
//			default:
//				break;
//			}
//		}
	}

	public void OnGrab(){
//		if (!open) {
//			switch (requiredItem){
//			case InventoryScript.Item.Key:
//				if (inventoryScript.hasKey){
//					if (locked){
//						locked = false;
//						gameUIcontrollerScript.DisplayText("*click*", 1f);
//						// TODO: play some sound?
//					} else gameUIcontrollerScript.DisplayText("Now that it is unlocked it still seems stuck.", 1f);
//				} else {
//					gameUIcontrollerScript.DisplayText("It's locked.", .5f);
//
//				}
//				break;
//			default:
//				break;
//			}
//		}
	}

	private void OpenDoor(){
		if (!gameManager.doorInCaveOpen)
			gameManager.doorInCaveOpen = true;
		open = true;
		transform.parent.Rotate (0, -80f, 0);
	}
}
