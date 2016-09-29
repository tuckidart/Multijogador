using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ScapeController : NetworkBehaviour {

	public int timeToScape;
	public GameObject scapePointPrefab;
	public List<Transform> scapePositions;

	private int durationTime;
	private float startTime;
	private bool scapeIsOpen;
	private int currentScapePointIndex;

	private bool lateStartCalled;

	void Awake ()
	{
		startTime = Time.time;
	}

	// Use this for initialization
	void Start () 
	{
		if (!isServer)
			return;
		
		//ChooseCurrentScape ();
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
		currentScapePointIndex = Random.Range (0, scapePositions.Count);
	}

	void SetScapeToOpen ()
	{
		scapeIsOpen = true;

		if (isServer) 
		{
			//scapePositions [currentScapePointIndex].gameObject.SetActive (true);
			CmdInstantiateScapeObject ();

			Debug.Log ("Opened the Scape");
		}
	}

	[Command]
	void CmdInstantiateScapeObject ()
	{
		RpcSetCurrentScape ();

		GameObject tempPoint;

		tempPoint = Instantiate (scapePointPrefab, scapePositions [currentScapePointIndex].position, Quaternion.identity) as GameObject;

		NetworkServer.Spawn (tempPoint);
	}

	[ClientRpc]
	void RpcSetCurrentScape ()
	{
		ChooseCurrentScape ();
	}

	private IEnumerator LateStart ()
	{
		yield return new WaitForSeconds (1f);
	
		durationTime = (int)_GameMaster.GM.myTimeControl.durationTime;

		lateStartCalled = true;
	}
}
