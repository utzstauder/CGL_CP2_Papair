using UnityEngine;
using System.Collections;

public class CableAnimationScript : MonoBehaviour {

	private GameObject[] cableJoints;
	public bool active = true;

	[SerializeField]
	private Material cableMaterial;

	[SerializeField]
	private Vector2 waitRange = Vector2.zero;
	[SerializeField]
	private ParticleSystem particleSystem;

	[SerializeField]
	private bool deactivateBySwitch1 = false;
	[SerializeField]
	private bool deactivateBySwitch2= false;
	[SerializeField]
	private bool deactivateByEnergycore = false;
	
	private GameManagerScript gameManagerScript;

	// Use this for initialization
	void Awake () {
		gameManagerScript = GameObject.Find ("GameManager").GetComponent<GameManagerScript> ();

		cableJoints = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) {
			cableJoints[i] = transform.GetChild (i).gameObject;
			if (cableJoints[i].GetComponent<Renderer>())
				cableJoints[i].GetComponent<Renderer>().material = cableMaterial;
		}

		if ((deactivateBySwitch1 && gameManagerScript.activatedSwitch1) ||
			(deactivateBySwitch2 && gameManagerScript.activatedSwitch2) ||
			(deactivateByEnergycore && gameManagerScript.collectedEnergycore)) {
			active = false;
			Destroy (this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if ((deactivateBySwitch1 && gameManagerScript.activatedSwitch1) ||
		    (deactivateBySwitch2 && gameManagerScript.activatedSwitch2) ||
		    (deactivateByEnergycore && gameManagerScript.collectedEnergycore)) {
			active = false;
			Destroy (this);
		}

		if (active) {
			StartCoroutine (RandomParticles ());
		} else
			StopAllCoroutines ();
	}

	private void SetColor (Color color){
		cableMaterial.SetColor ("_Color", color);
	}

	private IEnumerator RandomParticles(){
		yield return new WaitForSeconds (Random.Range (waitRange.x, waitRange.y));
		ParticleSystem particles = Instantiate (particleSystem, cableJoints [Random.Range (0, cableJoints.Length)].transform.position, Quaternion.identity) as ParticleSystem;
		Destroy (particles.gameObject, .4f);
	}
}
