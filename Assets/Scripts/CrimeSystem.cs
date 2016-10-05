using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CrimeSystem : NetworkBehaviour {

	private List<GameObject> newItem = new List<GameObject>();
	public GameObject crimePointPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision hit)
	{
		if (hit.relativeVelocity.magnitude > 10.0f && hit.gameObject.name != "Ground" && hit.gameObject.name != "Curb" && hit.gameObject.tag != "Police")
		{
			bl_MMItemInfo myPosition = new bl_MMItemInfo(transform.position);

			if(isServer)
				CmdCreateCrimePoint (myPosition);
		}
	}

	[Command]
	public void CmdCreateCrimePoint(bl_MMItemInfo item)
	{
		GameObject temp = Instantiate (crimePointPrefab, item.Position, Quaternion.identity) as GameObject;
		newItem.Add(temp);

		NetworkServer.Spawn (temp);
		Invoke ("DestroyPoint", 8f);
	}
	void DestroyPoint()
	{
		newItem[0].GetComponent<bl_MiniMapItem>().RpcDestroyItem(true);
		Destroy (newItem[0].gameObject);
		newItem.RemoveAt(0);
	}
}
