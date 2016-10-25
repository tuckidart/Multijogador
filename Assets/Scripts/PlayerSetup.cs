using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {
	public Camera cam;

	private GameObject newItem;

	public GameObject suspectPointPrefab;
	public GameObject policePointPrefab;
	private GameObject minimap;

	[SerializeField]
	Behaviour[] componentsToDisable;

	public override void OnStartLocalPlayer ()
	{
		cam = Camera.main;
		//cam.GetComponent<cam>().carObj = this.gameObject;
		cam.GetComponent<CameraFollow>().player = this.gameObject;
		cam.GetComponent<CameraFollow> ().enabled = true;
		cam.GetComponent<transparentBuildings> ().enabled = true;
		cam.GetComponent<transparentBuildings> ().player = this.gameObject;
	}

	void Start()
	{
		if (!base.isLocalPlayer)
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
				minimap.GetComponent<bl_MiniMap> ().LevelName = "Objective - Blend in and escape!";
			}
		}

		bl_MMItemInfo myPosition = new bl_MMItemInfo(transform.position);

		if(isServer)
			CmdCreateInitialPoint (myPosition);
	}
		
	[Command]
	public void CmdCreateInitialPoint(bl_MMItemInfo item)
	{
		if(gameObject.tag == "Cop")
			newItem = Instantiate (policePointPrefab, item.Position, Quaternion.identity) as GameObject;
		else
			newItem = Instantiate (suspectPointPrefab, item.Position, Quaternion.identity) as GameObject;
		//bl _MiniMapItem mmItem = newItem.GetComponent<bl_MiniMapItem> ();

		NetworkServer.Spawn (newItem);
		Invoke ("DestroyPoint", 5f);
	}
	void DestroyPoint()
	{
		newItem.GetComponent<bl_MiniMapItem>().RpcDestroyItem(true);
	}
}