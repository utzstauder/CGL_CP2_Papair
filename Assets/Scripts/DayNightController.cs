using UnityEngine;
using System.Collections;

public class DayNightController : MonoBehaviour {

	private Light sun;
	private float sunInitialIntensity;

	[SerializeField]
	private float secondsInFullDay = 120f;
	[SerializeField]
	[Range(0,1)]
	private float currentTimeOfDay = 0;
	private float timeMultiplier = 1f;

	public bool clockRunning = true;

	// Use this for initialization
	void Start () {
		sun = GetComponent<Light> ();
		sunInitialIntensity = sun.intensity;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateSun ();

		if (clockRunning) {
			currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

			if (currentTimeOfDay >= 1)
				currentTimeOfDay = 0;
		}
	}

	private void UpdateSun(){
		sun.transform.localRotation = Quaternion.Euler ((currentTimeOfDay * 360f) - 90, 170, 0);

		float intensityMultiplier = 1f;

		// Night
		if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
			intensityMultiplier = 0;
		else if (currentTimeOfDay <= 0.25f)
			intensityMultiplier = Mathf.Clamp01 ((currentTimeOfDay - 0.23f) * (1 / 0.02f));
		else if (currentTimeOfDay >= 0.73f)
			intensityMultiplier = 1f - Mathf.Clamp01 ((currentTimeOfDay - 0.73f) * (1 / 0.02f));

		//Debug.Log (intensityMultiplier);
		sun.intensity = sunInitialIntensity * intensityMultiplier;
	}
}
