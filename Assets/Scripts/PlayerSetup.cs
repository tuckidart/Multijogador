using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class MyMessage : MessageBase
{
	public NetworkInstanceId netId;
	public int stuff;
	public GameObject scape;
}

public class PlayerSetup : NetworkBehaviour {
	public Camera cam;

	private GameObject newItem;

	public GameObject suspectPointPrefab;
	public GameObject policePointPrefab;
	private GameObject minimap;

	[SerializeField]
	Behaviour[] componentsToDisable;

	short MyMsgId = 1000;
	public override void OnStartClient()
	{
		NetworkManager.singleton.client.RegisterHandler (MyMsgId, OnMyMsg);
	}

	public override void OnStartLocalPlayer ()
	{
		cam = Camera.main;
		cam.GetComponent<CameraFollow>().player = this.gameObject;
		cam.GetComponent<CameraFollow> ().enabled = true;
		cam.GetComponent<transparentBuildings> ().enabled = true;
		cam.GetComponent<transparentBuildings> ().player = this.gameObject;
	}

	void Start()
	{
		if (!base.isLocalPlayer && componentsToDisable.Length > 0)
		{
			for (int i = 0; i < componentsToDisable.Length; i++)
			{
				componentsToDisable [i].enabled = false;
			}
		}
		else
		{
			minimap = GameObject.Find ("MiniMap").gameObject;
			minimap.GetComponent<bl_MiniMap> ().m_Target = gameObject;
			minimap.GetComponent<bl_MMCompass> ().Target = gameObject.transform;

			if(gameObject.tag == "Cop")
			{
				minimap.GetComponent<bl_MiniMap> ().LevelName = "Objective - Locate, and Apprehend!";
			}
			else if(gameObject.tag == "Suspect")
			{
				minimap.GetComponent<bl_MiniMap> ().LevelName = "Objective - Blend in, rob stores and escape!";
			}
		}

		bl_MMItemInfo myPosition = new bl_MMItemInfo(transform.position);

		if(isServer)
			CmdCreateInitialPoint (myPosition);
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space))
			CmdSendToMe ();
	}
		
	[Command]
	public void CmdCreateInitialPoint(bl_MMItemInfo item)
	{
		if(gameObject.tag == "Cop")
			newItem = Instantiate (policePointPrefab, item.Position, Quaternion.identity) as GameObject;
		else
			newItem = Instantiate (suspectPointPrefab, item.Position, Quaternion.identity) as GameObject;

		NetworkServer.Spawn (newItem);
		Invoke ("DestroyPoint", 5f);
	}
	void DestroyPoint()
	{
		newItem.GetComponent<bl_MiniMapItem>().RpcDestroyItem(true);
	}

	[Command]
	void CmdSendToMe()
	{
		var msg = new MyMessage ();
		msg.stuff = 2456986;
		msg.netId = netId;

		base.connectionToClient.Send (MyMsgId, msg);
	}

	void DoStuff(int stuff)
	{
		Debug.Log ("Got msg " + stuff + " for " + gameObject);
	}

	static void OnMyMsg(NetworkMessage netMsg)
	{
		var msg = netMsg.ReadMessage<MyMessage> ();
		var player = ClientScene.FindLocalObject (msg.netId);
		player.GetComponent<PlayerSetup> ().DoStuff (msg.stuff);
	}
}