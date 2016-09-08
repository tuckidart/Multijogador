using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	bl_MMItemInfo myPosition;

	private GameObject minimap;

	[SerializeField]
	Behaviour[] componentsToDisable;

	void Start()
	{
		if(!isLocalPlayer)
		{
			for(int i=0;i<componentsToDisable.Length;i++)
			{
				componentsToDisable[i].enabled = false;
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

			myPosition = GetComponent<bl_MMItemInfo> ();
			myPosition.Position = transform.position;
			myPosition.Size = 15;
			myPosition.Color = new Color (1, 1, 0);
			CmdCreatePoint ();
		}
	}

	[Command]
	void CmdCreatePoint()
	{
		minimap.GetComponent<bl_MiniMap> ().CreateNewItem (myPosition);
	}
}
