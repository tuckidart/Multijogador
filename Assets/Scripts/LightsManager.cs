using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LightsManager : NetworkBehaviour {

	[SerializeField]
	private float timeBetweenLightTransitions;

	private List<LightScript> lights;

	[SyncVar]
	public int currentActiveLightIndex;

	void Awake ()
	{
		lights = new List<LightScript> ();
	}

	void Start () 
	{
		if (!isServer)
			return;
		
		for (int i = 0; i < transform.childCount; i++)
		{
			lights.Add (transform.GetChild(i).gameObject.GetComponent<LightScript> ());
		}

		CmdChoose ();
	}

	[Command]
	void CmdChoose()
	{
		int aux = Random.Range (0, lights.Count);
		RpcChooseStartingLights (aux);

		InvokeRepeating("RpcGoToNextLight", timeBetweenLightTransitions, timeBetweenLightTransitions);
	}

	[ClientRpc]
	void RpcChooseStartingLights(int index)
	{
		currentActiveLightIndex = index;
		lights [index].RpcToggleLight ();
	}

	[ClientRpc]
	void RpcGoToNextLight ()
	{
		lights [currentActiveLightIndex].RpcToggleLight ();

		currentActiveLightIndex++;

		if (currentActiveLightIndex >= lights.Count)
			currentActiveLightIndex = 0;

		lights [currentActiveLightIndex].RpcToggleLight ();
	}
}