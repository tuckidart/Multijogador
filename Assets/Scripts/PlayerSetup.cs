using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

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
		}
	}

}
