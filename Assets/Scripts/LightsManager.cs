using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LightsManager : NetworkBehaviour {

	[SerializeField]
	private float timeBetweenLightTransitions;

	private List<LightScript> lights;

//	[SyncVar]
	public int currentActiveLightIndex;

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

		Choose ();
	}

//	[Command]
	void Choose()
	{
//		int aux = Random.Range (0, lights.Count);
//		RpcChooseStartingLights (aux);

		currentActiveLightIndex = 0;
		lights [0].ToggleLight ();

		InvokeRepeating("GoToNextLight", timeBetweenLightTransitions, timeBetweenLightTransitions);
	}

	[ClientRpc]
	void RpcChooseStartingLights(int index)
	{
		currentActiveLightIndex = index;
		lights [index].ToggleLight ();
	}

//	[ClientRpc]
	void GoToNextLight ()
	{
		lights [currentActiveLightIndex].ToggleLight ();

		currentActiveLightIndex++;

		if (currentActiveLightIndex >= lights.Count)
			currentActiveLightIndex = 0;

		lights [currentActiveLightIndex].ToggleLight ();
	}
}