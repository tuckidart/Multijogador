using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ScapeController : NetworkBehaviour {

	public GameObject scapePointPrefab;

	public int timeToScape;
	public List<Transform> scapePositions;

	private int durationTime;
	private float startTime;
	private bool scapeIsOpen;

	private int currentScapeIndex;

	private bool lateStartCalled;

	void Awake ()
	{
		startTime = Time.time;
	}

	// Use this for initialization
	void Start ()
	{
		if(isServer)
			ChooseCurrentScape ();
		
		StartCoroutine (LateStart ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!scapeIsOpen && lateStartCalled)
		{
			if ((Time.time - startTime) >=  durationTime -  timeToScape)
			{
				SetScapeToOpen ();
			}
		}
	}

	void ChooseCurrentScape ()
	{
		int tempIndex = Random.Range (0, scapePositions.Count);
		RpcChangeIndex (tempIndex);
	}

	void SetScapeToOpen ()
	{
		scapeIsOpen = true;

		CmdIntantiatePrefab();
	}

	[Command]
	void CmdIntantiatePrefab()
	{
		GameObject temp = Instantiate (scapePointPrefab, scapePositions[currentScapeIndex].position, scapePositions[currentScapeIndex].rotation) as GameObject;
		
		NetworkServer.Spawn (temp);
	}

	[ClientRpc]
	void RpcChangeIndex(int index)
	{
		currentScapeIndex = index;
	}

	private IEnumerator LateStart ()
	{
		yield return new WaitForSeconds (1f);
	
		durationTime = (int)_GameMaster.GM.myTimeControl.durationTime;

		lateStartCalled = true;
	}
}