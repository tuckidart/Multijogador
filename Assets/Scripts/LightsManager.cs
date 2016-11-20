using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightsManager : MonoBehaviour {

	[SerializeField]
	private float timeBetweenLightTransitions;

	private List<LightScript> lights;

	private int currentActiveLightIndex;

	void Awake ()
	{
		lights = new List<LightScript> ();
	}

	void Start () 
	{
		for (int i = 0; i < transform.childCount; i++) 
		{
			lights.Add (transform.GetChild(i).gameObject.GetComponent<LightScript> ());
		}

		//int aux = Random.Range (0, lights.Count);
		currentActiveLightIndex = 0;
		lights [0].ToggleLight ();

		InvokeRepeating("GoToNextLight", timeBetweenLightTransitions, timeBetweenLightTransitions);
	}

	void GoToNextLight ()
	{
		lights [currentActiveLightIndex].ToggleLight ();

		currentActiveLightIndex++;

		if (currentActiveLightIndex >= lights.Count)
			currentActiveLightIndex = 0;

		lights [currentActiveLightIndex].ToggleLight ();
	}
}