using UnityEngine;
using System.Collections;

public class TextAreaTriggerScript : MonoBehaviour {

	[Header("What will be displayed?")]
	public string text = "Insert text here...";
	[Header("How long will the text be visible? (in seconds)")]
	public float time = 1f;
	
	public bool onlyOnce = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
