using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ScapeController : NetworkBehaviour {
	
	public GameObject scapePrefab;
	public int timeToScape;
	public List<Transform> scapePositions;

	private int durationTime;
	private float startTime;
	private bool scapeIsOpen;

	private int currentScapeIndex;

	private int tempIndex;

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
		tempIndex = Random.Range (0, scapePositions.Count);
		//currentScapeIndex = tempIndex;
		RpcChangeIndex ();
	}

	void SetScapeToOpen ()
	{
		scapeIsOpen = true;

		CmdIntantiatePrefab();
		
		//Debug.Log ("Opened the Scape");
	}

	[Command]
	void CmdIntantiatePrefab()
	{
		GameObject temp = Instantiate (scapePrefab, scapePositions[currentScapeIndex].position, Quaternion.identity) as GameObject;
		
		NetworkServer.Spawn (temp);
	}

	[ClientRpc]
	void RpcChangeIndex()
	{
		currentScapeIndex = tempIndex;
	}

	private IEnumerator LateStart ()
	{
		yield return new WaitForSeconds (1f);
	
		durationTime = (int)_GameMaster.GM.myTimeControl.durationTime;

		lateStartCalled = true;
	}
}